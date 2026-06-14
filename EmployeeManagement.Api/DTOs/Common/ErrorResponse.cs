namespace EmployeeManagement.Api.DTOs.Common;

public class ErrorResponse
{
    public string Message { get; init; } = string.Empty;
    public string? Details { get; init; }
    public string TraceId { get; init; } = string.Empty;
}