using Api.Errors;
using Api.Shared;

namespace Api.Models;

public sealed class Employee
{
    public int Id { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public decimal Salary { get; init; }
    public DateTime DateOfBirth { get; init; }
    public ICollection<Dependent> Dependents { get; init; } = new List<Dependent>();

    public Res AddDependent(Dependent dependent)
    {
        if (Dependents.Any(d =>
                d.Relationship is Relationship.Spouse or Relationship.DomesticPartner) &&
            dependent.Relationship is Relationship.DomesticPartner or Relationship.Spouse)
        {
            return Res.Failure(new EmployeeAlreadyHasDependantWithRelationship());
        }

        Dependents.Add(dependent);
        return Res.Success();
    }
}