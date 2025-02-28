using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Employee> Employees { get; set; } = null!;
    public DbSet<Dependent> Dependents { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).HasMaxLength(50).IsRequired(false);
            entity.Property(e => e.LastName).HasMaxLength(50).IsRequired(false);
            entity.Property(e => e.Salary).HasColumnType("decimal(18,2)");
            entity.Property(e => e.DateOfBirth).IsRequired();

            entity.HasMany(e => e.Dependents)
                .WithOne(d => d.Employee)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.ToTable("Employees");
        });

        modelBuilder.Entity<Dependent>(entity =>
        {
            entity.HasKey(d => d.Id);
            entity.Property(d => d.FirstName).HasMaxLength(50).IsRequired(false);
            entity.Property(d => d.LastName).HasMaxLength(50).IsRequired(false);
            entity.Property(d => d.DateOfBirth).IsRequired();
            entity.Property(d => d.Relationship).IsRequired();
            entity.ToTable("Dependents");
        });
    }
}