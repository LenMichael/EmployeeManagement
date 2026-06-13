using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.DTOs.Common;
using EmployeeManagement.Api.DTOs.Departments;
using EmployeeManagement.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DepartmentsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<DepartmentResponse>>> GetDepartments(
        [FromQuery] DepartmentSearchRequest request)
    {
        int pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        int pageSize = request.PageSize < 1 ? 10 : Math.Min(request.PageSize, 100);

        IQueryable<Department> query = _context.Departments
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            string searchTerm = request.Search.Trim();

            query = query.Where(department =>
                department.Name.Contains(searchTerm) ||
                (department.Description != null && department.Description.Contains(searchTerm)));
        }

        int totalCount = await query.CountAsync();

        List<DepartmentResponse> departments = await query
            .OrderBy(department => department.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(department => new DepartmentResponse
            {
                Id = department.Id,
                Name = department.Name,
                Description = department.Description,
                EmployeeCount = department.Employees.Count
            })
            .ToListAsync();

        var response = new PagedResponse<DepartmentResponse>
        {
            Items = departments,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };

        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DepartmentResponse>> GetDepartmentById(Guid id)
    {
        DepartmentResponse? department = await _context.Departments
            .AsNoTracking()
            .Where(department => department.Id == id)
            .Select(department => new DepartmentResponse
            {
                Id = department.Id,
                Name = department.Name,
                Description = department.Description,
                EmployeeCount = department.Employees.Count
            })
            .FirstOrDefaultAsync();

        if (department is null)
        {
            return NotFound();
        }

        return Ok(department);
    }

    [HttpPost]
    public async Task<ActionResult<DepartmentResponse>> CreateDepartment(CreateDepartmentRequest request)
    {
        string departmentName = request.Name.Trim();

        bool nameAlreadyExists = await _context.Departments
            .AnyAsync(department => department.Name == departmentName);

        if (nameAlreadyExists)
        {
            return Conflict("Department name already exists.");
        }

        var department = new Department
        {
            Name = departmentName,
            Description = request.Description
        };

        _context.Departments.Add(department);

        await _context.SaveChangesAsync();

        var response = new DepartmentResponse
        {
            Id = department.Id,
            Name = department.Name,
            Description = department.Description,
            EmployeeCount = 0
        };

        return CreatedAtAction(
            nameof(GetDepartmentById),
            new { id = department.Id },
            response);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateDepartment(Guid id, UpdateDepartmentRequest request)
    {
        Department? department = await _context.Departments
            .FirstOrDefaultAsync(department => department.Id == id);

        if (department is null)
        {
            return NotFound();
        }

        string departmentName = request.Name.Trim();

        bool nameAlreadyExists = await _context.Departments
            .AnyAsync(departmentRecord =>
                departmentRecord.Id != id &&
                departmentRecord.Name == departmentName);

        if (nameAlreadyExists)
        {
            return Conflict("Department name already exists.");
        }

        department.Name = departmentName;
        department.Description = request.Description;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteDepartment(Guid id)
    {
        Department? department = await _context.Departments
            .FirstOrDefaultAsync(department => department.Id == id);

        if (department is null)
        {
            return NotFound();
        }

        bool hasEmployees = await _context.Employees
            .AnyAsync(employee => employee.DepartmentId == id);

        if (hasEmployees)
        {
            return BadRequest("Cannot delete department because it has assigned employees.");
        }

        _context.Departments.Remove(department);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}