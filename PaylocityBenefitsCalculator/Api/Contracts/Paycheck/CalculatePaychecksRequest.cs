namespace Api.Contracts.Paycheck;

public sealed record CalculatePaychecksRequest
{
    public int EmployeeId { get; init; }
    public int Year { get; init; }
}