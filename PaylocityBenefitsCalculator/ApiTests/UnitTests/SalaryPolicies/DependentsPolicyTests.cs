using System;
using Api.Models;
using Api.SalaryPolicies;
using Xunit;
using ISalaryPolicy = Api.SalaryPolicies.ISalaryPolicy;

namespace ApiTests.UnitTests.SalaryPolicies;

public sealed class DependentsPolicyTests
{
    private readonly ISalaryPolicy _dependentsPolicy = new DependentsPolicy();

    [Fact]
    public void Calculate_WithNoDependent_ShouldReturnZero()
    {
        // arrange

        var employee = new Employee { Dependents = [] };
        var dateTime = new DateTime();

        // act
        var deductible = _dependentsPolicy.Calculate(employee, dateTime, dateTime.AddDays(14));

        // assert
        Assert.Equal(0, deductible);
    }

    [Fact]
    public void Calculate_WithDependent_ShouldReturnDeductible()
    {
        // arrange
        const decimal costSum = 600 * 12;
        var employee = new Employee
        {
            Dependents =
            [
                new Dependent
                {
                    FirstName = "Colonel",
                    LastName = "Sanders",
                }
            ]
        };
        var dateTime = new DateTime(2025, 1, 1);

        // act
        var deductible = _dependentsPolicy.Calculate(employee, dateTime, dateTime.AddDays(364));

        // assert
        Assert.Equal(costSum, deductible);
    }
}