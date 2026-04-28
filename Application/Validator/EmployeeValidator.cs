using FluentValidation;
using MinhaApiCQRS.Application.UseCases.Employee.CreateEmployee;

public class EmployeeValidator : AbstractValidator<CreateEmployeeCommand>
{
    public EmployeeValidator()
    {
        // Nome não pode vir vazio.
        RuleFor(x => x.Name)
        .NotEmpty()
        .WithMessage("Nome e obrigatorio");

        // E-mail precisa existir e seguir formato válido.
        RuleFor(x => x.Email)
        .NotEmpty()
        .WithMessage("Email e obrigatorio")
        .EmailAddress()
        .WithMessage("Email invalido! Cade o @ e o dominio?");

        // Senha precisa ter pelo menos 8 caracteres.
        RuleFor(x => x.Password)
        .MinimumLength(8)
        .WithMessage("Senha deve ter no minimo 8 caracteres")
        .When(x => !string.IsNullOrEmpty(x.Password)); // SÓ VALIDA SE O CARA DIGITAR ALGO

        // Exemplo de regra de negócio simples para idade aceitável.
        RuleFor(x => x.Age)
        .InclusiveBetween(18, 100)
        .WithMessage("Idade deve ser entre 18 e 100");
    }
}
