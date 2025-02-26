namespace Api.Contracts.Paycheck;

public sealed record PaycheckModel
{
    public required (int monthNumber, decimal salaryAmmount)[] Paycheck { get; init; }

    public static PaycheckModel Create(decimal[] salaries)
    {
        return new PaycheckModel
        {
            Paycheck = salaries.Select((salary, index) => (index + 1, salary)).ToArray()
        };
    }
}