using MediatR;
using MinhaApiCQRS.Application.ViewModel;

namespace MinhaApiCQRS.Application.UseCases.Employee.GetEmployeeById;

public record GetEmployeeByIdQuery(Guid Id) : IRequest<EmployeeDto>;
