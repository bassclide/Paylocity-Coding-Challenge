using System;
using System.Diagnostics.CodeAnalysis;
using Api.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ApiTests.UnitTests;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public sealed class DatabaseFixture : IClassFixture<DatabaseFixture>, IDisposable
{
    public AppDbContext Context { get; }

    public DatabaseFixture()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        Context = new AppDbContext(options);
        Context.Database.EnsureDeleted();
        Context.Database.EnsureCreated();
    }
    
    public void Dispose()
    {
        Context.Dispose();
    }
}