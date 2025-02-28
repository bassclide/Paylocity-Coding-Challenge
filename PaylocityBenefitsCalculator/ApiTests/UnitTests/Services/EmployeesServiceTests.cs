using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Api.Contracts.Dependent;
using Api.Contracts.Employee;
using Api.Errors;
using Api.Models;
using Api.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ApiTests.UnitTests.Services;

public sealed class EmployeesServiceTests(DatabaseFixture fixture) : IClassFixture<DatabaseFixture>
{
    [Fact]
    public async Task GetEmployeeById_WhenEmployeeDoesNotExist_ReturnsNotFound()
    {
        // arrange
        fixture.Context.Employees.RemoveRange(fixture.Context.Employees);
        await fixture.Context.SaveChangesAsync();
        var employeeService = new EmployeesService(fixture.Context);

        // act
        var res = await employeeService.GetEmployeeById(1);

        // assert
        Assert.NotNull(res);
        Assert.False(res.IsSuccess);
        Assert.IsType<EmployeeResourceNotFound>(res.Error);
        Assert.Equal(ErrorReason.NotFound, res.Error!.Reason);
    }

    [Fact]
    public async Task GetEmployeeById_WithValidEmployee_ReturnsEmployee()
    {
        // arrange
        var employeeService = new EmployeesService(fixture.Context);
        await fixture.Context.Employees.AddAsync(new Employee());
        await fixture.Context.SaveChangesAsync();
        var employee = await fixture.Context.Employees.FirstAsync();

        // act
        var res = await employeeService.GetEmployeeById(employee.Id);

        // assert
        Assert.NotNull(res);
        Assert.True(res.IsSuccess);
    }

    [Fact]
    [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local")]
    public async Task GetAllEmployees_WithValidEmployee_ReturnsEmployee()
    {
        // arrange
        fixture.Context.Employees.RemoveRange(fixture.Context.Employees);
        await fixture.Context.SaveChangesAsync();
        var employeeService = new EmployeesService(fixture.Context);
        var employee = new Employee
        {
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateTime(1985, 1, 1),
            Salary = 10000,
            Dependents =
            [
                new Dependent
                {
                    FirstName = "Jane",
                    LastName = "Doe",
                    Relationship = Relationship.Spouse,
                    DateOfBirth = new DateTime(1990, 7, 7)
                }
            ]
        };
        await fixture.Context.Employees.AddAsync(employee);
        await fixture.Context.SaveChangesAsync();

        // act
        var res = await employeeService.GetAllEmployees();

        // assert
        Assert.NotNull(res);
        Assert.True(res.IsSuccess);
        Assert.Collection(res.Entity, first =>
        {
            Assert.Equal(employee.Salary, first.Salary);
            Assert.Equal(employee.DateOfBirth, first.DateOfBirth);
            Assert.Equal(employee.FirstName, first.FirstName);
            Assert.Equal(employee.LastName, first.LastName);
        });

        Assert.Collection(res.Entity.First().Dependents, first =>
        {
            Assert.Equal(employee.Dependents.First().FirstName, first.FirstName);
            Assert.Equal(employee.Dependents.First().LastName, first.LastName);
            Assert.Equal(employee.Dependents.First().Relationship, first.Relationship);
            Assert.Equal(employee.Dependents.First().DateOfBirth, first.DateOfBirth);
        });
    }

    [Fact]
    [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local")]
    public async Task Create_WhenValidData_ShouldCreateEmployee()
    {
        // arrange
        fixture.Context.Employees.RemoveRange(fixture.Context.Employees);
        await fixture.Context.SaveChangesAsync();
        var employeeService = new EmployeesService(fixture.Context);
        var request = new CreateEmployeeRequest
        {
            Salary = 10000,
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateTime(1985, 1, 1)
        };

        // act
        var res = await employeeService.Create(request);

        // assert
        var employee = await fixture.Context.Employees.FirstAsync();
        Assert.NotNull(res);
        Assert.True(res.IsSuccess);
        Assert.Equal(request.Salary, employee.Salary);
        Assert.Equal(request.FirstName, employee.FirstName);
        Assert.Equal(request.LastName, employee.LastName);
        Assert.Equal(request.DateOfBirth, employee.DateOfBirth);
    }

    [Fact]
    public async Task Create_WhenMultipleDependentsWithSameRelationship_ShouldReturnConflict()
    {
        // arrange
        fixture.Context.Employees.RemoveRange(fixture.Context.Employees);
        await fixture.Context.SaveChangesAsync();

        var first = new DependentModel
        {
            FirstName = "Jane",
            LastName = "Doe",
            Relationship = Relationship.Spouse
        };

        var second = new DependentModel
        {
            FirstName = "Diana",
            LastName = "Doe",
            Relationship = Relationship.DomesticPartner
        };

        var employeeService = new EmployeesService(fixture.Context);
        var request = new CreateEmployeeRequest
        {
            Salary = 10000,
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateTime(1985, 1, 1),
            Dependents = [first, second]
        };

        // act
        var res = await employeeService.Create(request);

        // assert
        Assert.NotNull(res);
        Assert.False(res.IsSuccess);
        Assert.IsType<EmployeeAlreadyHasDependantWithRelationship>(res.Error);
        Assert.Equal(ErrorReason.Conflict, res.Error!.Reason);
    }
}