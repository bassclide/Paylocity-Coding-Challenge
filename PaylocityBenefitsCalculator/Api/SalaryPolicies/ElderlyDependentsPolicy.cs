using Api.Models;
using Api.Shared;

namespace Api.SalaryPolicies;

public class ElderlyDependentsPolicy : ISalaryPolicy
{
    private const decimal ElderlyDependentCost = 200;

    private const int ElderlyAge = 50;

    public decimal Calculate(Employee employee, DateTime from, DateTime to)
    {
        var cost = 0m;
        foreach (var elderly in employee.Dependents.Where(dep => IsElderly(dep, from)))
        {
            cost += ElderlyDependentPerDay(from.Year, elderly.DateOfBirth);
        }

        var days = (to - from).Days + 1;
        
        return cost * days;
    }

    private static bool IsElderly(Dependent dependent, DateTime date)
    {
        var age = date.Year - dependent.DateOfBirth.Year;
        if (date.Month < dependent.DateOfBirth.Month)
        {
            return age - 1 > ElderlyAge;
        }

        return age > ElderlyAge;
    }

    private decimal ElderlyDependentPerDay(int year, DateTime dateOfBirth)
    {
        var lastDayOfYear = new DateTime(year, 12, 31);
        var countOfDays = lastDayOfYear.DayOfYear - dateOfBirth.DayOfYear + 1;
        return ElderlyDependentCost * Constants.NumberOfMonthsInYear / countOfDays;
    }
    

    public static bool Between(DateTime date, DateTime from, DateTime to)
        => date >= from && date <= to;
}