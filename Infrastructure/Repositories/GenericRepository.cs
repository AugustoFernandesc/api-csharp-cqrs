using Microsoft.EntityFrameworkCore;
using MinhaApiCQRS.Application.Interfaces;
using MinhaApiCQRS.Infrastructure.Data;
using Shared.Pagination;

namespace MinhaApiCQRS.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly ConnectionContext _context;

    public GenericRepository(ConnectionContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<T>> GetAllAsync() =>
        await _context.Set<T>().ToListAsync();

    public async Task<T?> GetByIdAsync(Guid id) =>
        await _context.Set<T>().FindAsync(id);

    public async Task AddAsync(T entity) =>
        await _context.Set<T>().AddAsync(entity);

    public void Update(T entity) => _context.Set<T>().Update(entity);

    public void Delete(T entity) => _context.Set<T>().Remove(entity);

    public async Task<PagedResult<T>> PaginateQueryAsync(IQueryable<T> query, PaginationParams paginationParams)
    {
        var totalRecords = await query.CountAsync();

        var items = await query
        .Skip((paginationParams.Page - 1) * paginationParams.Limit)
        .Take(paginationParams.Limit)
        .ToListAsync();

        return new PagedResult<T>
        {
            Data = items,
            TotalRecords = totalRecords,
            CurrentPage = paginationParams.Page,
            PageSize = paginationParams.Limit
        };
    }
}
