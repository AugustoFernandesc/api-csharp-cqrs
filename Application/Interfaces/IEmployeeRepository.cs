using MinhaApiCQRS.Domain.Entities;

namespace MinhaApiCQRS.Application.Interfaces;

public interface IEmployeeRepository : IGenericRepository<Employee>
{
    List<Employee> GetEmployeesPhoto();

}