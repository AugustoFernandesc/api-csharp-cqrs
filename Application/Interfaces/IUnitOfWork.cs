using MinhaApiCQRS.Domain.Interfaces;

namespace MinhaApiCQRS.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IEmployeeRepository EmployeeRepository { get; }
    IGenericRepository<T> Repository<T>() where T : class;
    Task<int> CommitAsync();
}
