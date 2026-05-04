using MediatR;
using MinhaApiCQRS.Application.Interfaces;
using Photo.Services;

namespace MinhaApiCQRS.Application.UseCases.Employee.GetEmployeePhoto;


/// Handler responsavel por buscar a foto de um funcionario e transforma-la em bytes.
/// Esses bytes sao devolvidos pelo controller para o navegador renderizar a imagem.

public class GetEmployeePhotoHandler : IRequestHandler<GetEmployeePhotoQuery, byte[]>
{
    private readonly IUnitOfWork _uow;
    private readonly IPhotoService _photoService;

    public GetEmployeePhotoHandler(IUnitOfWork uow, IPhotoService photoService)
    {
        // UnitOfWork busca o funcionario e o caminho da foto salvo no banco.
        _uow = uow;

        // PhotoService abre o arquivo fisico e transforma em array de bytes.
        _photoService = photoService;
    }

    public async Task<byte[]> Handle(GetEmployeePhotoQuery request, CancellationToken cancellationToken)
    {
        // Busca o funcionario para descobrir onde a foto dele esta salva.
        var employee = await _uow.EmployeeRepository.GetByIdAsync(request.Id);

        if (employee == null || string.IsNullOrEmpty(employee.Photo))
        {
            throw new Exception("Funcionário ou foto não encontrados.");
        }

        // Le a foto do disco e retorna os bytes para o controller.
        return _photoService.GetImageBytes(employee.Photo);
    }
}
