using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.Middleware;
using EmployeeManagement.Api.Filters;
using EmployeeManagement.Api.Validators.Employees;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<FluentValidationFilter>();

builder.Services.AddControllers(options =>
{
    options.Filters.AddService<FluentValidationFilter>();
});

builder.Services.AddValidatorsFromAssemblyContaining<CreateEmployeeRequestValidator>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();