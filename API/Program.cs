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
using Microsoft.OpenApi;
using MinhaApiCQRS.API.Middlewares;
using QuestPDF.Infrastructure;
using MinhaApiCQRS.PDF;
using Application.Interfaces;
using MinhaApiCQRS.Email;
using Scheduler;


QuestPDF.Settings.License = LicenseType.Community;
QuestPDF.Settings.EnableDebugging = true;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Minha API CQRS", Version = "v1" });
});

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
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<EmployeeRepository>();
builder.Services.AddHostedService<EmployeeReportWorker>();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(MinhaApiCQRS.Application.Queries.GetEmployeePdfQuery).Assembly);
});

builder.Services.AddScoped<IPdfService, PdfGenerator>();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();



app.Run();
