using EmployeeManagement.Core.Entities;
using EmployeeManagement.Core.Enums;

namespace EmployeeManagement.Api.Data.Seed;

public static class SeedData
{
    public static readonly Guid EngineeringDepartmentId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid HumanResourcesDepartmentId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly Guid FinanceDepartmentId = Guid.Parse("33333333-3333-3333-3333-333333333333");

    public static readonly Guid AliceEmployeeId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    public static readonly Guid BobEmployeeId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
    public static readonly Guid MariaEmployeeId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");

    public static readonly Guid ApolloProjectId = Guid.Parse("44444444-4444-4444-4444-444444444444");
    public static readonly Guid HermesProjectId = Guid.Parse("55555555-5555-5555-5555-555555555555");
    public static readonly Guid AtlasProjectId = Guid.Parse("66666666-6666-6666-6666-666666666666");

    public static Department[] Departments =>
    [
        new Department
        {
            Id = EngineeringDepartmentId,
            Name = "Engineering",
            Description = "Software development and technical operations"
        },
        new Department
        {
            Id = HumanResourcesDepartmentId,
            Name = "Human Resources",
            Description = "Employee relations and recruitment"
        },
        new Department
        {
            Id = FinanceDepartmentId,
            Name = "Finance",
            Description = "Financial planning and accounting"
        }
    ];

    public static Employee[] Employees =>
    [
        new Employee
        {
            Id = AliceEmployeeId,
            FirstName = "Alice",
            LastName = "Smith",
            Email = "alice.smith@example.com",
            Status = EmployeeStatus.Active,
            HireDate = new DateTime(2022, 9, 1),
            Notes = "Senior backend developer",
            DepartmentId = EngineeringDepartmentId
        },
        new Employee
        {
            Id = BobEmployeeId,
            FirstName = "Bob",
            LastName = "Johnson",
            Email = "bob.johnson@example.com",
            Status = EmployeeStatus.Active,
            HireDate = new DateTime(2023, 1, 15),
            Notes = "HR specialist",
            DepartmentId = HumanResourcesDepartmentId
        },
        new Employee
        {
            Id = MariaEmployeeId,
            FirstName = "Maria",
            LastName = "Brown",
            Email = "maria.brown@example.com",
            Status = EmployeeStatus.Inactive,
            HireDate = new DateTime(2021, 5, 10),
            Notes = null,
            DepartmentId = FinanceDepartmentId
        }
    ];

    public static Project[] Projects =>
    [
        new Project
        {
            Id = ApolloProjectId,
            Name = "Apollo",
            Description = "Internal employee management platform",
            StartDate = new DateTime(2024, 1, 1),
            EndDate = null
        },
        new Project
        {
            Id = HermesProjectId,
            Name = "Hermes",
            Description = "Notification and communication system",
            StartDate = new DateTime(2024, 3, 1),
            EndDate = null
        },
        new Project
        {
            Id = AtlasProjectId,
            Name = "Atlas",
            Description = "Finance reporting dashboard",
            StartDate = new DateTime(2023, 6, 1),
            EndDate = new DateTime(2024, 6, 30)
        }
    ];

    public static EmployeeProject[] EmployeeProjects =>
    [
        new EmployeeProject
        {
            EmployeeId = AliceEmployeeId,
            ProjectId = ApolloProjectId
        },
        new EmployeeProject
        {
            EmployeeId = AliceEmployeeId,
            ProjectId = HermesProjectId
        },
        new EmployeeProject
        {
            EmployeeId = BobEmployeeId,
            ProjectId = HermesProjectId
        },
        new EmployeeProject
        {
            EmployeeId = MariaEmployeeId,
            ProjectId = AtlasProjectId
        }
    ];
}