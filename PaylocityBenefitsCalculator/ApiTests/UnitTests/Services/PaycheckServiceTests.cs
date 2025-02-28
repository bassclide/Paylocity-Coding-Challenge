using System;
using System.Linq;
using System.Threading.Tasks;
using Api.Contracts.Paycheck;
using Api.Models;
using Api.SalaryPolicies;
using Api.Services;
using Xunit;

namespace ApiTests.UnitTests.Services;

public sealed class PaycheckServiceTests(DatabaseFixture fixture) : IClassFixture<DatabaseFixture>
{
    [Fact]
    public async Task CalculatePaychecks_WhenNoPoliciesProvided_ShouldReturnPaychecks()
    {
        // arrange
        var paycheckService = new PaycheckService([], fixture.Context);
        var employee = new Employee { Salary = 75000 };
        fixture.Context.Employees.Add(employee);
        await fixture.Context.SaveChangesAsync();
        
        var request = new CalculatePaychecksRequest
        {
            Year = 2025,
            EmployeeId = employee.Id
        };
        
        // act
        var paycheckResponse = await paycheckService.CalculatePaychecks(request);

        // assert
        Assert.NotNull(paycheckResponse);
        Assert.True(paycheckResponse.IsSuccess);
        var paycheckModel = paycheckResponse.Entity;
        Assert.True(Math.Abs(paycheckModel.Paycheck.Sum(y => y.salaryAmmount) - employee.Salary) < 0.00000001m);
    }

    [Fact]
    public async Task CalculatePaychecks_WithBasePolicy_ShouldReturnPaychecks()
    {
        // arrange
        const decimal baseCost = 1000;
        var paycheckService = new PaycheckService([new BaseCostPolicy()], fixture.Context);
        var employee = new Employee { Salary = 75000 };
        fixture.Context.Employees.Add(employee);
        await fixture.Context.SaveChangesAsync();
        var request = new CalculatePaychecksRequest
        {
            Year = 2025,
            EmployeeId = employee.Id
        };
        
        // act
        var paycheckResult = await paycheckService.CalculatePaychecks(request);

        // assert
        Assert.NotNull(paycheckResult);
        Assert.True(paycheckResult.IsSuccess);
        var paycheckModel = paycheckResult.Entity;
        Assert.True(Math.Abs(paycheckModel.Paycheck.Sum(y => y.salaryAmmount) + 12 * baseCost - employee.Salary) < 0.00000001m);
    }

    [Fact]
    public async Task CalculatePaychecks_WithOverSalary_ShouldReturnPaychecks()
    {
        // arrange
        const decimal overSalaryPartitioner = 0.98m;
        var paycheckService = new PaycheckService([new OverSalaryPolicy()], fixture.Context);
        var employee = new Employee { Salary = 85000 };
        fixture.Context.Employees.Add(employee);
        await fixture.Context.SaveChangesAsync();
        var request = new CalculatePaychecksRequest
        {
            Year = 2025,
            EmployeeId = employee.Id
        };

        // act
        var paycheckResult = await paycheckService.CalculatePaychecks(request);

        // assert
        Assert.NotNull(paycheckResult);
        Assert.True(paycheckResult.IsSuccess);
        var paycheckModel = paycheckResult.Entity;
        Assert.True(Math.Abs(paycheckModel.Paycheck.Sum(y => y.salaryAmmount) - employee.Salary * overSalaryPartitioner) < 0.00000001m);
    }

    [Fact]
    public async Task CalculatePaychecks_WithDependentsPolicy_ShouldReturnPaychecks()
    {
        // arrange
        const decimal dependentCost = 600m * 12;
        var paycheckService = new PaycheckService([new DependentsPolicy()], fixture.Context);
        var employee = new Employee
        {
            Salary = 85000, Dependents =
            [
                new Dependent
                {
                    FirstName = "Thomas A.",
                    LastName = "Anderson",
                }
            ]
        };
        fixture.Context.Employees.Add(employee);
        await fixture.Context.SaveChangesAsync();
        var request = new CalculatePaychecksRequest
        {
            Year = 2025,
            EmployeeId = employee.Id
        };

        // act
        var paycheckResult = await paycheckService.CalculatePaychecks(request);

        // assert
        Assert.NotNull(paycheckResult);
        Assert.True(paycheckResult.IsSuccess);
        var paycheckModel = paycheckResult.Entity;
        Assert.True(Math.Abs(paycheckModel.Paycheck.Sum(y => y.salaryAmmount) - (employee.Salary - dependentCost)) < 0.00000001m);
    }

    [Fact]
    public async Task CalculatePaychecks_WithElderlyDependentsPolicy_ShouldReturnPaychecks()
    {
        // arrange
        const decimal dependentCost = 200m * 12;
        var paycheckService = new PaycheckService([new ElderlyDependentsPolicy()], fixture.Context);
        var employee = new Employee
        {
            Salary = 85000, Dependents =
            [
                new Dependent
                {
                    FirstName = "Thomas",
                    LastName = "Anderson",
                    DateOfBirth = new DateTime(1950, 1, 1)
                }
            ]
        };
        fixture.Context.Employees.Add(employee);
        await fixture.Context.SaveChangesAsync();

        // act
        var request = new CalculatePaychecksRequest
        {
            Year = 2025,
            EmployeeId = employee.Id
        };
        var paycheckResult = await paycheckService.CalculatePaychecks(request);

        // assert
        Assert.NotNull(paycheckResult);
        Assert.True(paycheckResult.IsSuccess);
        var paycheckModel = paycheckResult.Entity;
        Assert.True(Math.Abs(paycheckModel.Paycheck.Sum(y => y.salaryAmmount) - (employee.Salary - dependentCost)) < 0.00000001m);
    }

    [Fact]
    public async Task CalculatePaychecks_WithAllPolicies_ShouldReturnPaychecks()
    {
        // arrange
        const decimal expectedSum = 61700m;
        var employee = new Employee
        {
            Salary = 85000, Dependents =
            [
                new Dependent
                {
                    FirstName = "Colonel",
                    LastName = "Sanders",
                    DateOfBirth = new DateTime(1950, 1, 1)
                }
            ]
        };
        fixture.Context.Employees.Add(employee);
        await fixture.Context.SaveChangesAsync();

        var paycheckService = new PaycheckService(
            [
                new ElderlyDependentsPolicy(),
                new DependentsPolicy(),
                new OverSalaryPolicy(),
                new BaseCostPolicy()
            ],
            fixture.Context);

        var request = new CalculatePaychecksRequest
        {
            Year = 2025,
            EmployeeId = employee.Id
        };
        
        // act
        var paycheckResult = await paycheckService.CalculatePaychecks(request);

        // assert
        Assert.NotNull(paycheckResult);
        Assert.True(paycheckResult.IsSuccess);
        var paycheckModel = paycheckResult.Entity;
        Assert.True(Math.Abs(paycheckModel.Paycheck.Sum(y => y.salaryAmmount) - expectedSum) < 0.00000001m);
    }
}