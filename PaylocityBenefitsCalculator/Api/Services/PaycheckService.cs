using Api.Contracts.Paycheck;
using Api.Errors;
using Api.Models;
using Api.Repositories;
using Api.SalaryPolicies;
using Api.Shared;
using Microsoft.EntityFrameworkCore;

namespace Api.Services;

public sealed class PaycheckService(IEnumerable<ISalaryPolicy> policies, AppDbContext context) : IPaycheckService
{
    public async Task<Res<PaycheckModel>> CalculatePaychecks(CalculatePaychecksRequest calculatePaychecksRequest)
    {
        var employee = await context.Employees.Include(emp => emp.Dependents)
            .FirstOrDefaultAsync(emp => emp.Id == calculatePaychecksRequest.EmployeeId);
        if (employee is null)
        {
            return new EmployeeResourceNotFound();
        }

        var paycheckCalculations = CalculatePaychecks(employee, calculatePaychecksRequest.Year);
        return PaycheckModel.Create(paycheckCalculations);
    }

    private decimal[] CalculatePaychecks(Employee employee, int year)
    {
        var weeks = SplitYearIntoIntervals(year);
        var paychecks = new decimal[weeks.Length];
        for (var i = 0; i < weeks.Length; i++)
        {
            var deductible = 0m;
            foreach (var policy in policies)
            {
                deductible += policy.Calculate(employee, weeks[i].from, weeks[i].to);
            }

            paychecks[i] = (employee.Salary / 26) - deductible;
        }

        return paychecks;
    }

    private static (DateTime from, DateTime to)[] SplitYearIntoIntervals(int year)
    {
        const int intervals = 26;
        var startDate = new DateTime(year, 1, 1);
        var daysInYear = DateTime.IsLeapYear(year) ? 366 : 365;

        var daysPerInterval = daysInYear / intervals;
        var remainingDays = daysInYear % intervals;

        var intervalsList = new List<(DateTime from, DateTime to)>();
        var currentStartDate = startDate;

        for (var i = 0; i < intervals; i++)
        {
            var extraDay = (i < remainingDays) ? 1 : 0;
            var nextStartDate = currentStartDate.AddDays(daysPerInterval + extraDay);

            intervalsList.Add((currentStartDate, nextStartDate.AddDays(-1)));
            currentStartDate = nextStartDate;
        }

        return intervalsList.ToArray();
    }
}

public interface IPaycheckService
{
    Task<Res<PaycheckModel>> CalculatePaychecks(CalculatePaychecksRequest calculatePaychecksRequest);
}