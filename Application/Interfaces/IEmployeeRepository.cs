using MinhaApiCQRS.Domain.Entities;

namespace MinhaApiCQRS.Application.Interfaces;

public interface IEmployeeRepository : IGenericRepository<Employee>
{
    Task<Employee?> GetByEmailAsync(string email);
    Task<bool> IsEmailAllReadyInUse(string email, Guid? id = null);
}
