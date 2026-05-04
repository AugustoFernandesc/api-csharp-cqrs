using MediatR;
using MinhaApiCQRS.Application.Interfaces;

namespace MinhaApiCQRS.Application.UseCases.Employee.DeleteEmployee;

/// <summary>
/// Handler responsavel por excluir um funcionario.
/// Ele localiza a entidade pelo Id, remove do repositorio e confirma a exclusao no banco.
/// </summary>
public class DeleteEmployeeHandler : IRequestHandler<DeleteEmployeeCommand>
{
    private readonly IUnitOfWork _uow;

    public DeleteEmployeeHandler(IUnitOfWork uow)
    {
        // UnitOfWork fornece acesso ao repositorio e ao Commit.
        _uow = uow;
    }

    public async Task Handle(DeleteEmployeeCommand command, CancellationToken cancellationToken)
    {
        // Busca o funcionario antes de excluir para garantir que ele existe.
        var employee = await _uow.EmployeeRepository.GetByIdAsync(command.Id);
        if (employee == null)
        {
            throw new Exception("Funcionario nao encontrado");
        }

        // Remove a entidade do contexto e grava a exclusao.
        _uow.EmployeeRepository.Delete(employee);
        await _uow.CommitAsync();
    }
}
