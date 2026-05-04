using MediatR;
using MinhaApiCQRS.Application.Interfaces;
using MinhaApiCQRS.Application.Queries;
using MinhaApiCQRS.Domain.Exceptions;

namespace Application.UseCases.Employee.GetEmployeePdf;


/// Handler responsavel por gerar o PDF individual de um funcionario.
/// Ele busca o funcionario no banco e transforma nome, idade e foto em bytes de PDF.

public class GetEmployeePdfHandler : IRequestHandler<GetEmployeePdfQuery, byte[]>
{
    private readonly IUnitOfWork _uow;
    private readonly IPdfService _pdfService;

    public GetEmployeePdfHandler(IUnitOfWork uow, IPdfService pdfService)
    {
        // UnitOfWork busca os dados do funcionario.
        _uow = uow;

        // PdfService monta o documento e devolve o arquivo em bytes.
        _pdfService = pdfService;
    }

    public async Task<byte[]> Handle(GetEmployeePdfQuery request, CancellationToken cancellationToken)
    {
        // Busca o funcionario que tera o relatorio gerado.
        var employee = await _uow.EmployeeRepository.GetByIdAsync(request.Id);

        if (employee == null)
        {
            throw new EntityNotFoundException($"Funcionario com ID {request.Id} nao encontrado");
        }

        // Gera o PDF em memoria com nome, idade e foto do funcionario.
        var pdfBytes = _pdfService.GenerateEmployeePdf(employee.Name, employee.Age, employee.Photo);
        return pdfBytes;
    }
}
