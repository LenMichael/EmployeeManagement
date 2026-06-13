using EmployeeManagement.Api.Data.Seed;
using EmployeeManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeeManagement.Api.Data.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Projects");

        builder.HasKey(project => project.Id);

        builder.Property(project => project.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(project => project.Description)
            .HasMaxLength(500);

        builder.Property(project => project.StartDate)
            .IsRequired();

        builder.Property(project => project.EndDate)
            .IsRequired(false);

        builder.HasIndex(project => project.Name)
            .IsUnique();

        builder.HasData(SeedData.Projects);
    }
}