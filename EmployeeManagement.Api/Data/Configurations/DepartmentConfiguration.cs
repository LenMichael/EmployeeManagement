using EmployeeManagement.Api.Data.Seed;
using EmployeeManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeeManagement.Api.Data.Configurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("Departments");

        builder.HasKey(department => department.Id);

        builder.Property(department => department.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(department => department.Description)
            .HasMaxLength(500);

        builder.HasIndex(department => department.Name)
            .IsUnique();

        builder.HasData(SeedData.Departments);
    }
}