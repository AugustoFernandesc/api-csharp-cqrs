using MediatR;
using MinhaApiCQRS.Application.Interfaces;

namespace MinhaApiCQRS.Application.UseCases.Employee.UpdateEmployee;

public class UpdateEmployeeHandler : IRequestHandler<UpdateEmployeeCommand>
{
    private readonly IUnitOfWork _uow;

    public UpdateEmployeeHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task Handle(UpdateEmployeeCommand command, CancellationToken cancellationToken)
    {
        var employee = await _uow.EmployeeRepository.GetByIdAsync(command.Id);
        if (employee == null)
        {
            throw new Exception("Funcionario nao encontrado para atualizacaoo.");
        }

        employee.Update(command.Name, command.Email, command.Age);

        _uow.EmployeeRepository.Update(employee);
        await _uow.CommitAsync();
    }
}
