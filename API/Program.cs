using Microsoft.EntityFrameworkCore;
using MinhaApiCQRS.Infrastructure.Data;
using MinhaApiCQRS.Infrastructure.Repositories;
using MinhaApiCQRS.Application.Interfaces;
using MinhaApiCQRS.Application.UseCases.Employee.CreateEmployee;
using MinhaApiCQRS.Application.UseCases.Employee.DeleteEmployee;
using MinhaApiCQRS.Application.UseCases.Employee.GetEmployee;
using MinhaApiCQRS.Application.UseCases.Employee.GetEmployeeById;
using MinhaApiCQRS.Application.UseCases.Employee.GetEmployeePhoto;
using MinhaApiCQRS.Application.UseCases.Employee.UpdateEmployee;
using Photo.Services;
using MinhaApiCQRS.Domain.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<ConnectionContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IPhotoService, PhotoService>();

builder.Services.AddScoped<CreateEmployeeHandler>();
builder.Services.AddScoped<GetEmployeePhotoHandler>();
builder.Services.AddScoped<GetEmployeeByIdHandler>();
builder.Services.AddScoped<GetEmployeesHandler>();
builder.Services.AddScoped<UpdateEmployeeHandler>();
builder.Services.AddScoped<DeleteEmployeeHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
