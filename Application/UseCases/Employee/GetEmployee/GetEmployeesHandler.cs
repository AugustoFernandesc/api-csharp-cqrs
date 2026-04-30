using MediatR;
using MinhaApiCQRS.Application.Interfaces;
using MinhaApiCQRS.Application.ViewModel;
using Shared.Pagination;
using EmployeeEntity = MinhaApiCQRS.Domain.Entities.Employee;

namespace MinhaApiCQRS.Application.UseCases.Employee.GetEmployee;

public class GetEmployeesHandler : IRequestHandler<GetEmployeesQuery, PagedResult<EmployeeDto>>
{
    private readonly IReadOnlyRepository<EmployeeDto> _readOnlyRepository;

    public GetEmployeesHandler(IReadOnlyRepository<EmployeeDto> readOnlyRepository)
    {
        _readOnlyRepository = readOnlyRepository;
    }

    public async Task<PagedResult<EmployeeDto>> Handle(GetEmployeesQuery request, CancellationToken cancellationToken)
    {
        string[] columns = { "Id", "Name", "Email", "Age" };

        return await _readOnlyRepository.PaginatedQueryAsync("employees", columns, request);
    }
}