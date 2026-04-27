
namespace MinhaApiCQRS.Application.UseCases.GetEmployee;

using MinhaApiCQRS.Application.Interfaces;
using MinhaApiCQRS.Domain.Entities;

public class GetEmployeeHandler
{
    private readonly IEmployeeRepository _repository;

    public GetEmployeeHandler(IEmployeeRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<Employee>> Handle()
    {
        return await _repository.GetAllAsync();
    }
}