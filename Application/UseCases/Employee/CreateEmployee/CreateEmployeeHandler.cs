using MinhaApiCQRS.Application.Interfaces;
using MinhaApiCQRS.Domain.Entities;

namespace MinhaApiCQRS.Application.UseCases.CreateEmployee;

public class CreateEmployeeHandler
{
    private readonly IEmployeeRepository _repository;

    public CreateEmployeeHandler(IEmployeeRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(CreateEmployeeCommand command)
    {

        string? filePath = null;

        if (command.Photo != null)
        {

            if (!Directory.Exists("Storage"))
            {
                Directory.CreateDirectory("Storage");
            }

            filePath = Path.Combine("Storage", command.Photo.FileName);

            using (Stream fileStream = new FileStream(filePath, FileMode.Create))
            {
                await command.Photo.CopyToAsync(fileStream);
            }
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(command.Password);

        var employee = new Employee(command.Name, command.Email, passwordHash, command.Age, filePath);
        await _repository.AddAsync(employee);
        return employee.Id;
    }
}