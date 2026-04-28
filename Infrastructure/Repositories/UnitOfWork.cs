using MinhaApiCQRS.Application.Interfaces;
using MinhaApiCQRS.Domain.Interfaces;
using MinhaApiCQRS.Infrastructure.Data;

namespace MinhaApiCQRS.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ConnectionContext _context;
    private IEmployeeRepository? _employeeRepository;

    public UnitOfWork(ConnectionContext context)
    {
        _context = context;
    }

    public IEmployeeRepository EmployeeRepository =>
        _employeeRepository ??= new EmployeeRepository(_context);

    public IGenericRepository<T> Repository<T>() where T : class =>
        new GenericRepository<T>(_context);

    public Task<int> CommitAsync() =>
        _context.SaveChangesAsync();

    public void Dispose() => _context.Dispose();
}
