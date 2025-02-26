using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Api.Dtos.Dependent;
using Api.Models;
using Api.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ApiTests.IntegrationTests;

public sealed class DependentsControllerTests(IntegrationTestWebFactoryFixture fixture)
    : IClassFixture<IntegrationTestWebFactoryFixture>
{
    [Fact]
    public async Task GetAll_WithExistingData_ReturnsAllDependentsForEmployee()
    {
        // arrange
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
        var response = await client.GetAsync($"/api/v1/Employees/{employee.Id}/Dependents");

        // assert
        await response.ShouldReturn<List<GetDependentDto>>(
            HttpStatusCode.OK,
            [
                new GetDependentDto
                {
                    Id = employee.Dependents.First().Id,
                    FirstName = employee.Dependents.First().FirstName,
                    LastName = employee.Dependents.First().LastName,
                    DateOfBirth = employee.Dependents.First().DateOfBirth
                }
            ]);
    }

    [Fact]
    public async Task Get_WithExistingData_ReturnsDependentForEmployee()
    {
        // arrange
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
        var response =
            await client.GetAsync($"/api/v1/Employees/{employee.Id}/Dependents/{employee.Dependents.First().Id}");

        // assert
        await response.ShouldReturn(
            HttpStatusCode.OK,
            new GetDependentDto
            {
                Id = employee.Dependents.First().Id,
                FirstName = employee.Dependents.First().FirstName,
                LastName = employee.Dependents.First().LastName,
                DateOfBirth = employee.Dependents.First().DateOfBirth,
            }
        );
    }

    [Fact]
    public async Task Post_WithValidData_CreatesDependentForEmployee()
    {
        // arrange
        var employee = new Employee
        {
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateTime(1990, 01, 01),
            Salary = 120000,
        };

        using (var scope = fixture.Factory.Services.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<AppDbContext>();
            db.Employees.RemoveRange(db.Employees);
            await db.AddAsync(employee);
            await db.SaveChangesAsync();
        }

        var request = new CreateDependentDto
        {
            FirstName = "Colonel",
            LastName = "Sanders",
            DateOfBirth = new DateTime(1970, 1, 1)
        };

        var client = fixture.Factory.CreateClient();

        // act
        var response =
            await client.PostAsync($"/api/v1/Employees/{employee.Id}/Dependents", JsonContent.Create(request));

        // assert
        await response.ShouldReturn(HttpStatusCode.Created);
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