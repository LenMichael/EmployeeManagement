using EmployeeManagement.Api.DTOs.Projects;
using EmployeeManagement.Api.Services.Common;
using EmployeeManagement.Api.Services.Projects;
using EmployeeManagement.Core.Entities;
using EmployeeManagement.Core.Enums;
using EmployeeManagement.Tests.TestHelpers;

namespace EmployeeManagement.Tests.Services.Projects;

public class ProjectServiceTests
{
    [Fact]
    public async Task CreateProjectAsync_ReturnsBadRequest_WhenEndDateIsBeforeStartDate()
    {
        await using var context = TestDbContextFactory.Create();
        var service = new ProjectService(context);
        var request = new CreateProjectRequest
        {
            Name = "Apollo",
            StartDate = new DateTime(2024, 2, 1),
            EndDate = new DateTime(2024, 1, 1)
        };

        var result = await service.CreateProjectAsync(request);

        Assert.Equal(ServiceResultStatus.BadRequest, result.Status);
        Assert.Equal("End date cannot be earlier than start date.", result.ErrorMessage);
    }

    [Fact]
    public async Task GetProjectsAsync_FiltersByEmployeeId()
    {
        await using var context = TestDbContextFactory.Create();
        var department = new Department
        {
            Id = Guid.NewGuid(),
            Name = "Engineering"
        };
        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            FirstName = "Alice",
            LastName = "Smith",
            Email = "alice@example.com",
            Status = EmployeeStatus.Active,
            HireDate = new DateTime(2024, 1, 1),
            DepartmentId = department.Id
        };
        var assignedProject = new Project
        {
            Id = Guid.NewGuid(),
            Name = "Apollo",
            StartDate = new DateTime(2024, 1, 1)
        };
        var unassignedProject = new Project
        {
            Id = Guid.NewGuid(),
            Name = "Hermes",
            StartDate = new DateTime(2024, 2, 1)
        };

        context.Departments.Add(department);
        context.Employees.Add(employee);
        context.Projects.AddRange(assignedProject, unassignedProject);
        context.EmployeeProjects.Add(new EmployeeProject
        {
            EmployeeId = employee.Id,
            ProjectId = assignedProject.Id
        });
        await context.SaveChangesAsync();

        var service = new ProjectService(context);

        var response = await service.GetProjectsAsync(new ProjectSearchRequest
        {
            EmployeeId = employee.Id
        });

        Assert.Single(response.Items);
        Assert.Equal(assignedProject.Id, response.Items.Single().Id);
        Assert.Equal(1, response.TotalCount);
    }
}
