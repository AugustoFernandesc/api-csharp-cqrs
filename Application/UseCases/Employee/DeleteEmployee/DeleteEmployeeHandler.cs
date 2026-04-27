using MinhaApiCQRS.Application.Interfaces;

public class DeleteEmployeeHandler
{
    private readonly IEmployeeRepository _repository;
    private readonly IUnitOfWork _uow;

    public DeleteEmployeeHandler(IEmployeeRepository repository, IUnitOfWork uow)
    {
        _repository = repository;
        _uow = uow;
    }

    public async Task Handle(Guid Id)
    {
        var employee = await _repository.GetById(Id);
        if (employee == null)
        {
            throw new Exception("Funcionario nao encontrado");
        }

        await _repository.Delete(Id);
        await _uow.Commit();

    }
}