using Api.Contracts.Dependent;
using Api.Errors;
using Api.Models;
using Api.Repositories;
using Api.Shared;
using Microsoft.EntityFrameworkCore;

namespace Api.Services;

public sealed class DependentsService(AppDbContext appDbContext) : IDependentService
{
    public async Task<Res<DependentModel>> GetDependent(int employeeId, int dependentId)
    {
        var employee = await appDbContext.Employees.Include(emp => emp.Dependents.Where(dep => dep.Id == dependentId))
            .FirstOrDefaultAsync(emp => emp.Id == employeeId);

        if (employee is null)
        {
            return new EmployeeResourceNotFound();
        }

        if (employee.Dependents.Count == 0)
        {
            return new DependentResourceNotFound();
        }

        return DependentModel.Create(employee.Dependents.First());
    }

    public async Task<Res> AddDependent(CreateDependentRequest dependentRequest)
    {
        var employee = await appDbContext.Employees.FirstOrDefaultAsync(x => x.Id == dependentRequest.EmployeeId);

        if (employee is null)
        {
            return Res.Failure(new EmployeeResourceNotFound());
        }

        var dependent = Dependent.Create(
            dependentRequest.FirstName,
            dependentRequest.LastName,
            dependentRequest.DateOfBirth,
            dependentRequest.Relationship);

        var res = employee.AddDependent(dependent);
        if (!res.IsSuccess)
        {
            return res;
        }

        await appDbContext.SaveChangesAsync();
        return Res.Success();
    }

    public async Task<Res<IEnumerable<DependentModel>>> GetAll(int employeeId)
    {
        var employee = await appDbContext.Employees.Include(emp => emp.Dependents)
            .FirstOrDefaultAsync(x => x.Id == employeeId);

        if (employee is null)
        {
            return new EmployeeResourceNotFound();
        }

        return employee.Dependents.Select(DependentModel.Create).ToList();
    }
}

public interface IDependentService
{
    Task<Res<DependentModel>> GetDependent(int employeeId, int dependentId);
    Task<Res> AddDependent(CreateDependentRequest dependentRequest);
    Task<Res<IEnumerable<DependentModel>>> GetAll(int employeeId);
}