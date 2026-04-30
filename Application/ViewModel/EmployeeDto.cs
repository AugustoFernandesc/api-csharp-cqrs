namespace MinhaApiCQRS.Application.ViewModel;


public record EmployeeDto(
    Guid Id = default,
    string Name = "",
    string Email = "",
    int Age = 0
);