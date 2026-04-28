using MediatR;
using MinhaApiCQRS.Application.Interfaces;
using Photo.Services;

namespace MinhaApiCQRS.Application.UseCases.Employee.GetEmployeePhoto;

public class GetEmployeePhotoHandler : IRequestHandler<GetEmployeePhotoQuery, byte[]>
{
    private readonly IUnitOfWork _uow;
    private readonly IPhotoService _photoService;

    public GetEmployeePhotoHandler(IUnitOfWork uow, IPhotoService photoService)
    {
        _uow = uow;
        _photoService = photoService;
    }

    public async Task<byte[]> Handle(GetEmployeePhotoQuery request, CancellationToken cancellationToken)
    {
        var employee = await _uow.EmployeeRepository.GetByIdAsync(request.Id);

        if (employee == null || string.IsNullOrEmpty(employee.Photo))
        {
            throw new Exception("Funcionário ou foto não encontrados.");
        }

        return _photoService.GetImageBytes(employee.Photo);
    }
}
