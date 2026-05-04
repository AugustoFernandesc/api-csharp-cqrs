using Application.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;
using Email;

namespace MinhaApiCQRS.Email;


/// Servico responsavel por enviar e-mail com anexo.
/// Ele monta a mensagem, anexa o PDF em bytes, conecta no SMTP configurado e envia para o funcionario.

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;

    public EmailService(IOptions<EmailSettings> settings)
    {
        // Carrega as configuracoes de SMTP do appsettings/Options para nao deixar credenciais fixas no codigo.
        _settings = settings.Value;
    }

    public async Task SendEmailWithAttachmentAsync(string toEmail, string subject, string body, byte[] attachment, string fileName)
    {
        // Cria a mensagem MIME que sera enviada pelo SMTP.
        var message = new MimeMessage();

        // Define remetente, destinatario e assunto usando as configuracoes carregadas.
        message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = subject;

        // Monta o corpo do e-mail e adiciona o PDF recebido em bytes como anexo.
        var builder = new BodyBuilder { TextBody = body };
        builder.Attachments.Add(fileName, attachment);
        message.Body = builder.ToMessageBody();

        // SmtpClient abre conexao com o servidor de e-mail configurado.
        using var client = new SmtpClient();

        try
        {
            // Conecta, autentica e envia a mensagem para o funcionario.
            await client.ConnectAsync(_settings.SmtpServer, _settings.Port, true);
            await client.AuthenticateAsync(_settings.SenderEmail, _settings.AppPassword);
            await client.SendAsync(message);
        }
        catch (Exception ex)
        {
            // Registra o erro e relanca para o scheduler nao marcar como enviado quando falhar.
            Console.WriteLine($"Erro ao enviar email: {ex.Message}");
            throw;
        }
        finally
        {
            // Fecha a conexao SMTP mesmo quando ocorre erro no envio.
            await client.DisconnectAsync(true);
        }
    }
}
