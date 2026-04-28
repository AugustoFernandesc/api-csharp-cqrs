using MinhaApiCQRS.Domain.Exceptions;

namespace MinhaApiCQRS.Domain.Entities;

public class Employee
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string Password { get; private set; } = null!;
    public int Age { get; private set; }

    //adicionados para a atv de envio de pdf 
    public string? Photo { get; private set; }
    public bool IsEmailSent { get; private set; }

    private Employee()
    {
    }

    public Employee(string name, string email, string password, int age, string? photo)
    {
        Validate(name, email, age, password);

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
        Validate(name, email, age);

        this.Name = name.Trim();
        this.Email = email.Trim();
        this.Age = age;
    }

    private static void Validate(string name, string email, int age, string? password = null)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length < 3)
        {
            throw new InvalidEmployeeException("Employee name must contain at least 3 characters.");
        }

        if (age < 0)
        {
            throw new InvalidEmployeeException("Employee age cannot be negative.");
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new InvalidEmployeeException("Employee email cannot be empty.");
        }

        if (password != null && string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidEmployeeException("Employee password cannot be empty.");
        }
    }

    // Método para o Scheduler usar depois de enviar o e-mail
    public void MarkAsEmailSent()
    {
        this.IsEmailSent = true;
    }
}
