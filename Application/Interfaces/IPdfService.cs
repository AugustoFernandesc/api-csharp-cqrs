namespace MinhaApiCQRS.Application.Interfaces;

public interface IPdfService
{
    byte[] GenerateEmployeePdf(string name, int age, string? photoPath);
}