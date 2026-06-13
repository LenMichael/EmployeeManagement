using EmployeeManagement.Api.Data.Seed;
using EmployeeManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeeManagement.Api.Data.Configurations;

public class EmployeeProjectConfiguration : IEntityTypeConfiguration<EmployeeProject>
{
    public void Configure(EntityTypeBuilder<EmployeeProject> builder)
    {
        builder.ToTable("EmployeeProjects");

        builder.HasKey(employeeProject => new
        {
            employeeProject.EmployeeId,
            employeeProject.ProjectId
        });

        builder.HasOne(employeeProject => employeeProject.Employee)
            .WithMany(employee => employee.EmployeeProjects)
            .HasForeignKey(employeeProject => employeeProject.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(employeeProject => employeeProject.Project)
            .WithMany(project => project.EmployeeProjects)
            .HasForeignKey(employeeProject => employeeProject.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(SeedData.EmployeeProjects);
    }
}