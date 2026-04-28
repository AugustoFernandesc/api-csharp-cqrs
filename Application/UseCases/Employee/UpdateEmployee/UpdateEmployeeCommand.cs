namespace MinhaApiCQRS.Application.UseCases.Employee.UpdateEmployee;

public record UpdateEmployeeCommand(Guid Id, string Name, string Email, int Age);
