using MediatR;
using MinhaApiCQRS.Application.ViewModel;

namespace MinhaApiCQRS.Application.UseCases.Employee.GetEmployee;

public record GetEmployeesQuery() : IRequest<IReadOnlyList<EmployeeDto>>;
