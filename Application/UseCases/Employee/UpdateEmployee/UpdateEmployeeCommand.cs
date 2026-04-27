
namespace MinhaApiCQRS.Application.UseCases;


public record UpdateEmployeeCommand(Guid Id, EmployeeDto employeeDto);