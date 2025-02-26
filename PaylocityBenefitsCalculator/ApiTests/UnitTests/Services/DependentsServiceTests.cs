using System.Linq;
using System.Threading.Tasks;
using Api.Contracts.Dependent;
using Api.Errors;
using Api.Models;
using Api.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ApiTests.UnitTests.Services;

public sealed class DependentsServiceTests(DatabaseFixture fixture) : IClassFixture<DatabaseFixture>
{
    [Fact]
    public async Task GetDependent_WhenEmployeeIsNotExisting_ShouldReturnNotFound()
    {
        // arrange
        var dependentService = new DependentsService(fixture.Context);
        const int employeeId = 99999;
        const int dependentId = 99999;

        // act
        var res = await dependentService.GetDependent(employeeId, dependentId);

        // assert
        Assert.NotNull(res);
        Assert.False(res.IsSuccess);
        Assert.IsType<EmployeeResourceNotFound>(res.Error);
        Assert.Equal(ErrorReason.NotFound, res.Error!.Reason);
    }

    [Fact]
    public async Task GetDependent_WhenDependentIsNotExisting_ShouldReturnNotFound()
    {
        // arrange
        fixture.Context.Employees.RemoveRange(fixture.Context.Employees);
        fixture.Context.Employees.Add(new Employee());
        await fixture.Context.SaveChangesAsync();
        var employee = await fixture.Context.Employees.FirstAsync();
        const int dependentId = 99999;

        var dependentService = new DependentsService(fixture.Context);

        // act
        var res = await dependentService.GetDependent(employee.Id, dependentId);

        // assert
        Assert.NotNull(res);
        Assert.False(res.IsSuccess);
        Assert.IsType<DependentResourceNotFound>(res.Error);
        Assert.Equal(ErrorReason.NotFound, res.Error!.Reason);
    }

    [Fact]
    public async Task GetDependent_WhenDependentIsExisting_ShouldReturnDependent()
    {
        // arrange
        var dependent = new Dependent
        {
            Relationship = Relationship.Spouse,
            FirstName = "John",
            LastName = "Doe",
        };
        fixture.Context.Employees.Add(new Employee { Dependents = [dependent] });
        await fixture.Context.SaveChangesAsync();
        var employee = await fixture.Context.Employees.Include(emp => emp.Dependents).FirstAsync();
        var dependentService = new DependentsService(fixture.Context);

        // act
        var res = await dependentService.GetDependent(employee.Id, employee.Dependents.First().Id);

        // assert
        Assert.NotNull(res);
        Assert.True(res.IsSuccess);
    }

    [Fact]
    public async Task AddDependent_WhenEmployeeIsNotExisting_ShouldReturnNotFound()
    {
        // arrange
        var request = new CreateDependentRequest
        {
            Relationship = Relationship.Spouse,
            FirstName = "John",
            LastName = "Doe",
            EmployeeId = 999
        };

        var dependentService = new DependentsService(fixture.Context);

        // act
        var res = await dependentService.AddDependent(request);

        // assert
        Assert.NotNull(res);
        Assert.False(res.IsSuccess);
        Assert.IsType<EmployeeResourceNotFound>(res.Error);
        Assert.Equal(ErrorReason.NotFound, res.Error!.Reason);
    }

    [Theory]
    [InlineData(Relationship.Spouse, Relationship.DomesticPartner)]
    [InlineData(Relationship.Spouse, Relationship.Spouse)]
    [InlineData(Relationship.DomesticPartner, Relationship.DomesticPartner)]
    [InlineData(Relationship.DomesticPartner, Relationship.Spouse)]
    public async Task AddDependent_WhenEmployeeAlreadyHasDependentWithSameRelationship_ShouldReturnConflict(
        Relationship first, Relationship second)
    {
        // arrange
        var dependent = new Dependent
        {
            Relationship = first,
            FirstName = "Dona",
            LastName = "Doe",
        };

        fixture.Context.Employees.Add(new Employee { Dependents = [dependent] });
        await fixture.Context.SaveChangesAsync();
        var employee = await fixture.Context.Employees.FirstAsync();
        var request = new CreateDependentRequest
        {
            Relationship = second,
            FirstName = "John",
            LastName = "Doe",
            EmployeeId = employee.Id
        };

        var dependentService = new DependentsService(fixture.Context);

        // act
        var res = await dependentService.AddDependent(request);

        // assert
        Assert.NotNull(res);
        Assert.False(res.IsSuccess);
        Assert.IsType<EmployeeAlreadyHasDependantWithRelationship>(res.Error);
        Assert.Equal(ErrorReason.Conflict, res.Error!.Reason);
    }

    [Fact]
    public async Task AddDependent_WhenValidData_ShouldAddDependent()
    {
        // arrange
        fixture.Context.Employees.RemoveRange(fixture.Context.Employees);
        fixture.Context.Employees.Add(new Employee());
        await fixture.Context.SaveChangesAsync();
        var employee = await fixture.Context.Employees.FirstAsync();
        var request = new CreateDependentRequest
        {
            Relationship = Relationship.Spouse,
            FirstName = "John",
            LastName = "Doe",
            EmployeeId = employee.Id
        };

        var dependentService = new DependentsService(fixture.Context);

        // act
        var res = await dependentService.AddDependent(request);

        // assert
        var alternated = await fixture.Context.Employees.Include(emp => emp.Dependents)
            .FirstAsync(emp => emp.Id == employee.Id);
        Assert.NotNull(res);
        Assert.True(res.IsSuccess);
        Assert.NotNull(alternated.Dependents);
        Assert.Single(alternated.Dependents);
    }

    [Fact]
    public async Task GetAll_WhenEmployeeDoesNotExist_ShouldReturnNotFound()
    {
        // arrange
        var dependentService = new DependentsService(fixture.Context);
        const int employeeId = 99999;

        // act
        var res = await dependentService.GetAll(employeeId);

        // assert
        Assert.NotNull(res);
        Assert.False(res.IsSuccess);
        Assert.IsType<EmployeeResourceNotFound>(res.Error);
        Assert.Equal(ErrorReason.NotFound, res.Error!.Reason);
    }

    [Fact]
    public async Task GetAll_WithValidData_ShouldReturnValidData()
    {
        // arrange
        fixture.Context.Employees.RemoveRange(fixture.Context.Employees);
        fixture.Context.Employees.Add(new Employee
        {
            Dependents =
            [
                new Dependent
                {
                    FirstName = "Colonel",
                    LastName = "Sanders",
                    Relationship = Relationship.Spouse
                }
            ]
        });
        await fixture.Context.SaveChangesAsync();
        var employee = await fixture.Context.Employees.Include(emp => emp.Dependents).FirstAsync();
        var dependentService = new DependentsService(fixture.Context);

        // act
        var res = await dependentService.GetAll(employee.Id);

        // assert
        Assert.NotNull(res);
        Assert.True(res.IsSuccess);
        Assert.NotNull(res.Entity);
        Assert.Collection(res.Entity, dependent => Assert.Equal(Relationship.Spouse, dependent.Relationship));
    }
}