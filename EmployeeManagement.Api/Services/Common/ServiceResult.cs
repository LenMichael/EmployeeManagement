namespace EmployeeManagement.Api.Services.Common;

public enum ServiceResultStatus
{
    Success,
    NotFound,
    BadRequest,
    Conflict
}

public class ServiceResult
{
    public ServiceResultStatus Status { get; init; }
    public string? ErrorMessage { get; init; }

    public bool Succeeded => Status == ServiceResultStatus.Success;

    public static ServiceResult Success() => new()
    {
        Status = ServiceResultStatus.Success
    };

    public static ServiceResult NotFound(string? errorMessage = null) => new()
    {
        Status = ServiceResultStatus.NotFound,
        ErrorMessage = errorMessage
    };

    public static ServiceResult BadRequest(string errorMessage) => new()
    {
        Status = ServiceResultStatus.BadRequest,
        ErrorMessage = errorMessage
    };

    public static ServiceResult Conflict(string errorMessage) => new()
    {
        Status = ServiceResultStatus.Conflict,
        ErrorMessage = errorMessage
    };
}

public class ServiceResult<T> : ServiceResult
{
    public T? Value { get; init; }

    public static ServiceResult<T> Success(T value) => new()
    {
        Status = ServiceResultStatus.Success,
        Value = value
    };

    public new static ServiceResult<T> NotFound(string? errorMessage = null) => new()
    {
        Status = ServiceResultStatus.NotFound,
        ErrorMessage = errorMessage
    };

    public new static ServiceResult<T> BadRequest(string errorMessage) => new()
    {
        Status = ServiceResultStatus.BadRequest,
        ErrorMessage = errorMessage
    };

    public new static ServiceResult<T> Conflict(string errorMessage) => new()
    {
        Status = ServiceResultStatus.Conflict,
        ErrorMessage = errorMessage
    };
}