Employee Management API

A small ASP.NET Core Web API project created as part of a Junior Backend Developer technical assessment.

The application manages employees, departments, and projects using Entity Framework Core and SQL Server.

Features
ASP.NET Core Web API
.NET 8 LTS
Entity Framework Core
SQL Server / LocalDB
Fluent API entity configuration
EF Core migrations
Seed data
Swagger UI
CRUD endpoints for:
Employees
Departments
Projects
Search and pagination support
Many-to-many relationship between Employees and Projects
One-to-many relationship between Departments and Employees
DTOs for request and response models

Domain Model
Employee

An employee belongs to one department and can be assigned to many projects.

Main properties:

Id
FirstName
LastName
Email
Status
HireDate
Notes
DepartmentId
Department

A department can have many employees.

Main properties:

Id
Name
Description
Project

A project can have many employees.

Main properties:

Id
Name
Description
StartDate
EndDate
Relationships
One Department has many Employees
One Employee belongs to one Department
One Employee can belong to many Projects
One Project can have many Employees

The Employee-Project relationship is implemented using an explicit join entity: EmployeeProject.

Technologies Used
.NET 8
ASP.NET Core Web API
Entity Framework Core
SQL Server
Swagger / Swashbuckle
Prerequisites

Make sure the following are installed:

.NET 8 SDK
Visual Studio 2022 or another IDE
SQL Server LocalDB or SQL Server
EF Core tools

To check the installed .NET SDKs:

dotnet --list-sdks

To check LocalDB:

sqllocaldb info
Database Configuration

The default connection string is configured in:

EmployeeManagement.Api/appsettings.json

Example LocalDB connection string:

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=EmployeeManagementDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  }
}
How to Run

Clone the repository:

git clone https://github.com/LenMichael/EmployeeManagement.git
cd EmployeeManagement

Restore packages:

dotnet restore

Build the solution:

dotnet build

Apply database migrations:

dotnet ef database update --project EmployeeManagement.Api --startup-project EmployeeManagement.Api

Run the API:

dotnet run --project EmployeeManagement.Api

Open Swagger in the browser:

https://localhost:{port}/swagger

The exact port is displayed in the terminal when the application starts.

API Endpoints
Employees
GET     /api/employees
GET     /api/employees/{id}
POST    /api/employees
PUT     /api/employees/{id}
DELETE  /api/employees/{id}

Supported query parameters for employee search:

search
status
departmentId
pageNumber
pageSize

Example:

GET /api/employees?search=alice&pageNumber=1&pageSize=10
Departments
GET     /api/departments
GET     /api/departments/{id}
POST    /api/departments
PUT     /api/departments/{id}
DELETE  /api/departments/{id}

Supported query parameters for department search:

search
pageNumber
pageSize

Example:

GET /api/departments?search=engineering
Projects
GET     /api/projects
GET     /api/projects/{id}
POST    /api/projects
PUT     /api/projects/{id}
DELETE  /api/projects/{id}

Supported query parameters for project search:

search
employeeId
pageNumber
pageSize

Example:

GET /api/projects?search=apollo
Seed Data

The database is seeded with initial data for:

Departments
Employees
Projects
Employee-project assignments

Seed data is configured through EF Core HasData inside the entity configuration classes.

Validation and Error Handling

The API uses request DTOs with validation attributes such as:

Required
MaxLength
EmailAddress
Range

The API returns appropriate HTTP status codes, including:

200 OK
201 Created
204 No Content
400 Bad Request
404 Not Found
409 Conflict

Examples:

Creating an employee with an existing email returns 409 Conflict
Creating a project with an invalid employee ID returns 400 Bad Request
Requesting a missing resource returns 404 Not Found
Notes

This project uses DTOs instead of exposing EF Core entities directly through the API.

This helps keep the API contract separate from the database model and avoids issues such as circular references, over-posting, and exposing unnecessary internal data.
