using MediatR;
using MinhaApiCQRS.Application.ViewModel;
using Shared.Pagination;
using EmployeeEntity = MinhaApiCQRS.Domain.Entities.Employee;

namespace MinhaApiCQRS.Application.UseCases.Employee.GetEmployee;

public class GetEmployeesQuery : PaginationParams, IRequest<PagedResult<EmployeeDto>>
{
}