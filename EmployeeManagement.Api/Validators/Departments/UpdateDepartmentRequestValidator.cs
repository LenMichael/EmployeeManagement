using EmployeeManagement.Api.DTOs.Departments;
using FluentValidation;

namespace EmployeeManagement.Api.Validators.Departments;

public class UpdateDepartmentRequestValidator : AbstractValidator<UpdateDepartmentRequest>
{
    public UpdateDepartmentRequestValidator()
    {
        RuleFor(department => department.Name)
            .NotEmpty()
            .WithMessage("Department name is required.")
            .MaximumLength(100)
            .WithMessage("Department name cannot exceed 100 characters.");

        RuleFor(department => department.Description)
            .MaximumLength(500)
            .WithMessage("Department description cannot exceed 500 characters.");
    }
}