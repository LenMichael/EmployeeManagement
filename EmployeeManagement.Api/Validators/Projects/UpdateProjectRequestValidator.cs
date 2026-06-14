using EmployeeManagement.Api.DTOs.Projects;
using FluentValidation;

namespace EmployeeManagement.Api.Validators.Projects;

public class UpdateProjectRequestValidator : AbstractValidator<UpdateProjectRequest>
{
    public UpdateProjectRequestValidator()
    {
        RuleFor(project => project.Name)
            .NotEmpty()
            .WithMessage("Project name is required.")
            .MaximumLength(150)
            .WithMessage("Project name cannot exceed 150 characters.");

        RuleFor(project => project.Description)
            .MaximumLength(500)
            .WithMessage("Project description cannot exceed 500 characters.");

        RuleFor(project => project.StartDate)
            .NotEmpty()
            .WithMessage("Start date is required.");

        RuleFor(project => project.EndDate)
            .Must((project, endDate) =>
                !endDate.HasValue || endDate.Value.Date >= project.StartDate.Date)
            .WithMessage("End date cannot be earlier than start date.");

        RuleFor(project => project.EmployeeIds)
            .NotNull()
            .WithMessage("Employee ids cannot be null.");

        RuleFor(project => project.EmployeeIds)
            .Must(employeeIds => employeeIds.Distinct().Count() == employeeIds.Count)
            .When(project => project.EmployeeIds is not null)
            .WithMessage("Employee ids must be unique.");

        RuleForEach(project => project.EmployeeIds)
            .NotEmpty()
            .WithMessage("Employee id cannot be empty.");
    }
}