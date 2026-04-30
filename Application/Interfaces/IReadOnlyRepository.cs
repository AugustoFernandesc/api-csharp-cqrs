using Shared.Pagination;

namespace MinhaApiCQRS.Application.Interfaces;

public interface IReadOnlyRepository<T> where T : class
{
    Task<PagedResult<T>> PaginatedQueryAsync(string tableName, string[] columns, PaginationParams request);
}
