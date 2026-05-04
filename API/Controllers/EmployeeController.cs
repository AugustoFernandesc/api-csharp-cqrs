using MinhaApiCQRS.Application.UseCases.Employee.GetEmployeePhoto;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MinhaApiCQRS.Application.Queries;
using MinhaApiCQRS.Application.UseCases.Employee.CreateEmployee;
using MinhaApiCQRS.Application.UseCases.Employee.DeleteEmployee;
using MinhaApiCQRS.Application.UseCases.Employee.GetEmployee;
using MinhaApiCQRS.Application.UseCases.Employee.GetEmployeeById;
using MinhaApiCQRS.Application.UseCases.Employee.UpdateEmployee;
using MinhaApiCQRS.Application.ViewModel;

namespace MinhaApiCQRS.API.Controllers;

[ApiController]
[Route("api/[controller]")]

/// Controller responsavel por expor as rotas HTTP de funcionario.
/// Ele cria, lista, busca, atualiza, exclui, retorna foto em bytes e permite baixar o PDF do funcionario.

public class EmployeeController : ControllerBase
{
    private readonly IMediator _mediator;

    public EmployeeController(IMediator mediator)
    {
        // O mediator encaminha cada requisicao para o handler correto da camada Application.
        _mediator = mediator;
    }

    // Cria um funcionario novo. Usa FromForm porque o cadastro pode receber arquivo de foto.
    [HttpPost]
    public async Task<IActionResult> Add([FromForm] CreateEmployeeCommand command)
    {
        // Envia o comando para o handler salvar o funcionario e retornar o Id criado.
        var id = await _mediator.Send(command);

        // Retorna HTTP 201 apontando para a rota de consulta por Id.
        return CreatedAtAction(nameof(GetById), new { id = id }, id);
    }

    // Retorna a foto do funcionario como bytes para o navegador conseguir renderizar no <img>.
    [HttpGet("{id:guid}/photo")]
    public async Task<IActionResult> GetPhoto(Guid id)
    {
        // O handler localiza o funcionario, le o arquivo salvo no disco e devolve os bytes da imagem.
        var imageBytes = await _mediator.Send(new GetEmployeePhotoQuery(id));
        return File(imageBytes, "image/jpeg");
    }

    // Busca um funcionario especifico pelo Id.
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        // Busca os dados no banco usando a camada Application.
        var employee = await _mediator.Send(new GetEmployeeByIdQuery(id));

        // Troca o caminho fisico salvo no banco por uma URL que o navegador consegue abrir.
        return Ok(WithPhotoUrl(employee));
    }

    // Lista funcionarios com paginacao, ordenacao e filtros.
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetEmployeesQuery query)
    {
        // Executa a consulta paginada usando os parametros recebidos na query string.
        var result = await _mediator.Send(query);

        // A listagem tambem precisa da URL da foto para o <img src> funcionar no front.
        result.Data = result.Data.Select(WithPhotoUrl).ToList();
        return Ok(result);
    }

    // Atualiza os dados basicos do funcionario.
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromForm] UpdateEmployeeCommand command)
    {
        // O Id vem pela URL; o "with" cria uma copia do comando colocando esse Id no objeto enviado ao handler.
        await _mediator.Send(command with { Id = id });
        return Ok("Atualizado com sucesso");
    }

    // Exclui um funcionario pelo Id.
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        // O handler procura o funcionario e remove do banco.
        await _mediator.Send(new DeleteEmployeeCommand(id));
        return NoContent();
    }

    // Gera e baixa o PDF individual do funcionario pela lista do front.
    [HttpGet("{id}/download-pdf")]
    public async Task<IActionResult> DownloadPdf(Guid id)
    {
        // O handler transforma os dados do funcionario em um arquivo PDF em memoria.
        var pdfBytes = await _mediator.Send(new GetEmployeePdfQuery(id));

        // Retorna o arquivo como download para o navegador.
        return File(pdfBytes, "application/pdf", $"funcionario_{id}.pdf");
    }

    // Converte o campo Photo do DTO em uma URL publica da propria API.
    private EmployeeDto WithPhotoUrl(EmployeeDto employee)
    {
        // Se o funcionario nao tem foto, mantem o DTO como veio do banco.
        if (string.IsNullOrWhiteSpace(employee.Photo))
        {
            return employee;
        }

        // Mantem o arquivo protegido no servidor e expoe somente a rota de leitura da foto.
        var photoPath = Url.Action(nameof(GetPhoto), new { id = employee.Id });
        return employee with { Photo = $"{Request.Scheme}://{Request.Host}{photoPath}" };
    }

}

//     //teste de envio de email
//     [HttpPost("{id}/send-email")]
//     public async Task<IActionResult> SendMail(Guid id)
//     {
//         var employee = await _mediator.Send(new GetEmployeeByIdQuery(id));
//         var pdfBytes = _pdfService.GenerateEmployeePdf(employee.Name, employee.Age, employee.Photo);
//         await _emailService.SendEmailWithAttachmentAsync(employee.Email, "Relatório de Teste MediatR", $"Olá {employee.Name}, este é um teste do sistema de automação.", pdfBytes, "RelatorioFuncionario.pdf");
//         return Ok($"E-mail enviado com sucesso para {employee.Email}");

//     }
// 
