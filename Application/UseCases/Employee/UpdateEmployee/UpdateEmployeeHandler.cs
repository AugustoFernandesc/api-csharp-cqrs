using MediatR;
using MinhaApiCQRS.Application.Interfaces;

namespace MinhaApiCQRS.Application.UseCases.Employee.UpdateEmployee;


/// Handler responsavel por atualizar dados basicos de um funcionario.
/// Ele busca a entidade, altera nome, email e idade, e salva a atualizacao no banco.

public class UpdateEmployeeHandler : IRequestHandler<UpdateEmployeeCommand>
{
    private readonly IUnitOfWork _uow;

    public UpdateEmployeeHandler(IUnitOfWork uow)
    {
        // UnitOfWork fornece o repositorio e confirma a transacao.
        _uow = uow;
    }

    public async Task Handle(UpdateEmployeeCommand command, CancellationToken cancellationToken)
    {
        // Busca o funcionario que sera alterado.
        var employee = await _uow.EmployeeRepository.GetByIdAsync(command.Id);
        if (employee == null)
        {
            throw new Exception("Funcionario nao encontrado para atualizacaoo.");
        }

        // Atualiza somente os campos permitidos nesse comando.
        employee.Update(command.Name, command.Email, command.Age);

        // Marca a entidade como atualizada e confirma no banco.
        _uow.EmployeeRepository.Update(employee);
        await _uow.CommitAsync();
    }
}
