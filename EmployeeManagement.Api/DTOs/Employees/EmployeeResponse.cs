using EmployeeManagement.Api.DTOs.Departments;
using EmployeeManagement.Api.DTOs.Projects;
using EmployeeManagement.Core.Enums;

namespace EmployeeManagement.Api.DTOs.Responses;

public class EmployeeResponse
{
    public Guid Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public EmployeeStatus Status { get; init; }
    public DateTime HireDate { get; init; }
    public string? Notes { get; init; }
    public DepartmentSummaryResponse? Department { get; init; }
    public IReadOnlyCollection<ProjectSummaryResponse> Projects { get; init; } = Array.Empty<ProjectSummaryResponse>();
}