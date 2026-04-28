using MediatR;
using MinhaApiCQRS.Application.Interfaces;
using MinhaApiCQRS.Application.Queries;
using MinhaApiCQRS.Domain.Exceptions;

namespace Application.UseCases.Employee.GetEmployeePdf;

public class GetEmployeePdfHandler : IRequestHandler<GetEmployeePdfQuery, byte[]>
{
    private readonly IUnitOfWork _uow;
    private readonly IPdfService _pdfService;

    public GetEmployeePdfHandler(IUnitOfWork uow, IPdfService pdfService)
    {
        _uow = uow;
        _pdfService = pdfService;
    }

    public async Task<byte[]> Handle(GetEmployeePdfQuery request, CancellationToken cancellationToken)
    {
        var employee = await _uow.EmployeeRepository.GetByIdAsync(request.Id);

        if (employee == null)
        {
            throw new EntityNotFoundException($"Funcionario com ID {request.Id} nao encontrado");
        }

        var pdfBytes = _pdfService.GenerateEmployeePdf(employee.Name, employee.Age, employee.Photo);
        return pdfBytes;
    }
}

