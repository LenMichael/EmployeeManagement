using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.DTOs.Common;
using EmployeeManagement.Api.DTOs.Departments;
using EmployeeManagement.Api.DTOs.Employees;
using EmployeeManagement.Api.DTOs.Projects;
using EmployeeManagement.Api.DTOs.Requests;
using EmployeeManagement.Api.DTOs.Responses;
using EmployeeManagement.Api.Services.Common;
using EmployeeManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Services.Employees;

public class EmployeeService : IEmployeeService
{
    private readonly ApplicationDbContext _context;

    public EmployeeService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResponse<EmployeeResponse>> GetEmployeesAsync(EmployeeSearchRequest request)
    {
        int pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        int pageSize = request.PageSize < 1 ? 10 : Math.Min(request.PageSize, 100);

        IQueryable<Employee> query = _context.Employees
            .Include(employee => employee.Department)
            .Include(employee => employee.EmployeeProjects)
                .ThenInclude(employeeProject => employeeProject.Project)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            string searchTerm = request.Search.Trim();

            query = query.Where(employee =>
                employee.FirstName.Contains(searchTerm) ||
                employee.LastName.Contains(searchTerm) ||
                employee.Email.Contains(searchTerm));
        }

        if (request.Status.HasValue)
        {
            query = query.Where(employee => employee.Status == request.Status.Value);
        }

        if (request.DepartmentId.HasValue)
        {
            query = query.Where(employee => employee.DepartmentId == request.DepartmentId.Value);
        }

        int totalCount = await query.CountAsync();

        List<Employee> employees = await query
            .OrderBy(employee => employee.LastName)
            .ThenBy(employee => employee.FirstName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResponse<EmployeeResponse>
        {
            Items = employees.Select(MapToResponse).ToList(),
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<EmployeeResponse?> GetEmployeeByIdAsync(Guid id)
    {
        Employee? employee = await GetEmployeeWithDetailsAsync(id, asNoTracking: true);

        return employee is null
            ? null
            : MapToResponse(employee);
    }

    public async Task<ServiceResult<EmployeeResponse>> CreateEmployeeAsync(CreateEmployeeRequest request)
    {
        bool departmentExists = await _context.Departments
            .AnyAsync(department => department.Id == request.DepartmentId);

        if (!departmentExists)
        {
            return ServiceResult<EmployeeResponse>.BadRequest("Department does not exist.");
        }

        bool emailAlreadyExists = await _context.Employees
            .AnyAsync(employee => employee.Email == request.Email);

        if (emailAlreadyExists)
        {
            return ServiceResult<EmployeeResponse>.Conflict("Employee email already exists.");
        }

        List<Guid> projectIds = request.ProjectIds
            .Distinct()
            .ToList();

        List<Guid> existingProjectIds = await _context.Projects
            .Where(project => projectIds.Contains(project.Id))
            .Select(project => project.Id)
            .ToListAsync();

        if (existingProjectIds.Count != projectIds.Count)
        {
            return ServiceResult<EmployeeResponse>.BadRequest("One or more projects do not exist.");
        }

        var employee = new Employee
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Status = request.Status,
            HireDate = request.HireDate,
            Notes = request.Notes,
            DepartmentId = request.DepartmentId,
            EmployeeProjects = existingProjectIds
                .Select(projectId => new EmployeeProject
                {
                    ProjectId = projectId
                })
                .ToList()
        };

        _context.Employees.Add(employee);

        await _context.SaveChangesAsync();

        Employee createdEmployee = await GetEmployeeWithDetailsAsync(employee.Id, asNoTracking: true)
            ?? employee;

        return ServiceResult<EmployeeResponse>.Success(MapToResponse(createdEmployee));
    }

    public async Task<ServiceResult> UpdateEmployeeAsync(Guid id, UpdateEmployeeRequest request)
    {
        Employee? employee = await _context.Employees
            .Include(employee => employee.EmployeeProjects)
            .FirstOrDefaultAsync(employee => employee.Id == id);

        if (employee is null)
        {
            return ServiceResult.NotFound();
        }

        bool departmentExists = await _context.Departments
            .AnyAsync(department => department.Id == request.DepartmentId);

        if (!departmentExists)
        {
            return ServiceResult.BadRequest("Department does not exist.");
        }

        bool emailAlreadyExists = await _context.Employees
            .AnyAsync(employeeRecord =>
                employeeRecord.Id != id &&
                employeeRecord.Email == request.Email);

        if (emailAlreadyExists)
        {
            return ServiceResult.Conflict("Employee email already exists.");
        }

        List<Guid> projectIds = request.ProjectIds
            .Distinct()
            .ToList();

        List<Guid> existingProjectIds = await _context.Projects
            .Where(project => projectIds.Contains(project.Id))
            .Select(project => project.Id)
            .ToListAsync();

        if (existingProjectIds.Count != projectIds.Count)
        {
            return ServiceResult.BadRequest("One or more projects do not exist.");
        }

        employee.FirstName = request.FirstName;
        employee.LastName = request.LastName;
        employee.Email = request.Email;
        employee.Status = request.Status;
        employee.HireDate = request.HireDate;
        employee.Notes = request.Notes;
        employee.DepartmentId = request.DepartmentId;

        employee.EmployeeProjects.Clear();

        foreach (Guid projectId in existingProjectIds)
        {
            employee.EmployeeProjects.Add(new EmployeeProject
            {
                EmployeeId = employee.Id,
                ProjectId = projectId
            });
        }

        await _context.SaveChangesAsync();

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> DeleteEmployeeAsync(Guid id)
    {
        Employee? employee = await _context.Employees
            .FirstOrDefaultAsync(employee => employee.Id == id);

        if (employee is null)
        {
            return ServiceResult.NotFound();
        }

        _context.Employees.Remove(employee);

        await _context.SaveChangesAsync();

        return ServiceResult.Success();
    }

    private async Task<Employee?> GetEmployeeWithDetailsAsync(Guid id, bool asNoTracking)
    {
        IQueryable<Employee> query = _context.Employees
            .Include(employee => employee.Department)
            .Include(employee => employee.EmployeeProjects)
                .ThenInclude(employeeProject => employeeProject.Project);

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(employee => employee.Id == id);
    }

    private static EmployeeResponse MapToResponse(Employee employee)
    {
        return new EmployeeResponse
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            Status = employee.Status,
            HireDate = employee.HireDate,
            Notes = employee.Notes,
            Department = employee.Department is null
                ? null
                : new DepartmentSummaryResponse
                {
                    Id = employee.Department.Id,
                    Name = employee.Department.Name
                },
            Projects = employee.EmployeeProjects
                .Where(employeeProject => employeeProject.Project is not null)
                .Select(employeeProject => new ProjectSummaryResponse
                {
                    Id = employeeProject.Project.Id,
                    Name = employeeProject.Project.Name
                })
                .ToList()
        };
    }
}