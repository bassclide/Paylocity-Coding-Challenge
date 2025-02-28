using Api.Contracts.Employee;
using Api.Errors;
using Api.Models;
using Api.Repositories;
using Api.Shared;
using Microsoft.EntityFrameworkCore;

namespace Api.Services;

public class EmployeesService(AppDbContext dbContext) : IEmployeesService
{
    public async Task<Res<EmployeeModel>> GetEmployeeById(int id)
    {
        var entity = await dbContext.Employees.Include(employee => employee.Dependents)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (entity is null)
        {
            return new EmployeeResourceNotFound();
        }

        return EmployeeModel.Create(entity);
    }

    public async Task<Res<IEnumerable<EmployeeModel>>> GetAllEmployees()
    {
        var entities = await dbContext.Employees.Include(emp => emp.Dependents).ToListAsync();
        return entities.Select(EmployeeModel.Create).ToList();
    }

    public async Task<Res> Create(CreateEmployeeRequest employeeRequest)
    {
        var employee = new Employee
        {
            FirstName = employeeRequest.FirstName,
            LastName = employeeRequest.LastName,
            DateOfBirth = employeeRequest.DateOfBirth,
            Salary = employeeRequest.Salary,
        };

        foreach (var dependent in employeeRequest.Dependents)
        {
            var dependentModel = Dependent.Create(
                dependent.FirstName,
                dependent.LastName,
                dependent.DateOfBirth,
                dependent.Relationship);

            var dependentAdditionResult = employee.AddDependent(dependentModel);
            if (!dependentAdditionResult)
            {
                return dependentAdditionResult;
            }
        }

        dbContext.Employees.Add(employee);
        await dbContext.SaveChangesAsync();

        return Res.Success();
    }
}

public interface IEmployeesService
{
    Task<Res<EmployeeModel>> GetEmployeeById(int id);
    Task<Res<IEnumerable<EmployeeModel>>> GetAllEmployees();

    Task<Res> Create(CreateEmployeeRequest employee);
}