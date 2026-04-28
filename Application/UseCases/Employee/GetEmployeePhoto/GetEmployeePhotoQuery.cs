using MediatR;

namespace MinhaApiCQRS.Application.UseCases.Employee.GetEmployeePhoto;

public record GetEmployeePhotoQuery(Guid Id) : IRequest<byte[]>;
