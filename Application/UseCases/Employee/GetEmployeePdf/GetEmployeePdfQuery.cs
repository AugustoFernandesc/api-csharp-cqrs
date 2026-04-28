using MediatR;

namespace MinhaApiCQRS.Application.Queries;

// Uma Query que retorna um array de bytes (o PDF)
public record GetEmployeePdfQuery(Guid Id) : IRequest<byte[]>;