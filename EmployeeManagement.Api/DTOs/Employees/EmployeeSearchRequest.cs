using System.ComponentModel.DataAnnotations;
using EmployeeManagement.Core.Enums;

namespace EmployeeManagement.Api.DTOs.Employees;

public class EmployeeSearchRequest
{
    public string? Search { get; set; }
    public EmployeeStatus? Status { get; set; }
    public Guid? DepartmentId { get; set; }
    [Range(1, int.MaxValue)]
    public int PageNumber { get; set; } = 1;
    [Range(1, 100)]
    public int PageSize { get; set; } = 10;
}