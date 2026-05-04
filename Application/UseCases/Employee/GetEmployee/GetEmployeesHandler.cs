using MediatR;
using MinhaApiCQRS.Application.Interfaces;
using MinhaApiCQRS.Application.ViewModel;
using Shared.Pagination;

namespace MinhaApiCQRS.Application.UseCases.Employee.GetEmployee;


/// Handler responsavel por listar funcionarios.
/// Ele busca os dados de forma paginada, filtrada e ordenada usando o repositorio somente leitura.

public class GetEmployeesHandler : IRequestHandler<GetEmployeesQuery, PagedResult<EmployeeDto>>
{
    private readonly IReadOnlyRepository<EmployeeDto> _readOnlyRepository;

    public GetEmployeesHandler(IReadOnlyRepository<EmployeeDto> readOnlyRepository)
    {
        // Repositorio com Dapper usado para consultas de listagem, sem alterar dados.
        _readOnlyRepository = readOnlyRepository;
    }

    public async Task<PagedResult<EmployeeDto>> Handle(GetEmployeesQuery request, CancellationToken cancellationToken)
    {
        // Inclui Photo para o front exibir a imagem e IsEmailSent para controlar quem ja recebeu o relatorio.
        string[] columns = { "Id", "Name", "Email", "Age", "Photo", "IsEmailSent" };

        // Executa a consulta paginada na tabela employees usando os filtros recebidos pela query string.
        return await _readOnlyRepository.PaginatedQueryAsync("employees", columns, request);
    }
}
