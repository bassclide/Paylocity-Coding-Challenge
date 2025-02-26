using Api.Models;
using Api.Shared;

namespace Api.SalaryPolicies;

public class OverSalaryPolicy : ISalaryPolicy
{
    private const decimal OverSalaryThreshold = 80000;
    private const decimal OverSalaryRate = 0.02m;

    public decimal Calculate(Employee employee, DateTime from, DateTime to)
    {
        if (employee.Salary > OverSalaryThreshold)
        {
            return ((to - from).Days + 1) * ValuePerDay(from.Year, employee.Salary * OverSalaryRate);
        }
        return 0;
    }
    
    private static decimal ValuePerDay(int year, decimal value)
    {
        if (DateTime.IsLeapYear(year))
        {
            return value / Constants.NumberOfDaysInLeapYear;
        }

        return value / Constants.NumberOfMonthsInYear;
    }

}