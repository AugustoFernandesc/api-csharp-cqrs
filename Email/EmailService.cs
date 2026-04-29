using Application.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;
using MinhaApiCQRS.Application.Interfaces;

namespace MinhaApiCQRS.Email;

public class EmailService : IEmailService
{


    public async Task SendEmailWithAttachmentAsync(string toEmail, string subject, string body, byte[] attachment, string fileName)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Ciclano de patente maior do Rh", "augustofc39@gmail.com"));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = subject;

        var builder = new BodyBuilder();
        builder.TextBody = body;

        builder.Attachments.Add(fileName, attachment);

        message.Body = builder.ToMessageBody();

        using var client = new SmtpClient();

        try
        {
            await client.ConnectAsync("smtp.gmail.com", 465, true);
            await client.AuthenticateAsync("augustofc39@gmail.com", "ykwb pheq ffzz gent");
            await client.SendAsync(message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao enviar email: {ex.Message}");
            throw;
        }
        finally
        {
            await client.DisconnectAsync(true);
        }
    }
}
