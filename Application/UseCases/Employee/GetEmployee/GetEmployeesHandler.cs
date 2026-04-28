using MinhaApiCQRS.Application.Interfaces;
using MinhaApiCQRS.Application.ViewModel;
using EmployeeEntity = MinhaApiCQRS.Domain.Entities.Employee;

namespace MinhaApiCQRS.Application.UseCases.Employee.GetEmployee;

public class GetEmployeesHandler
{
    private readonly IUnitOfWork _uow;

    public GetEmployeesHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<IReadOnlyList<EmployeeDto>> HandleAsync()
    {
        var employees = await _uow.Repository<EmployeeEntity>().GetAllAsync();
        return employees
            .Select(employee => new EmployeeDto(employee.Id, employee.Name, employee.Email, employee.Age, employee.Photo))
            .ToList();
    }
}
