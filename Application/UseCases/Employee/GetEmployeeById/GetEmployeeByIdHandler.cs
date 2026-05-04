using MediatR;
using MinhaApiCQRS.Application.Interfaces;
using MinhaApiCQRS.Application.ViewModel;

namespace MinhaApiCQRS.Application.UseCases.Employee.GetEmployeeById;


/// Handler responsavel por buscar um funcionario pelo Id.
/// Ele retorna um DTO com os dados usados no modal de edicao, na foto e no controle de envio.

public class GetEmployeeByIdHandler : IRequestHandler<GetEmployeeByIdQuery, EmployeeDto>
{
    private readonly IUnitOfWork _uow;

    public GetEmployeeByIdHandler(IUnitOfWork uow)
    {
        // UnitOfWork permite acessar o repositorio de funcionarios.
        _uow = uow;
    }

    public async Task<EmployeeDto> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
    {
        // Procura o funcionario no banco usando o Id recebido na rota.
        var employee = await _uow.EmployeeRepository.GetByIdAsync(request.Id);

        if (employee == null)
        {
            throw new Exception("Funcionário não encontrado");
        }

        // Converte a entidade em DTO para nao expor a entidade diretamente ao front.
        return new EmployeeDto(
            employee.Id,
            employee.Name,
            employee.Email,
            employee.Age,
            employee.Photo,
            employee.IsEmailSent
        );
    }
}
