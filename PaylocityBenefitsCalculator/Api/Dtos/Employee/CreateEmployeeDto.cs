using System.Diagnostics.CodeAnalysis;
using Api.Dtos.Dependent;

namespace Api.Dtos.Employee;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public sealed record CreateEmployeeDto
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public decimal Salary { get; init; }
    public DateTime DateOfBirth { get; init; }
    
    // ReSharper disable once CollectionNeverUpdated.Global
    public ICollection<DependentDto> Dependents { get; init; } = new List<DependentDto>();
}