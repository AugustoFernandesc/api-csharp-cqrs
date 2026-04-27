namespace MinhaApiCQRS.Domain.Entities;

public class Employee
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string Password { get; private set; }
    public int Age { get; private set; }

    private Employee()
    {
    }

    public Employee(string name, string email, string password, int age)
    {
        Validate(name, email, age, password);

        Id = Guid.NewGuid();
        Name = name.Trim();
        Email = email.Trim();
        Password = password.Trim();
        Age = age;
    }

    public void Update(string name, string email, int age)
    {
        Validate(name, email, age);

        Name = name.Trim();
        Email = email.Trim();
        Age = age;
    }

    private static void Validate(string name, string email, int age, string? password = null)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length < 3)
        {
            throw new ArgumentException("Employee name must contain at least 3 characters.");
        }

        if (age < 0)
        {
            throw new ArgumentException("Employee age cannot be negative.");
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Employee email cannot be empty.");
        }

        if (password != null && string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Employee password cannot be empty.");
        }
    }
}
