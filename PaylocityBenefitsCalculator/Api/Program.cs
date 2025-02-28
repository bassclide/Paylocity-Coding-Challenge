using Api.Repositories;
using Api.SalaryPolicies;
using Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Employee Benefit Cost Calculation Api",
        Description = "Api to support employee benefit cost calculations"
    });
});

var allowLocalhost = "allow localhost";
builder.Services.AddCors(options =>
{
    options.AddPolicy(allowLocalhost,
        policy => { policy.WithOrigins("http://localhost:3000", "http://localhost"); });
});

builder.Services.AddTransient<IEmployeesService, EmployeesService>();
builder.Services.AddTransient<IDependentService, DependentsService>();
builder.Services.AddTransient<IPaycheckService, PaycheckService>();

//salary policies
builder.Services.AddSingleton<ISalaryPolicy, BaseCostPolicy>();
builder.Services.AddSingleton<ISalaryPolicy, DependentsPolicy>();
builder.Services.AddSingleton<ISalaryPolicy, ElderlyDependentsPolicy>();
builder.Services.AddSingleton<ISalaryPolicy, OverSalaryPolicy>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("DataSource=test-sqlite.db"));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.OpenConnection();
    context.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(allowLocalhost);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

// needed for WebApplicationFactory
public partial class Program { }
