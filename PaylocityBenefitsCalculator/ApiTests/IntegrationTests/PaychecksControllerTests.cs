using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Api.Dtos.Dependent;
using Api.Dtos.Paycheck;
using Api.Models;
using Api.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ApiTests.IntegrationTests;

public sealed class PayChecksControllerTests(IntegrationTestWebFactoryFixture fixture)
    : IClassFixture<IntegrationTestWebFactoryFixture>
{
    [Fact]
    public async Task Get_WithExistingData_ReturnsPaycheckForEmployeeForYear()
    {
        // arrange
        const int year = 2021;
        var employee = new Employee
        {
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateTime(1990, 01, 01),
            Salary = 120000,
            Dependents = new List<Dependent>
            {
                new()
                {
                    FirstName = "Colonel",
                    LastName = "Sanders",
                    DateOfBirth = new DateTime(1970, 1, 1)
                }
            }
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
        var response = await client.GetAsync($"/api/v1/Employees/{employee.Id}/Paychecks?year={year}");

        // assert
        await response.ShouldReturn(HttpStatusCode.OK);
        
    }

    [Fact]
    public async Task Get_WhenEmployeeIsNotExisting_ReturnsNotFound()
    {
        // arrange
        using (var scope = fixture.Factory.Services.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<AppDbContext>();
            db.Employees.RemoveRange(db.Employees);
        }

        const int employeeId = 1;
        const int year = 2021;
        var client = fixture.Factory.CreateClient();

        // act
        var response = await client.GetAsync($"/api/v1/Employees/{employeeId}/Paychecks?year={year}");

        // assert
        await response.ShouldReturn(HttpStatusCode.NotFound);
    }
}