using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.DTOs.Common;
using EmployeeManagement.Api.DTOs.Employees;
using EmployeeManagement.Api.DTOs.Projects;
using EmployeeManagement.Api.Services.Common;
using EmployeeManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Services.Projects;

public class ProjectService : IProjectService
{
    private readonly ApplicationDbContext _context;

    public ProjectService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResponse<ProjectResponse>> GetProjectsAsync(ProjectSearchRequest request)
    {
        int pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        int pageSize = request.PageSize < 1 ? 10 : Math.Min(request.PageSize, 100);

        IQueryable<Project> query = _context.Projects
            .Include(project => project.EmployeeProjects)
                .ThenInclude(employeeProject => employeeProject.Employee)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            string searchTerm = request.Search.Trim();

            query = query.Where(project =>
                project.Name.Contains(searchTerm) ||
                (project.Description != null && project.Description.Contains(searchTerm)));
        }

        if (request.EmployeeId.HasValue)
        {
            query = query.Where(project =>
                project.EmployeeProjects.Any(employeeProject =>
                    employeeProject.EmployeeId == request.EmployeeId.Value));
        }

        int totalCount = await query.CountAsync();

        List<Project> projects = await query
            .OrderBy(project => project.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResponse<ProjectResponse>
        {
            Items = projects.Select(MapToResponse).ToList(),
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<ProjectResponse?> GetProjectByIdAsync(Guid id)
    {
        Project? project = await GetProjectWithDetailsAsync(id, asNoTracking: true);

        return project is null
            ? null
            : MapToResponse(project);
    }

    public async Task<ServiceResult<ProjectResponse>> CreateProjectAsync(CreateProjectRequest request)
    {
        if (request.EndDate.HasValue && request.EndDate.Value.Date < request.StartDate.Date)
        {
            return ServiceResult<ProjectResponse>.BadRequest("End date cannot be earlier than start date.");
        }

        string projectName = request.Name.Trim();

        bool nameAlreadyExists = await _context.Projects
            .AnyAsync(project => project.Name == projectName);

        if (nameAlreadyExists)
        {
            return ServiceResult<ProjectResponse>.Conflict("Project name already exists.");
        }

        List<Guid> employeeIds = request.EmployeeIds
            .Distinct()
            .ToList();

        List<Guid> existingEmployeeIds = await _context.Employees
            .Where(employee => employeeIds.Contains(employee.Id))
            .Select(employee => employee.Id)
            .ToListAsync();

        if (existingEmployeeIds.Count != employeeIds.Count)
        {
            return ServiceResult<ProjectResponse>.BadRequest("One or more employees do not exist.");
        }

        var project = new Project
        {
            Name = projectName,
            Description = request.Description,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            EmployeeProjects = existingEmployeeIds
                .Select(employeeId => new EmployeeProject
                {
                    EmployeeId = employeeId
                })
                .ToList()
        };

        _context.Projects.Add(project);

        await _context.SaveChangesAsync();

        Project createdProject = await GetProjectWithDetailsAsync(project.Id, asNoTracking: true)
            ?? project;

        return ServiceResult<ProjectResponse>.Success(MapToResponse(createdProject));
    }

    public async Task<ServiceResult> UpdateProjectAsync(Guid id, UpdateProjectRequest request)
    {
        if (request.EndDate.HasValue && request.EndDate.Value.Date < request.StartDate.Date)
        {
            return ServiceResult.BadRequest("End date cannot be earlier than start date.");
        }

        Project? project = await _context.Projects
            .Include(project => project.EmployeeProjects)
            .FirstOrDefaultAsync(project => project.Id == id);

        if (project is null)
        {
            return ServiceResult.NotFound();
        }

        string projectName = request.Name.Trim();

        bool nameAlreadyExists = await _context.Projects
            .AnyAsync(projectRecord =>
                projectRecord.Id != id &&
                projectRecord.Name == projectName);

        if (nameAlreadyExists)
        {
            return ServiceResult.Conflict("Project name already exists.");
        }

        List<Guid> employeeIds = request.EmployeeIds
            .Distinct()
            .ToList();

        List<Guid> existingEmployeeIds = await _context.Employees
            .Where(employee => employeeIds.Contains(employee.Id))
            .Select(employee => employee.Id)
            .ToListAsync();

        if (existingEmployeeIds.Count != employeeIds.Count)
        {
            return ServiceResult.BadRequest("One or more employees do not exist.");
        }

        project.Name = projectName;
        project.Description = request.Description;
        project.StartDate = request.StartDate;
        project.EndDate = request.EndDate;

        _context.EmployeeProjects.RemoveRange(project.EmployeeProjects);

        project.EmployeeProjects = existingEmployeeIds
            .Select(employeeId => new EmployeeProject
            {
                ProjectId = project.Id,
                EmployeeId = employeeId
            })
            .ToList();

        await _context.SaveChangesAsync();

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> DeleteProjectAsync(Guid id)
    {
        Project? project = await _context.Projects
            .FirstOrDefaultAsync(project => project.Id == id);

        if (project is null)
        {
            return ServiceResult.NotFound();
        }

        _context.Projects.Remove(project);

        await _context.SaveChangesAsync();

        return ServiceResult.Success();
    }

    private async Task<Project?> GetProjectWithDetailsAsync(Guid id, bool asNoTracking)
    {
        IQueryable<Project> query = _context.Projects
            .Include(project => project.EmployeeProjects)
                .ThenInclude(employeeProject => employeeProject.Employee);

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(project => project.Id == id);
    }

    private static ProjectResponse MapToResponse(Project project)
    {
        return new ProjectResponse
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            EmployeeCount = project.EmployeeProjects.Count,
            Employees = project.EmployeeProjects
                .Where(employeeProject => employeeProject.Employee is not null)
                .OrderBy(employeeProject => employeeProject.Employee.LastName)
                .ThenBy(employeeProject => employeeProject.Employee.FirstName)
                .Select(employeeProject => new EmployeeSummaryResponse
                {
                    Id = employeeProject.Employee.Id,
                    FirstName = employeeProject.Employee.FirstName,
                    LastName = employeeProject.Employee.LastName,
                    Email = employeeProject.Employee.Email
                })
                .ToList()
        };
    }
}