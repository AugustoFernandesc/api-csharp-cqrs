using MediatR;
using MinhaApiCQRS.Application.Interfaces;
using Photo.Services;
using EmployeeEntity = MinhaApiCQRS.Domain.Entities.Employee;

namespace MinhaApiCQRS.Application.UseCases.Employee.CreateEmployee;


/// Handler responsavel por criar e salvar um novo funcionario.
/// Ele valida email duplicado, salva a foto no disco, gera hash da senha e persiste o funcionario no banco.

public class CreateEmployeeHandler : IRequestHandler<CreateEmployeeCommand, Guid>
{
    private readonly IPhotoService _photoService;
    private readonly IUnitOfWork _uow;

    public CreateEmployeeHandler(IPhotoService photoService, IUnitOfWork uow)
    {
        // Servico que grava o arquivo de foto e retorna o caminho salvo.
        _photoService = photoService;

        // UnitOfWork concentra os repositorios e confirma a transacao no banco.
        _uow = uow;
    }

    public async Task<Guid> Handle(CreateEmployeeCommand command, CancellationToken cancellationToken)
    {
        // Antes de criar, impede cadastro duplicado com o mesmo email.
        if (await _uow.EmployeeRepository.IsEmailAllReadyInUse(command.Email))
        {
            throw new Exception("Ja existe um funcionario com este email.");
        }

        // Comeca sem foto; se vier arquivo no formulario, o caminho sera preenchido abaixo.
        string? filePath = null;

        if (command.Photo != null)
        {
            // Salva a imagem na pasta Storage e guarda o caminho fisico para usar no PDF e na leitura da foto.
            filePath = await _photoService.UploadAsync(command.Photo);
        }

        // A senha nunca e salva pura; aqui ela vira hash antes de ir para o banco.
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(command.Password);

        // Cria a entidade de dominio ja validando nome, email, idade, senha e caminho da foto.
        var employee = new EmployeeEntity(command.Name, command.Email, passwordHash, command.Age, filePath);

        // Adiciona no repositorio e confirma a gravacao no banco.
        await _uow.EmployeeRepository.AddAsync(employee);
        await _uow.CommitAsync();

        // Retorna o Id para o controller responder HTTP 201 com o funcionario criado.
        return employee.Id;
    }
}
