using MinhaApiCQRS.Application.Interfaces;

namespace MinhaApiCQRS.Application.UseCases.Employee.DeleteEmployee;

public class DeleteEmployeeHandler
{
    private readonly IUnitOfWork _uow;

    public DeleteEmployeeHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task HandleAsync(Guid id)
    {
        var employee = await _uow.EmployeeRepository.GetByIdAsync(id);
        if (employee == null)
        {
            throw new Exception("Funcionario nao encontrado");
        }

        _uow.EmployeeRepository.Delete(employee);
        await _uow.CommitAsync();
    }
}
