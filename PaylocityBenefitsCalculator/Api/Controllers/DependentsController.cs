using Api.Contracts.Dependent;
using Api.Dtos.Dependent;
using Api.Models;
using Api.Services;
using Api.Shared;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/employees/{employeeId:int}/dependents")]
public class DependentsController(IDependentService dependentService) : ControllerBase
{
    [SwaggerOperation(Summary = "Get dependent by id for employee")]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<GetDependentDto>>> Get([FromRoute] int employeeId, int id)
    {
        var response = await dependentService.GetDependent(employeeId, id);
        if (!response.IsSuccess)
        {
            return response.ToErrorResponse<GetDependentDto>();
        }

        return new ApiResponse<GetDependentDto>
        {
            Data = GetDependentDto.Create(response.Entity)
        };
    }

    [SwaggerOperation(Summary = "Get all dependents of the employee")]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<GetDependentDto>>>> GetAll([FromRoute] int employeeId)
    {
        var data = await dependentService.GetAll(employeeId);
        if (!data.IsSuccess)
        {
            return data.ToErrorResponse<IEnumerable<GetDependentDto>>();
        }

        var converted = data.Entity.Select(GetDependentDto.Create);
        return new ApiResponse<IEnumerable<GetDependentDto>> { Data = converted.ToList() };
    }

    [SwaggerOperation(Summary = "Create dependent for employee")]
    [HttpPost]
    public async Task<ActionResult> CreateDependent([FromRoute] int employeeId, CreateDependentDto dependent)
    {
        var res = await dependentService.AddDependent(
            new CreateDependentRequest
            {
                FirstName = dependent.FirstName,
                LastName = dependent.LastName,
                DateOfBirth = dependent.DateOfBirth,
                Relationship = dependent.Relationship,
                EmployeeId = employeeId
            });
        if (!res.IsSuccess)
        {
            return res.ToErrorResponse();
        }

        return Created();
    }
}