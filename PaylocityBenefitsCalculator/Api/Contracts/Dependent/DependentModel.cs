using Api.Models;

namespace Api.Contracts.Dependent;

public sealed class DependentModel
{
    public int Id { get; private init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public DateTime DateOfBirth { get; init; }
    public Relationship Relationship { get; init; }

    public static DependentModel Create(Models.Dependent dependent)
    {
        return new DependentModel
        {
            Id = dependent.Id,
            FirstName = dependent.FirstName,
            DateOfBirth = dependent.DateOfBirth,
            LastName = dependent.LastName,
            Relationship = dependent.Relationship
        };
    }
}