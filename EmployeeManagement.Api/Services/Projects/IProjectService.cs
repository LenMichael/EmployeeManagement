using EmployeeManagement.Api.DTOs.Common;
using EmployeeManagement.Api.DTOs.Projects;
using EmployeeManagement.Api.Services.Common;

namespace EmployeeManagement.Api.Services.Projects;

public interface IProjectService
{
    Task<PagedResponse<ProjectResponse>> GetProjectsAsync(ProjectSearchRequest request);
    Task<ProjectResponse?> GetProjectByIdAsync(Guid id);
    Task<ServiceResult<ProjectResponse>> CreateProjectAsync(CreateProjectRequest request);
    Task<ServiceResult> UpdateProjectAsync(Guid id, UpdateProjectRequest request);
    Task<ServiceResult> DeleteProjectAsync(Guid id);
}