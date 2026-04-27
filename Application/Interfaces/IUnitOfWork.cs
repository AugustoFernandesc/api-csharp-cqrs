using MinhaApiCQRS.Application.Interfaces;

namespace MinhaApiCQRS.Application.Interfaces;

// O UNIT OF WORK É O "GERENTE DA TRANSAÇÃO":
// Ele junta várias operações no banco e só confirma tudo no final com Commit().
public interface IUnitOfWork : IDisposable
{
    // Repositório especializado de funcionário.
    IEmployeeRepository EmployeeRepository { get; }

    // Fábrica de repositórios genéricos para qualquer entidade.
    IGenericRepository<T> Repository<T>() where T : class;

    // Confirma no banco tudo o que foi preparado no contexto.
    Task<int> Commit();
}
