using MinhaApiCQRS.Domain.Exceptions;

namespace MinhaApiCQRS.Domain.Entities;


/// Entidade de dominio que representa um funcionario.
/// Ela guarda dados cadastrais, caminho da foto e o controle de envio do e-mail com PDF.

public class Employee
{
    // Identificador unico do funcionario.
    public Guid Id { get; private set; }

    // Dados basicos do funcionario.
    public string Name { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string Password { get; private set; } = null!;
    public int Age { get; private set; }

    // Campos usados pela atividade de relatorio: Photo guarda o caminho da imagem e IsEmailSent controla o envio.
    public string? Photo { get; private set; }
    public bool IsEmailSent { get; private set; }

    private Employee()
    {
        // Construtor privado usado pelo Entity Framework ao materializar a entidade.
    }

    public Employee(string name, string email, string password, int age, string? photo)
    {
        // Valida os dados de entrada antes de montar a entidade.
        Validate(name, email, age, password);
        ChangeEmail(email);

        // Inicializa os campos do novo funcionario.
        this.Id = Guid.NewGuid();
        this.Name = name.Trim();
        this.Email = email.Trim();
        this.Password = password;
        this.Age = age;
        this.Photo = photo;
        this.IsEmailSent = false;
    }

    public void Update(string name, string email, int age)
    {
        // Reaproveita as validacoes para manter a entidade consistente em atualizacoes.
        Validate(name, email, age);
        ChangeEmail(email);

        // Atualiza somente os campos que podem mudar nessa operacao.
        this.Name = name.Trim();
        this.Email = email.Trim();
        this.Age = age;
    }

    private static void Validate(string name, string email, int age, string? password = null)
    {
        // Nome precisa existir e ter pelo menos tres caracteres.
        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length < 3)
        {
            throw new InvalidEmployeeException("Employee name must contain at least 3 characters.");
        }

        // Idade nao pode ser negativa.
        if (age < 0)
        {
            throw new InvalidEmployeeException("Employee age cannot be negative.");
        }

        // Senha e obrigatoria na criacao, quando ela e enviada para validacao.
        if (password != null && string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidEmployeeException("Employee password cannot be empty.");
        }
    }

    public void ChangeEmail(string email)
    {
        // Email precisa existir.
        if (string.IsNullOrWhiteSpace(email))
            throw new InvalidEmployeeException("Employee email cannot be empty.");

        // Validacao simples para impedir e-mails sem @.
        if (!email.Contains("@"))
            throw new InvalidEmployeeException("Email inválido");

        // Salva o e-mail normalizado.
        Email = email;
    }

    // Chamado pelo Scheduler depois que o e-mail com PDF e enviado com sucesso.
    public void MarkAsEmailSent()
    {
        this.IsEmailSent = true;
    }
}
