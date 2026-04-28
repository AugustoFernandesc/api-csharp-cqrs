using MediatR;
using Microsoft.AspNetCore.Http;

namespace MinhaApiCQRS.Application.UseCases.Employee.CreateEmployee;

public record CreateEmployeeCommand(string Name, string Email, string Password, int Age, IFormFile? Photo) : IRequest<Guid>;
