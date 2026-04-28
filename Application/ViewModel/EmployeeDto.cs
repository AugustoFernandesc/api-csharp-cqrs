namespace MinhaApiCQRS.Application.ViewModel;

public record EmployeeDto(Guid Id, string Name, string Email, int Age, string? Photo);
