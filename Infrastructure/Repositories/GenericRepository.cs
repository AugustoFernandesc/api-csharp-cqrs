using Microsoft.EntityFrameworkCore;
using MinhaApiCQRS.Application.Interfaces;
using MinhaApiCQRS.Infrastructure.Data.ConnectionContext;

namespace MinhaApiCQRS.Infrastructure.Repositories.GenericRepository;

// IMPLEMENTAÇÃO DO REPOSITÓRIO GENÉRICO:
// Aqui moram as operações básicas de banco reaproveitáveis por qualquer entidade.
public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    // O contexto é o "canal aberto" com o banco via EF Core.
    protected readonly ConnectionContext _context;

    public GenericRepository(ConnectionContext context)
    {
        _context = context;
    }

    // Traz todos os registros da tabela referente ao tipo T.
    public async Task<IReadOnlyList<T>> GetAllAsync() => await _context.Set<T>().ToListAsync();

    // Procura pelo id usando o mecanismo do EF Core.
    public async Task<T?> GetById(Guid id) => await _context.Set<T>().FindAsync(id);

    // Adiciona a entidade ao contexto, mas ainda não grava no banco sem Commit().
    public async Task AddAsync(T entity) => await _context.Set<T>().AddAsync(entity);

    // Marca a entidade como modificada para o EF persistir depois.
    public async Task Update(T entity) => _context.Set<T>().Update(entity);

    public async Task Delete(Guid id)
    {
        // Primeiro localiza o registro no banco/contexto.
        var entity = await _context.Set<T>().FindAsync(id);

        // Só remove se realmente encontrou alguma coisa.
        if (entity != null)
        {
            _context.Set<T>().Remove(entity);
        }
    }
}
