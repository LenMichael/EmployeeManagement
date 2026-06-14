using EmployeeManagement.Api.DTOs.Employees;
using EmployeeManagement.Api.DTOs.Requests;
using FluentValidation;

namespace EmployeeManagement.Api.Validators.Employees;

public class CreateEmployeeRequestValidator : AbstractValidator<CreateEmployeeRequest>
{
    public CreateEmployeeRequestValidator()
    {
        RuleFor(employee => employee.FirstName)
            .NotEmpty()
            .WithMessage("First name is required.")
            .MaximumLength(100)
            .WithMessage("First name cannot exceed 100 characters.");

        RuleFor(employee => employee.LastName)
            .NotEmpty()
            .WithMessage("Last name is required.")
            .MaximumLength(100)
            .WithMessage("Last name cannot exceed 100 characters.");

        RuleFor(employee => employee.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Email format is invalid.")
            .MaximumLength(200)
            .WithMessage("Email cannot exceed 200 characters.");

        RuleFor(employee => employee.Status)
            .IsInEnum()
            .WithMessage("Employee status is invalid.");

        RuleFor(employee => employee.HireDate)
            .NotEmpty()
            .WithMessage("Hire date is required.");

        RuleFor(employee => employee.Notes)
            .MaximumLength(1000)
            .WithMessage("Notes cannot exceed 1000 characters.");

        RuleFor(employee => employee.DepartmentId)
            .NotEmpty()
            .WithMessage("Department id is required.");

        RuleFor(employee => employee.ProjectIds)
            .NotNull()
            .WithMessage("Project ids cannot be null.");

        RuleFor(employee => employee.ProjectIds)
            .Must(projectIds => projectIds.Distinct().Count() == projectIds.Count)
            .When(employee => employee.ProjectIds is not null)
            .WithMessage("Project ids must be unique.");

        RuleForEach(employee => employee.ProjectIds)
            .NotEmpty()
            .WithMessage("Project id cannot be empty.");
    }
}