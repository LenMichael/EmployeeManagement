namespace EmployeeManagement.Api.DTOs.Employees;

public class EmployeeSummaryResponse
{
    public Guid Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}".Trim();
}