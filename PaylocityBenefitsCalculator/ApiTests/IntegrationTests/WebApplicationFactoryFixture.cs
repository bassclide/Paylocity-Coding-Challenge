using System;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Api.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace ApiTests.IntegrationTests;

public class IntegrationTestWebFactoryFixture : IDisposable, IAsyncDisposable
{
    public readonly IntegrationApplicationFactory<Program> Factory = new();


    public void Dispose()
    {
        Factory.Dispose();
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await Factory.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}

public class IntegrationApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(IDbContextOptionsConfiguration<AppDbContext>));

            services.Remove(dbContextDescriptor!);

            var dbConnectionDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbConnection));

            services.Remove(dbConnectionDescriptor!);

            // Create open SqliteConnection so EF won't automatically close it.
            services.AddSingleton<DbConnection>(_ =>
            {
                var connection = new SqliteConnection("DataSource=:memory:");
                connection.Open();

                return connection;
            });

            services.AddDbContext<AppDbContext>((container, options) =>
            {
                var connection = container.GetRequiredService<DbConnection>();
                options.UseSqlite(connection);
            });
        });
    }
}