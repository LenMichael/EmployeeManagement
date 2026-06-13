using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Api.DTOs.Projects;

public class UpdateProjectRequest
{
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(500)]
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<Guid> EmployeeIds { get; set; } = new();
}