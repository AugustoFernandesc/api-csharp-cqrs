using MinhaApiCQRS.Application.Interfaces;
using MinhaApiCQRS.Domain.Entities;

public class UpdateEmployeeHandler
{
    private readonly IEmployeeRepository _repository;
    private readonly IUnitOfWork _uow;

    public UpdateEmployeeHandler(IEmployeeRepository repository, IUnitOfWork uow)
    {
        _repository = repository;
        _uow = uow;
    }

    public async Task Handle(Guid Id, EmployeeDto dto)
    {
        var employee = await _repository.GetById(Id);
        if (employee == null)
        {
            throw new Exception("Funcionario nao encontrado para atualizacaoo.");
        }

        employee.Update(dto.Name, dto.Email, dto.Age);

        await _repository.Update(employee);
        await _uow.Commit();
    }

}