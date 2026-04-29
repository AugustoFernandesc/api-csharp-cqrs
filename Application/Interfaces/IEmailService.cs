namespace Application.Interfaces;

public interface IEmailService
{
    Task SendEmailWithAttachmentAsync(string toEmail, string subject, string body, byte[] attachment, string fileName);
}
