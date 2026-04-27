using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinhaApiCQRS.Domain.Entities;

namespace MinhaApiCQRS.Infrastructure.Configurations;

// IEntityTypeConfiguration separa as regras de banco da regra de negócio (Domain)
public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        // Define o nome da tabela no Postgres (evita nomes automáticos estranhos)
        builder.ToTable("employees");

        // Define a chave primária (O Guid que a gente gerou na Entidade)
        builder.HasKey(x => x.Id);

        // Configurações de coluna: nome obrigatório e tamanho máximo
        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        // Email é único? Poderíamos adicionar .HasIndex(x => x.Email).IsUnique()
        builder.Property(x => x.Email)
            .HasMaxLength(150)
            .IsRequired();

        // Idade é obrigatória
        builder.Property(x => x.Age)
            .IsRequired();
    }
}
