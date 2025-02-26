using Api.Models;

namespace Api.Contracts.Dependent;

public sealed record CreateDependentRequest
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public DateTime DateOfBirth { get; init; }
    public int EmployeeId { get; init; }
    public Relationship Relationship { get; init; }
}