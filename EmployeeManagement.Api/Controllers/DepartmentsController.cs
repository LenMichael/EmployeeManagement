using EmployeeManagement.Api.DTOs.Common;
using EmployeeManagement.Api.DTOs.Departments;
using EmployeeManagement.Api.Services.Common;
using EmployeeManagement.Api.Services.Departments;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentsController : ControllerBase
{
    private readonly IDepartmentService _departmentService;

    public DepartmentsController(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<DepartmentResponse>>> GetDepartments(
        [FromQuery] DepartmentSearchRequest request)
    {
        PagedResponse<DepartmentResponse> response = await _departmentService.GetDepartmentsAsync(request);

        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DepartmentResponse>> GetDepartmentById(Guid id)
    {
        DepartmentResponse? department = await _departmentService.GetDepartmentByIdAsync(id);

        return department is null
            ? NotFound()
            : Ok(department);
    }

    [HttpPost]
    public async Task<ActionResult<DepartmentResponse>> CreateDepartment(CreateDepartmentRequest request)
    {
        ServiceResult<DepartmentResponse> result = await _departmentService.CreateDepartmentAsync(request);

        if (!result.Succeeded || result.Value is null)
        {
            return ToActionResult(result);
        }

        return CreatedAtAction(
            nameof(GetDepartmentById),
            new { id = result.Value.Id },
            result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateDepartment(Guid id, UpdateDepartmentRequest request)
    {
        ServiceResult result = await _departmentService.UpdateDepartmentAsync(id, request);

        return result.Succeeded
            ? NoContent()
            : ToActionResult(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteDepartment(Guid id)
    {
        ServiceResult result = await _departmentService.DeleteDepartmentAsync(id);

        return result.Succeeded
            ? NoContent()
            : ToActionResult(result);
    }

    private ActionResult ToActionResult(ServiceResult result)
    {
        return result.Status switch
        {
            ServiceResultStatus.NotFound => NotFound(result.ErrorMessage),
            ServiceResultStatus.BadRequest => BadRequest(result.ErrorMessage),
            ServiceResultStatus.Conflict => Conflict(result.ErrorMessage),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }
}