using System;
using Api.Models;
using Api.SalaryPolicies;
using Xunit;
using ISalaryPolicy = Api.SalaryPolicies.ISalaryPolicy;

namespace ApiTests.UnitTests.SalaryPolicies;

public class ElderlyDependentsPolicyTests
{
    private readonly ISalaryPolicy _elderlyDependentsPolicy = new ElderlyDependentsPolicy();

    [Fact]
    public void Calculate_WithElderlyDependent_ShouldReturnDeductible()
    {
        // arrange
        var employee = new Employee
        {
            Dependents =
            [
                new Dependent
                {
                    FirstName = "Colonel",
                    LastName = "Sanders", DateOfBirth = new DateTime(1970, 1, 1)
                }
            ]
        };
        var dateTime = new DateTime(2021, 1, 1);

        // act
        var deductible = _elderlyDependentsPolicy.Calculate(employee, dateTime, dateTime.AddDays(364));

        // assert
        Assert.Equal(2400, deductible);
    }

    [Fact]
    public void Calculate_WithDependent_ShouldReturnDeductible()
    {
        // arrange
        var employee = new Employee
        {
            Dependents =
            [
                new Dependent
                {
                    FirstName = "Colonel",
                    LastName = "Sanders",
                    DateOfBirth = new DateTime(2020, 1, 1)
                }
            ]
        };
        var dateTime = new DateTime(2021, 1, 1);

        // act
        var deductible = _elderlyDependentsPolicy.Calculate(employee, dateTime, dateTime.AddDays(364));

        // assert
        Assert.Equal(0, deductible);
    }
}