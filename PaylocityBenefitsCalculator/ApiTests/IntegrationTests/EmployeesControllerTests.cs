using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Api.Dtos.Employee;
using Api.Models;
using Api.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ApiTests.IntegrationTests;

public sealed class EmployeesControllerTests(IntegrationTestWebFactoryFixture fixture)
    : IClassFixture<IntegrationTestWebFactoryFixture>
{
    [Fact]
    public async Task GetAll_WithExistingData_ReturnsAllEmployees()
    {
        // arrange
        var employee = new Employee
        {
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateTime(1990, 01, 01),
            Salary = 120000
        };

        using (var scope = fixture.Factory.Services.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<AppDbContext>();
            db.Employees.RemoveRange(db.Employees);

            await db.AddAsync(employee);
            await db.SaveChangesAsync();
        }

        var client = fixture.Factory.CreateClient();

        // act
        var response = await client.GetAsync("/api/v1/Employees");

        // assert
        await response.ShouldReturn<List<GetEmployeeDto>>(
            HttpStatusCode.OK,
        [
            new GetEmployeeDto
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                DateOfBirth = employee.DateOfBirth,
                Salary = employee.Salary
            }
        ]);
    }
    
    [Fact]
    public async Task GetById_WhenEmployeeIsNotExisting_ReturnsNotFound()
    {
        // arrange
        using (var scope = fixture.Factory.Services.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<AppDbContext>();
            db.Employees.RemoveRange(db.Employees);
        }

        const int employeeId = 1;
        var client = fixture.Factory.CreateClient();

        // act
        var response = await client.GetAsync($"/api/v1/Employees/{employeeId}");

        // assert
        await response.ShouldReturn(HttpStatusCode.NotFound);
    }
}