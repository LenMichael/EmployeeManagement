using EmployeeManagement.Api.Data.Seed;
using EmployeeManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeeManagement.Api.Data.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");

        builder.HasKey(employee => employee.Id);

        builder.Property(employee => employee.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(employee => employee.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(employee => employee.Email)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(employee => employee.Email)
            .IsUnique();

        builder.Property(employee => employee.Status)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(employee => employee.HireDate)
            .IsRequired();

        builder.Property(employee => employee.Notes)
            .HasMaxLength(1000);

        builder.HasOne(employee => employee.Department)
            .WithMany(department => department.Employees)
            .HasForeignKey(employee => employee.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(SeedData.Employees);
    }
}