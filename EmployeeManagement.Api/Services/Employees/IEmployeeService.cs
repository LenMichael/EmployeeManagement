using EmployeeManagement.Api.DTOs.Common;
using EmployeeManagement.Api.DTOs.Employees;
using EmployeeManagement.Api.DTOs.Requests;
using EmployeeManagement.Api.DTOs.Responses;
using EmployeeManagement.Api.Services.Common;

namespace EmployeeManagement.Api.Services.Employees;

public interface IEmployeeService
{
    Task<PagedResponse<EmployeeResponse>> GetEmployeesAsync(EmployeeSearchRequest request);
    Task<EmployeeResponse?> GetEmployeeByIdAsync(Guid id);
    Task<ServiceResult<EmployeeResponse>> CreateEmployeeAsync(CreateEmployeeRequest request);
    Task<ServiceResult> UpdateEmployeeAsync(Guid id, UpdateEmployeeRequest request);
    Task<ServiceResult> DeleteEmployeeAsync(Guid id);
}