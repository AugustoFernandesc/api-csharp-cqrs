using MediatR;
using MinhaApiCQRS.Application.Interfaces;

namespace MinhaApiCQRS.Application.UseCases.Employee.DeleteEmployee;

public class DeleteEmployeeHandler : IRequestHandler<DeleteEmployeeCommand>
{
    private readonly IUnitOfWork _uow;

    public DeleteEmployeeHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task Handle(DeleteEmployeeCommand command, CancellationToken cancellationToken)
    {
        var employee = await _uow.EmployeeRepository.GetByIdAsync(command.Id);
        if (employee == null)
        {
            throw new Exception("Funcionario nao encontrado");
        }

        _uow.EmployeeRepository.Delete(employee);
        await _uow.CommitAsync();
    }
}
