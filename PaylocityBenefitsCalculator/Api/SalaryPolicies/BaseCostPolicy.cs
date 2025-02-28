using Api.Models;
using Api.Shared;

namespace Api.SalaryPolicies;

public sealed class BaseCostPolicy : ISalaryPolicy
{
    private const decimal BaseCost = 1000;
    public decimal Calculate(Employee employee, DateTime from, DateTime to)
    {
        return ((to - from).Days + 1) * ValuePerDay(from.Year, BaseCost);
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