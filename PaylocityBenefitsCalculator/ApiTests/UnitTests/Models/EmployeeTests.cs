using Api.Errors;
using Api.Models;
using Xunit;

namespace ApiTests.UnitTests.Models;

public class EmployeeTests
{
    [Fact]
    public void AddDependent_WhenPresentedWithValidData_ShouldAddDependent()
    {
        // arrange
        var employee = new Employee();
        var dependent = new Dependent
        {
            FirstName = "Colonel",
            LastName = "Sanders",
        };


        // act
        var res = employee.AddDependent(dependent);

        // assert
        Assert.True(res);
        Assert.True(employee.Dependents.Count == 1);
    }

    [Fact]
    public void AddDependent_WhenDependentWithSameRelationshipExists_ShouldReturnConflict()
    {
        // arrange
        var dependent = new Dependent
        {
            FirstName = "Colonel",
            LastName = "Sanders",
            Relationship = Relationship.Spouse
        };

        var employee = new Employee
        {
            Dependents = [dependent]
        };


        // act
        var res = employee.AddDependent(dependent);

        // assert
        Assert.False(res);
        Assert.NotNull(res.Error);
        Assert.Equal(ErrorReason.Conflict, res.Error!.Reason);
    }

    [Fact]
    public void AddDependent_WhenDependentContains_ShouldReturnConflict()
    {
        // arrange
        var dependent = new Dependent
        {
            FirstName = "John",
            LastName = "Cena",
            Relationship = Relationship.Spouse
        };

        var employee = new Employee
        {
            Dependents =
            [
                new Dependent
                {
                    FirstName = "Colonel",
                    LastName = "Sanders",
                    Relationship = Relationship.DomesticPartner
                }
            ]
        };
        
        // act
        var res = employee.AddDependent(dependent);

        // assert
        Assert.False(res);
        Assert.NotNull(res.Error);
        Assert.Equal(ErrorReason.Conflict, res.Error!.Reason);
    }
}