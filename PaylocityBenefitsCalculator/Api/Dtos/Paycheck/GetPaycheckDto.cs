using Api.Contracts.Paycheck;

namespace Api.Dtos.Paycheck;

public sealed record GetPaycheckDto
{
    public required PaycheckDto[] Paycheck { get; init; }

    public static GetPaycheckDto Create(PaycheckModel paycheckModel)
    {
        return new GetPaycheckDto
        {
            Paycheck = paycheckModel.Paycheck
                .Select(p => new PaycheckDto
                {
                    MonthNumber = p.monthNumber, SalaryAmount = p.salaryAmmount
                })
                .OrderBy(pay => pay.MonthNumber).ToArray(),
        };
    }
}

public sealed record PaycheckDto
{
    public int MonthNumber { get; init; }
    public decimal SalaryAmount { get; init; }
}