using Api.Contracts.Dependent;
using Api.Contracts.Employee;
using Api.Dtos.Employee;
using Api.Models;
using Api.Services;
using Api.Shared;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/employees")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeesService _employeesService;

    public EmployeesController(IEmployeesService employeesService)
    {
        _employeesService = employeesService;
    }

    [SwaggerOperation(Summary = "Get employee by id")]
    
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<GetEmployeeDto>>> Get(int id)
    {
        var result = await _employeesService.GetEmployeeById(id);
        if (!result.IsSuccess)
        {
            return result.ToErrorResponse();
        }

        return new ApiResponse<GetEmployeeDto>
        {
            Data = GetEmployeeDto.Create(result),
            Success = true
        };
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create employee")]
    public async Task<ActionResult> CreateEmployee(CreateEmployeeDto dependent)
    {
        var entity = new CreateEmployeeRequest
        {
            FirstName = dependent.FirstName,
            LastName = dependent.LastName,
            Salary = dependent.Salary,
            DateOfBirth = dependent.DateOfBirth,
            Dependents = dependent.Dependents.Select(dep => new DependentModel
            {
                FirstName = dep.FirstName!,
                LastName = dep.LastName!,
                DateOfBirth = dep.DateOfBirth,
                Relationship = dep.Relationship,
            }).ToList(),
        };

        var res = await _employeesService.Create(entity);
        if (!res.IsSuccess)
        {
            return res.ToErrorResponse();
        }

        return new CreatedResult();
    }

    [SwaggerOperation(Summary = "Get all employees")]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<GetEmployeeDto>>>> GetAll()
    {
        var res = await _employeesService.GetAllEmployees();
        if (!res.IsSuccess)
        {
            return res.ToErrorResponse<IEnumerable<GetEmployeeDto>>();
        }

        var data = res.Entity.Select(GetEmployeeDto.Create).ToList();
        return new ApiResponse<IEnumerable<GetEmployeeDto>> { Data = data };
    }
}