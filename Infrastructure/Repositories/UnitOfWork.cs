using MinhaApiCQRS.Application.Interfaces;
using MinhaApiCQRS.Infrastructure.Data.ConnectionContext;
using MinhaApiCQRS.Infrastructure.Repositories.GenericRepository;

namespace MinhaApi.Infrastructure.Repositories;

// IMPLEMENTAÇÃO DO UNIT OF WORK:
// Ele centraliza o contexto e garante que todos os repositórios falem com a mesma transação.
public class UnitOfWork : IUnitOfWork
{
    private readonly ConnectionContext _context;
    private IEmployeeRepository? _employeeRepository;

    public UnitOfWork(ConnectionContext context)
    {
        _context = context;
    }

    // Cria o repositório de funcionário só quando ele for realmente usado.
    public IEmployeeRepository EmployeeRepository =>
        _employeeRepository ??= new EmployeeRepository(_context);

    public IGenericRepository<T> Repository<T>() where T : class
    {
        // Entrega um repositório genérico para qualquer entidade.
        return new GenericRepository<T>(_context);
    }

    // Commit é o "agora grava de verdade no banco".
    public async Task<int> Commit() => await _context.SaveChangesAsync();

    // Libera recursos do contexto ao final do ciclo.
    public void Dispose() => _context.Dispose();
}
