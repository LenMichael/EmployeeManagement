using EmployeeManagement.Api.DTOs.Requests;
using EmployeeManagement.Api.Services.Common;
using EmployeeManagement.Api.Services.Employees;
using EmployeeManagement.Core.Entities;
using EmployeeManagement.Core.Enums;
using EmployeeManagement.Tests.TestHelpers;

namespace EmployeeManagement.Tests.Services.Employees;

public class EmployeeServiceTests
{
    [Fact]
    public async Task CreateEmployeeAsync_ReturnsConflict_WhenEmailAlreadyExists()
    {
        await using var context = TestDbContextFactory.Create();
        var department = new Department
        {
            Id = Guid.NewGuid(),
            Name = "Engineering"
        };

        context.Departments.Add(department);
        context.Employees.Add(new Employee
        {
            FirstName = "Alice",
            LastName = "Smith",
            Email = "alice@example.com",
            Status = EmployeeStatus.Active,
            HireDate = new DateTime(2024, 1, 1),
            DepartmentId = department.Id
        });
        await context.SaveChangesAsync();

        var service = new EmployeeService(context);
        var request = new CreateEmployeeRequest
        {
            FirstName = "Alice",
            LastName = "Jones",
            Email = "alice@example.com",
            Status = EmployeeStatus.Active,
            HireDate = new DateTime(2024, 2, 1),
            DepartmentId = department.Id
        };

        var result = await service.CreateEmployeeAsync(request);

        Assert.Equal(ServiceResultStatus.Conflict, result.Status);
        Assert.Equal("Employee email already exists.", result.ErrorMessage);
    }

    [Fact]
    public async Task CreateEmployeeAsync_ReturnsBadRequest_WhenDepartmentDoesNotExist()
    {
        await using var context = TestDbContextFactory.Create();
        var service = new EmployeeService(context);
        var request = new CreateEmployeeRequest
        {
            FirstName = "Alice",
            LastName = "Smith",
            Email = "alice@example.com",
            Status = EmployeeStatus.Active,
            HireDate = new DateTime(2024, 1, 1),
            DepartmentId = Guid.NewGuid()
        };

        var result = await service.CreateEmployeeAsync(request);

        Assert.Equal(ServiceResultStatus.BadRequest, result.Status);
        Assert.Equal("Department does not exist.", result.ErrorMessage);
    }

    [Fact]
    public async Task UpdateEmployeeAsync_UpdatesAssignedProjects()
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
        var firstProject = new Project
        {
            Id = Guid.NewGuid(),
            Name = "Apollo",
            StartDate = new DateTime(2024, 1, 1)
        };
        var secondProject = new Project
        {
            Id = Guid.NewGuid(),
            Name = "Hermes",
            StartDate = new DateTime(2024, 2, 1)
        };

        context.Departments.Add(department);
        context.Employees.Add(employee);
        context.Projects.AddRange(firstProject, secondProject);
        context.EmployeeProjects.Add(new EmployeeProject
        {
            EmployeeId = employee.Id,
            ProjectId = firstProject.Id
        });
        await context.SaveChangesAsync();

        var service = new EmployeeService(context);
        var request = new UpdateEmployeeRequest
        {
            FirstName = "Alice",
            LastName = "Smith",
            Email = "alice@example.com",
            Status = EmployeeStatus.Active,
            HireDate = new DateTime(2024, 1, 1),
            DepartmentId = department.Id,
            ProjectIds = [secondProject.Id]
        };

        var result = await service.UpdateEmployeeAsync(employee.Id, request);

        Assert.True(result.Succeeded);
        Assert.DoesNotContain(context.EmployeeProjects, assignment => assignment.ProjectId == firstProject.Id);
        Assert.Contains(context.EmployeeProjects, assignment =>
            assignment.EmployeeId == employee.Id &&
            assignment.ProjectId == secondProject.Id);
    }
}
