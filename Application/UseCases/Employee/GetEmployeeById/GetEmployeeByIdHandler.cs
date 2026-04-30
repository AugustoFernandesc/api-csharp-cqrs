using MediatR;
using MinhaApiCQRS.Application.Interfaces;
using MinhaApiCQRS.Application.ViewModel;

namespace MinhaApiCQRS.Application.UseCases.Employee.GetEmployeeById;

public class GetEmployeeByIdHandler : IRequestHandler<GetEmployeeByIdQuery, EmployeeDto>
{
    private readonly IUnitOfWork _uow;

    public GetEmployeeByIdHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<EmployeeDto> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
    {
        var employee = await _uow.EmployeeRepository.GetByIdAsync(request.Id);

        if (employee == null)
        {
            throw new Exception("Funcionário não encontrado");
        }

        return new EmployeeDto(
            employee.Id,
            employee.Name,
            employee.Email,
            employee.Age
        );
    }
}
