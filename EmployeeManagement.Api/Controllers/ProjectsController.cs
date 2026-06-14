using EmployeeManagement.Api.DTOs.Common;
using EmployeeManagement.Api.DTOs.Projects;
using EmployeeManagement.Api.Services.Common;
using EmployeeManagement.Api.Services.Projects;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<ProjectResponse>>> GetProjects(
        [FromQuery] ProjectSearchRequest request)
    {
        PagedResponse<ProjectResponse> response = await _projectService.GetProjectsAsync(request);

        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProjectResponse>> GetProjectById(Guid id)
    {
        ProjectResponse? project = await _projectService.GetProjectByIdAsync(id);

        return project is null
            ? NotFound()
            : Ok(project);
    }

    [HttpPost]
    public async Task<ActionResult<ProjectResponse>> CreateProject(CreateProjectRequest request)
    {
        ServiceResult<ProjectResponse> result = await _projectService.CreateProjectAsync(request);

        if (!result.Succeeded || result.Value is null)
        {
            return ToActionResult(result);
        }

        return CreatedAtAction(
            nameof(GetProjectById),
            new { id = result.Value.Id },
            result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateProject(Guid id, UpdateProjectRequest request)
    {
        ServiceResult result = await _projectService.UpdateProjectAsync(id, request);

        return result.Succeeded
            ? NoContent()
            : ToActionResult(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteProject(Guid id)
    {
        ServiceResult result = await _projectService.DeleteProjectAsync(id);

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