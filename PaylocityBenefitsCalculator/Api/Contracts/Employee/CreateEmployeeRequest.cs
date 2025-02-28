using Api.Contracts.Dependent;

namespace Api.Contracts.Employee;

public sealed record CreateEmployeeRequest
{
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public decimal Salary { get; init; }
    public DateTime DateOfBirth { get; init; }
    public ICollection<DependentModel> Dependents { get; init; } = new List<DependentModel>();
}