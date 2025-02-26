using Api.Contracts.Paycheck;
using Api.Dtos.Paycheck;
using Api.Models;
using Api.Services;
using Api.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/employees/{employeeId:int}/paychecks")]
[ProducesResponseType(typeof(ApiResponse<GetPaycheckDto>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status404NotFound)]
public class PaychecksController(IPaycheckService paycheckService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<GetPaycheckDto>>> Get([FromRoute] int employeeId, int year)
    {
        var request = new CalculatePaychecksRequest
        {
            EmployeeId = employeeId,
            Year = year
        };

        var result = await paycheckService.CalculatePaychecks(request);
        if (!result.IsSuccess)
        {
            return result.ToErrorResponse<GetPaycheckDto>();
        }

        return new ApiResponse<GetPaycheckDto>
        {
            Data = GetPaycheckDto.Create(result)
        };
    }
}