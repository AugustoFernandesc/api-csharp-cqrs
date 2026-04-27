namespace MinhaApiCQRS.Application.Interfaces;

// O REPOSITÓRIO GENÉRICO É A "CAIXA DE FERRAMENTAS PADRÃO" DO BANCO:
// Ele define operações comuns que qualquer entidade da aplicação pode reaproveitar.
public interface IGenericRepository<T> where T : class
{
    // Busca todos os registros daquele tipo.
    Task<IReadOnlyList<T>> GetAllAsync();

    // Busca um registro específico pelo identificador.
    Task<T?> GetById(Guid id);

    // Agenda a inclusão de uma nova entidade no contexto.
    Task AddAsync(T entity);

    // Marca uma entidade existente como alterada.
    Task Update(T entity);

    // Remove um registro pelo id, se ele existir.
    Task Delete(Guid id);
}
