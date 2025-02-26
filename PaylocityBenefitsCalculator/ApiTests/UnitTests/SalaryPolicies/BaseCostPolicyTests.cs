using System;
using Api.Models;
using Api.SalaryPolicies;
using Xunit;

namespace ApiTests.UnitTests.SalaryPolicies;

public sealed class BaseCostPolicyTests
{
    [Fact]
    public void Calculate_WithEmployee_ShouldReturnBaseCost()
    {
        // arrange
        var overSalaryPolicy = new BaseCostPolicy();
        var employee = new Employee();
        var dateTime = new DateTime(2025,1,1);

        // act
        var deductible = overSalaryPolicy.Calculate(employee, dateTime, dateTime.AddDays(364));
        
        // assert
        Assert.Equal(12000, deductible);
    }
}