using Api.Contracts.Employee;
using Api.Dtos.Dependent;

namespace Api.Dtos.Employee;

public class GetEmployeeDto
{
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public decimal Salary { get; set; }
    public DateTime DateOfBirth { get; set; }
    public ICollection<GetDependentDto> Dependents { get; set; } = new List<GetDependentDto>();

    public static GetEmployeeDto Create(EmployeeModel employee)
    {
        return new GetEmployeeDto
        {
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Salary = employee.Salary,
            DateOfBirth = employee.DateOfBirth,
            Id = employee.Id,
            Dependents = employee.Dependents.Select(GetDependentDto.Create).ToList()
        };
    }
}
