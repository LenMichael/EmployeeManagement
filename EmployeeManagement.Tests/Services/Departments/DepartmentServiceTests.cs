using EmployeeManagement.Api.Services.Common;
using EmployeeManagement.Api.Services.Departments;
using EmployeeManagement.Core.Entities;
using EmployeeManagement.Core.Enums;
using EmployeeManagement.Tests.TestHelpers;

namespace EmployeeManagement.Tests.Services.Departments;

public class DepartmentServiceTests
{
    [Fact]
    public async Task DeleteDepartmentAsync_ReturnsBadRequest_WhenDepartmentHasEmployees()
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

        var service = new DepartmentService(context);

        var result = await service.DeleteDepartmentAsync(department.Id);

        Assert.Equal(ServiceResultStatus.BadRequest, result.Status);
        Assert.Equal("Cannot delete department because it has assigned employees.", result.ErrorMessage);
    }
}
