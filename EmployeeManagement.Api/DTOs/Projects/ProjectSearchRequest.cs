using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Api.DTOs.Projects;

public class ProjectSearchRequest
{
    public string? Search { get; set; }
    public Guid? EmployeeId { get; set; }
    [Range(1, int.MaxValue)]
    public int PageNumber { get; set; } = 1;
    [Range(1, 100)]
    public int PageSize { get; set; } = 10;
}