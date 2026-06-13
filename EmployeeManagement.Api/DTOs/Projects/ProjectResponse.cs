using EmployeeManagement.Api.DTOs.Employees;

namespace EmployeeManagement.Api.DTOs.Projects;

public class ProjectResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public int EmployeeCount { get; init; }
    public IReadOnlyCollection<EmployeeSummaryResponse> Employees { get; init; } = Array.Empty<EmployeeSummaryResponse>();
}