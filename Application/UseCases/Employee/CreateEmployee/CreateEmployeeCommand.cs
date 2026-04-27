using Microsoft.AspNetCore.Http;

namespace MinhaApiCQRS.Application.UseCases.CreateEmployee;

public record CreateEmployeeCommand(string Name, string Email, string Password, int Age, IFormFile? Photo);