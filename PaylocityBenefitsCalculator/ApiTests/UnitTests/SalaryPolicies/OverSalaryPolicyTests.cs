using System;
using Api.Models;
using Api.SalaryPolicies;
using Xunit;

namespace ApiTests.UnitTests.SalaryPolicies;

public sealed class OverSalaryPolicyTests
{
    [Fact]
    public void Calculate_WhenSalaryIsOver_ShouldReturnBenefitFee()
    {
        // arrange
        var overSalaryPolicy = new OverSalaryPolicy();
        var employee = new Employee { Salary = 85000 };
        var dateTime = new DateTime();

        // act
        var deductible = overSalaryPolicy.Calculate(employee, dateTime, dateTime.AddDays(14));
        
        // assert
        Assert.True(deductible > 0);
    }
    
    [Fact]
    public void Calculate_WhenSalaryIsUnder_ShouldReturnZero()
    {
        // arrange
        var overSalaryPolicy = new OverSalaryPolicy();
        var employee = new Employee { Salary = 75000 };
        var dateTime = new DateTime();

        // act
        var deductible = overSalaryPolicy.Calculate(employee, dateTime, dateTime.AddDays(14));
        
        // assert
        Assert.Equal(0, deductible);
    }
}