using EmployeeManagement.Api.DTOs.Common;
using EmployeeManagement.Api.DTOs.Employees;
using EmployeeManagement.Api.DTOs.Requests;
using EmployeeManagement.Api.DTOs.Responses;
using EmployeeManagement.Api.Services.Common;
using EmployeeManagement.Api.Services.Employees;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<EmployeeResponse>>> GetEmployees(
        [FromQuery] EmployeeSearchRequest request)
    {
        PagedResponse<EmployeeResponse> response = await _employeeService.GetEmployeesAsync(request);

        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EmployeeResponse>> GetEmployeeById(Guid id)
    {
        EmployeeResponse? employee = await _employeeService.GetEmployeeByIdAsync(id);

        return employee is null
            ? NotFound()
            : Ok(employee);
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeResponse>> CreateEmployee(CreateEmployeeRequest request)
    {
        ServiceResult<EmployeeResponse> result = await _employeeService.CreateEmployeeAsync(request);

        if (!result.Succeeded || result.Value is null)
        {
            return ToActionResult(result);
        }

        return CreatedAtAction(
            nameof(GetEmployeeById),
            new { id = result.Value.Id },
            result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateEmployee(Guid id, UpdateEmployeeRequest request)
    {
        ServiceResult result = await _employeeService.UpdateEmployeeAsync(id, request);

        return result.Succeeded
            ? NoContent()
            : ToActionResult(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteEmployee(Guid id)
    {
        ServiceResult result = await _employeeService.DeleteEmployeeAsync(id);

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