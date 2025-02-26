using System.Diagnostics.CodeAnalysis;
using Api.Models;

namespace Api.Dtos.Dependent;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public sealed record DependentDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Relationship Relationship { get; set; }
}