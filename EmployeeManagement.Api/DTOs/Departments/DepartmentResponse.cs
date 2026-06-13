namespace EmployeeManagement.Api.DTOs.Departments;

public class DepartmentResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int EmployeeCount { get; init; }
}