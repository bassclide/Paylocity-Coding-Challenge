using Api.Models;

namespace Api.SalaryPolicies;

public interface ISalaryPolicy
{
    decimal Calculate(Employee employee, DateTime from, DateTime to);
    
}