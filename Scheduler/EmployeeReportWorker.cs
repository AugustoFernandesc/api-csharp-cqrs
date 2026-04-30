using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MinhaApiCQRS.Application.Interfaces;
using MinhaApiCQRS.Domain.Entities;

namespace Scheduler;

public class EmployeeReportWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<EmployeeReportWorker> _logger;

    public EmployeeReportWorker(IServiceScopeFactory scopeFactory, ILogger<EmployeeReportWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("O Robo de Relatórios foi iniciado");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Verificando banco de dados às: {time}", DateTimeOffset.Now);
                using (var scope = _scopeFactory.CreateScope())
                {
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    var pdfService = scope.ServiceProvider.GetRequiredService<IPdfService>();
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                    var employees = await unitOfWork.Repository<Employee>().GetAllAsync();

                    var targetEmployees = employees
                    .Where(e => !string.IsNullOrEmpty(e.Photo) && !e.IsEmailSent)
                    .ToList();


                    foreach (var employee in targetEmployees)
                    {

                        _logger.LogInformation("📄 Gerando e enviando para: {name}", employee.Name);

                        var pdfBytes = pdfService.GenerateEmployeePdf(employee.Name, employee.Age, employee.Photo);
                        await emailService.SendEmailWithAttachmentAsync(
                            employee.Email,
                            "Relatório Automatico de Funcionario",
                            $"Olá {employee.Name}, este é seu relatorio.",
                            pdfBytes,
                            "RelatorioFuncionario.pdf"
                            );

                        employee.MarkAsEmailSent();
                        unitOfWork.Repository<Employee>().Update(employee);
                        await unitOfWork.CommitAsync();

                        _logger.LogInformation(" Sucesso: {name} processado e marcado como enviado.", employee.Name);
                    }
                }
            }

            catch (Exception ex)
            {
                _logger.LogError("Erro no processamento do Scheduler: {error}", ex.Message);
            }

            await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
        }
    }
}

