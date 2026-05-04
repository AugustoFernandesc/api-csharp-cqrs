using MediatR;
using MinhaApiCQRS.Application.ViewModel;
using Shared.Pagination;

namespace MinhaApiCQRS.Application.UseCases.Employee.GetEmployee;

public class GetEmployeesQuery : PaginationParams, IRequest<PagedResult<EmployeeDto>>
{
}