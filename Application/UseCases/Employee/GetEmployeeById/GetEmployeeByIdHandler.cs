using MinhaApiCQRS.Application.Interfaces;

public class GetEmployeeByIdHandler
{
    private readonly IEmployeeRepository _repository;

    public GetEmployeeByIdHandler(IEmployeeRepository repository)
    {
        _repository = repository;
    }

    public async Task<EmployeeDto> HandleAsync(Guid id)
    {
        var employee = await _repository.GetById(id);

        if (employee == null)
        {
            throw new Exception("Funcionário não encontrado");
        }

        return new EmployeeDto(
            employee.Id,
            employee.Name,
            employee.Email,
            employee.Age,
            employee.Photo
        );
    }
}