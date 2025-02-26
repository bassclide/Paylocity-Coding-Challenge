using Api.Models;
using Api.Shared;

namespace Api.SalaryPolicies;

public class DependentsPolicy : ISalaryPolicy
{
    private const decimal DependentCost = 600;
    public decimal Calculate(Employee employee, DateTime from, DateTime to)
    {
        return ((to - from).Days + 1) * ValuePerDay(from.Year, employee.Dependents.Count * DependentCost);
    }
    
    private static decimal ValuePerDay(int year, decimal value)
    {
        if (DateTime.IsLeapYear(year))
        {
            return value * Constants.NumberOfMonthsInYear / Constants.NumberOfDaysInLeapYear;
        }

        return value * Constants.NumberOfMonthsInYear / Constants.NumberOfDaysInYear;
    }
}