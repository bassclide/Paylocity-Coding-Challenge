using Api.Models;

namespace Api.Dtos.Dependent;

public sealed record CreateDependentDto
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public DateTime DateOfBirth { get; init; }
    public Relationship Relationship { get; set; }
}