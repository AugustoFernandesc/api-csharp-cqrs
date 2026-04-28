using MinhaApiCQRS.Application.Interfaces;
using Photo.Services;
using EmployeeEntity = MinhaApiCQRS.Domain.Entities.Employee;

namespace MinhaApiCQRS.Application.UseCases.Employee.CreateEmployee;

public class CreateEmployeeHandler
{
    private readonly IPhotoService _photoService;
    private readonly IUnitOfWork _uow;

    public CreateEmployeeHandler(IPhotoService photoService, IUnitOfWork uow)
    {
        _photoService = photoService;
        _uow = uow;
    }

    public async Task<Guid> HandleAsync(CreateEmployeeCommand command)
    {

        if (await _uow.EmployeeRepository.IsEmailAllReadyInUse(command.Email))
        {
            throw new Exception("Ja existe um funcionario com este email.");
        }

        string? filePath = null;

        if (command.Photo != null)
        {
            filePath = await _photoService.UploadAsync(command.Photo);
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(command.Password);

        var employee = new EmployeeEntity(command.Name, command.Email, passwordHash, command.Age, filePath);
        await _uow.EmployeeRepository.AddAsync(employee);
        await _uow.CommitAsync();
        return employee.Id;
    }
}
