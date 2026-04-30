using MinhaApiCQRS.Domain.Entities;
using Shared.Pagination;

namespace MinhaApiCQRS.Application.Interfaces;

public interface IEmployeeRepository : IGenericRepository<Employee>
{
    Task<Employee?> GetByEmailAsync(string email);
    Task<bool> IsEmailAllReadyInUse(string email, Guid? id = null);
}
