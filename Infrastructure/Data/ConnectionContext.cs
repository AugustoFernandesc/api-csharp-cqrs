using Microsoft.EntityFrameworkCore;
using MinhaApiCQRS.Domain.Entities;

namespace MinhaApiCQRS.Infrastructure.Data.ConnectionContext;

public class ConnectionContext : DbContext
{
    public ConnectionContext(DbContextOptions<ConnectionContext> options) : base(options)
    {
    }

    // A porta de entrada para a tabela de funcionarios no banco
    public DbSet<Employee> Employees { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Em vez de configurar cada tabela aqui e criar um arquivo de 1000 linhas,
        // este comando varre o projeto ("Assembly") e aplica todas as classes 
        // que implementam IEntityTypeConfiguration automaticamente.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ConnectionContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
