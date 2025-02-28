using Api.Contracts.Dependent;

namespace Api.Contracts.Employee;

public sealed record EmployeeModel
{
    public int Id { get; private init; }
    public string? FirstName { get; private init; }
    public string? LastName { get; private init; }
    public decimal Salary { get; private init; }
    public DateTime DateOfBirth { get; private init; }
    public ICollection<DependentModel> Dependents { get; private init; } = new List<DependentModel>();

    public static EmployeeModel Create(Models.Employee employee)
    {
        return new EmployeeModel
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Salary = employee.Salary,
            DateOfBirth = employee.DateOfBirth,
            Dependents = employee.Dependents.Select(DependentModel.Create).ToList()
        };
    }
}