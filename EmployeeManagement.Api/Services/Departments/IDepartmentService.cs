using EmployeeManagement.Api.DTOs.Common;
using EmployeeManagement.Api.DTOs.Departments;
using EmployeeManagement.Api.Services.Common;

namespace EmployeeManagement.Api.Services.Departments;

public interface IDepartmentService
{
    Task<PagedResponse<DepartmentResponse>> GetDepartmentsAsync(DepartmentSearchRequest request);
    Task<DepartmentResponse?> GetDepartmentByIdAsync(Guid id);
    Task<ServiceResult<DepartmentResponse>> CreateDepartmentAsync(CreateDepartmentRequest request);
    Task<ServiceResult> UpdateDepartmentAsync(Guid id, UpdateDepartmentRequest request);
    Task<ServiceResult> DeleteDepartmentAsync(Guid id);
}