using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MinhaApiCQRS.Application.Interfaces;
using MinhaApiCQRS.Domain.Entities;

namespace Scheduler;


/// Worker em segundo plano responsavel por enviar relatorios de funcionarios por e-mail.
/// A cada ciclo ele busca funcionarios com foto e ainda nao enviados, gera PDF, envia o e-mail e marca como enviado.

public class EmployeeReportWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<EmployeeReportWorker> _logger;

    public EmployeeReportWorker(IServiceScopeFactory scopeFactory, ILogger<EmployeeReportWorker> logger)
    {
        // ScopeFactory cria escopos para resolver servicos scoped dentro do BackgroundService.
        _scopeFactory = scopeFactory;

        // Logger registra o andamento do robo no console/log da aplicacao.
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Mensagem inicial para indicar que o scheduler foi iniciado junto com a API.
        _logger.LogInformation("O Robo de Relatórios foi iniciado");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Cada volta do loop verifica novamente o banco.
                _logger.LogInformation("Verificando banco de dados às: {time}", DateTimeOffset.Now);

                // Cria um escopo para resolver UnitOfWork, PDF e Email como servicos scoped.
                using (var scope = _scopeFactory.CreateScope())
                {
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    var pdfService = scope.ServiceProvider.GetRequiredService<IPdfService>();
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                    // Busca todos os funcionarios para selecionar apenas os que precisam receber relatorio.
                    var employees = await unitOfWork.Repository<Employee>().GetAllAsync();

                    // Envia somente para funcionarios com foto e que ainda nao foram marcados como enviados.
                    var targetEmployees = employees
                    .Where(e => !string.IsNullOrEmpty(e.Photo) && !e.IsEmailSent)
                    .ToList();


                    foreach (var employee in targetEmployees)
                    {
                        // Processa cada funcionario individualmente para gerar o PDF e enviar por e-mail.
                        _logger.LogInformation(" Gerando e enviando para: {name}", employee.Name);

                        // O PDF anexado leva os dados nome, idade e foto.
                        var pdfBytes = pdfService.GenerateEmployeePdf(employee.Name, employee.Age, employee.Photo);
                        await emailService.SendEmailWithAttachmentAsync(
                            employee.Email,
                            "Relatório Automatico de Funcionario",
                            $"Olá {employee.Name}, este é seu relatorio.",
                            pdfBytes,
                            "RelatorioFuncionario.pdf"
                            );

                        // Depois do envio, marca o funcionario para nao reenviar nos proximos ciclos.
                        employee.MarkAsEmailSent();
                        unitOfWork.Repository<Employee>().Update(employee);
                        await unitOfWork.CommitAsync();

                        _logger.LogInformation(" Sucesso: {name} processado e marcado como enviado.", employee.Name);
                    }
                }
            }

            catch (Exception ex)
            {
                // Se algum envio falhar, registra o erro e continua tentando nos proximos ciclos.
                _logger.LogError("Erro no processamento do Scheduler: {error}", ex.Message);
            }

            // Intervalo entre verificacoes do scheduler.
            await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
        }
    }
}
