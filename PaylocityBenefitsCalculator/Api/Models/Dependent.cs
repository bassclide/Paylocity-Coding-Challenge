namespace Api.Models;

public class Dependent
{
    public int Id { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public DateTime DateOfBirth { get; init; }
    public Relationship Relationship { get; init; }
    public int EmployeeId { get; init; }
    public Employee? Employee { get; init; }

    public static Dependent Create(string firstName, string lastName, DateTime dateOfBirth, Relationship relationship)
    {
        return new Dependent
        {
            FirstName = firstName,
            LastName = lastName,
            DateOfBirth = dateOfBirth,
            Relationship = relationship
        };
    }
}
