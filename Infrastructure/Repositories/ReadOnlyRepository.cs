using Dapper;
using Microsoft.Extensions.Configuration;
using MinhaApiCQRS.Application.Interfaces;
using Npgsql;
using Shared.Pagination;
using System.Data;


namespace MinhaApiCQRS.Infrastructure.Repositories;


/// Repositorio generico somente leitura usado para listagens com Dapper.
/// Ele monta SQL de paginacao, filtro e ordenacao, executa a consulta e retorna um PagedResult.

public class ReadOnlyRepository<T> : IReadOnlyRepository<T> where T : class
{
    private readonly string _connectionString;

    public ReadOnlyRepository(IConfiguration configuration)
    {
        // Lê a string de conexao usada pelo Npgsql para abrir conexao com o PostgreSQL.
        _connectionString = configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("String de conexao 'DefaultConnection' nao encontrada");
    }

    public async Task<PagedResult<T>> PaginatedQueryAsync(string tableName, string[] columns, PaginationParams request)
    {
        // Monta o trecho WHERE usando os filtros recebidos da tela.
        var where = BuildWhereClause(request.Filters);

        // Monta o ORDER BY usando o campo e a direcao pedidos pelo front.
        var sort = BuildOrderByClause(request.OrderBy, request.OrderByDirection);

        // Mantem um espaco antes do WHERE para a contagem funcionar quando houver filtro.
        var countSql = $"SELECT COUNT(*) FROM \"{tableName}\" {where}";

        // Consulta principal com colunas selecionadas, filtro, ordenacao e paginacao.
        var itemSql = $"SELECT {BuildQueryColumns(columns)} FROM \"{tableName}\" {where} {sort} LIMIT @RecordsPerPage OFFSET (@CurrentPage - 1) * @RecordsPerPage";

        // Define valores padrao caso a tela envie limite ou pagina invalida.
        var records = request.Limit > 0 ? request.Limit : 10;
        var currentPage = request.Page > 0 ? request.Page : 1;

        return await ConnectionWrapper(async connection =>
        {
            // Conta quantos registros existem com o filtro atual para alimentar a paginacao do front.
            var totalItens = records > 0
            ? await connection.ExecuteScalarAsync<int>(countSql)
            : 1;

            // Busca a pagina atual de dados.
            var items = await connection.QueryAsync<T>(itemSql,
            new { RecordsPerPage = records, CurrentPage = currentPage });

            // Empacota os dados e metadados de paginacao.
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
        // Sem filtros, a consulta nao precisa de WHERE.
        if (filters == null || !filters.Any()) return "";

        // Converte cada filtro recebido em uma condicao SQL.
        var whereClause = string.Join(" AND ", filters.Select(filter =>
        {
            // Usa aspas porque as colunas foram criadas com nomes PascalCase no PostgreSQL.
            var fieldName = $"\"{filter.Column}\"";
            var value = filter.Value;

            // Strings com Equals/Contains recebem aspas simples para virar literal SQL.
            if (filter.DataType == DataType.String && filter.Operator != OperatorType.ILike)
            {
                value = $"'{value?.ToString()?.Replace("'", "''")}'";
            }

            // Datas chegam como string pela query; aqui sao convertidas para formato SQL.
            // if (filter.DataType == DataType.DataTime && DateTime.TryParse(value, out var dt))
            // {
            //     value = $"'{dt:yyyy-MM-dd HH:mm:ss}'";
            // }

            // ILike permite busca parcial sem diferenciar maiusculas/minusculas no PostgreSQL.
            if (filter.Operator == OperatorType.ILike)
            {
                value = $"'%{value?.ToString()?.Replace("'", "''")}%'";
            }

            // Traduz o enum do operador para o simbolo SQL correspondente.
            var fieldOperator = GetSqlOperatorString(filter.Operator);

            return $"{fieldName} {fieldOperator} {value}";
        }));

        return $"WHERE {whereClause}";
    }

    protected static string BuildOrderByClause(string? orderBy, string? orderDirection)
    {
        // Se a tela nao escolher ordenacao, ordena por Id para manter resultado previsivel.
        if (string.IsNullOrWhiteSpace(orderBy)) return "ORDER BY \"Id\" ASC";

        // Aceita apenas DESC explicitamente; qualquer outro valor vira ASC.
        var direction = orderDirection?.ToUpper() == "DESC" ? "DESC" : "ASC";
        return $"ORDER BY \"{orderBy}\" {direction}";
    }

    protected static string BuildQueryColumns(string[] columns)
    {
        // Sem lista de colunas, busca todas.
        if (columns == null || columns.Length == 0) return "*";

        // Coloca aspas nas colunas para respeitar os nomes PascalCase do banco.
        return string.Join(", ", columns.Select(c => $"\"{c}\""));
    }

    private static string GetSqlOperatorString(OperatorType op)
    {
        // Mapeia o enum usado pela aplicacao para operadores SQL.
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
        // Abre a conexao, executa a acao recebida e fecha a conexao ao sair do using.
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        return await action(connection);
    }

}
