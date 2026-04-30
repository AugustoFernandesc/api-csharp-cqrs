using Dapper;
using Microsoft.Extensions.Configuration;
using MinhaApiCQRS.Application.Interfaces;
using Npgsql;
using Shared.Pagination;
using System.Data;


namespace MinhaApiCQRS.Infrastructure.Repositories;

public class ReadOnlyRepository<T> : IReadOnlyRepository<T> where T : class
{
    private readonly string _connectionString;

    public ReadOnlyRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("String de conexao 'DefaultConnection' nao encontrada");
    }

    public async Task<PagedResult<T>> PaginatedQueryAsync(string tableName, string[] columns, PaginationParams request)
    {
        var where = BuildWhereClause(request.Filters);
        var sort = BuildOrderByClause(request.OrderBy, request.OrderByDirection);

        var countSql = $"SELECT COUNT(*) FROM \"{tableName}\"{where}";

        var itemSql = $"SELECT {BuildQueryColumns(columns)} FROM \"{tableName}\" {where} {sort} OFFSET (@RecordsPerPage * (@CurrentPage - 1)) ROWS FETCH NEXT @RecordsPerPage ROWS ONLY";

        var records = request.Limit > 0 ? request.Limit : 10;
        var currentPage = request.Page > 0 ? request.Page : 1;

        return await ConnectionWrapper(async connection =>
        {
            var totalItens = records > 0
            ? await connection.ExecuteScalarAsync<int>(countSql)
            : 1;

            var items = await connection.QueryAsync<T>(itemSql,
            new { RecordsPerPage = records, CurrentPage = currentPage });

            return new PagedResult<T>
            {
                Data = items.ToList(),
                TotalRecords = totalItens,
                CurrentPage = currentPage,
                PageSize = records
            };
        });

    }

    protected static string BuildWhereClause(IEnumerable<PagedFilterRequest> filters)
    {
        if (filters == null || !filters.Any()) return "";
        var whereClause = string.Join(" AND ", filters.Select(filter =>
        {
            var fieldName = $"\"{filter.Column}\"";
            var value = filter.Value;

            if (filter.DataType == DataType.String && filter.Operator != OperatorType.ILike)
            {
                value = $"'{value?.ToString()?.Replace("'", "''")}'";
            }

            if (filter.DataType == DataType.DataTime && value is DateTime dt)
            {
                value = $"'{dt:yyyy-MM-dd HH:mm:ss}'";
            }

            if (filter.Operator == OperatorType.ILike)
            {
                value = $"'%{value?.ToString()?.Replace("'", "''")}%'";
            }

            var fieldOperator = GetSqlOperatorString(filter.Operator);

            return $"{fieldName} {fieldOperator} {value}";
        }));

        return $"WHERE {whereClause}";
    }

    protected static string BuildOrderByClause(string? orderBy, string? orderDirection)
    {
        if (string.IsNullOrWhiteSpace(orderBy)) return "ORDER BY \"id\" ASC";

        var direction = orderDirection?.ToUpper() == "DESC" ? "DESC" : "ASC";
        return $"ORDER BY \"{orderBy}\" {direction}";
    }

    protected static string BuildQueryColumns(string[] columns)
    {
        if (columns == null || columns.Length == 0) return "*";
        return string.Join(", ", columns.Select(c => $"\"{c}\""));
    }

    private static string GetSqlOperatorString(OperatorType op)
    {
        return op switch
        {
            OperatorType.Equals => "=",
            OperatorType.NotEquals => "<>",
            OperatorType.GreaterThan => ">",
            OperatorType.LessThan => "<",
            OperatorType.GreaterThanOrEqual => ">=",
            OperatorType.LessThanOrEqual => "<=",
            OperatorType.Contains => "LIKE",
            OperatorType.ILike => "ILIKE",
            _ => "="
        };
    }

    protected async Task<TResult> ConnectionWrapper<TResult>(Func<IDbConnection, Task<TResult>> action)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        return await action(connection);
    }

}
