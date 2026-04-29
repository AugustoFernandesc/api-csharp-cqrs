using MinhaApiCQRS.Application.UseCases.Employee.GetEmployeePhoto;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MinhaApiCQRS.Application.Queries;
using MinhaApiCQRS.Application.UseCases.Employee.CreateEmployee;
using MinhaApiCQRS.Application.UseCases.Employee.DeleteEmployee;
using MinhaApiCQRS.Application.UseCases.Employee.GetEmployee;
using MinhaApiCQRS.Application.UseCases.Employee.GetEmployeeById;
using MinhaApiCQRS.Application.UseCases.Employee.UpdateEmployee;
using Application.Interfaces;
using MinhaApiCQRS.Application.Interfaces;

namespace MinhaApiCQRS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeeController : ControllerBase
{
    private readonly IMediator _mediator;
    // private readonly IPdfService _pdfService;
    // private readonly IEmailService _emailService;
    public EmployeeController(IMediator mediator)
    {
        _mediator = mediator;
        // _pdfService = pdfService;
        // _emailService = emailService;
    }

    // 1. CREATE
    [HttpPost]
    public async Task<IActionResult> Add([FromForm] CreateEmployeeCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = id }, id);
    }

    // 2. GET PHOTO
    [HttpGet("{id:guid}/photo")]
    public async Task<IActionResult> GetPhoto(Guid id)
    {
        var imageBytes = await _mediator.Send(new GetEmployeePhotoQuery(id));
        return File(imageBytes, "image/jpeg");
    }

    // 3. GET BY ID 
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var employee = await _mediator.Send(new GetEmployeeByIdQuery(id));
        return Ok(employee);
    }

    // 4. LIST ALL
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var employees = await _mediator.Send(new GetEmployeesQuery());
        return Ok(employees);
    }

    // 5. UPDATE
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromForm] UpdateEmployeeCommand command)
    {
        await _mediator.Send(command with { Id = id });
        return Ok("Atualizado com sucesso");
    }

    // 6. DELETE
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteEmployeeCommand(id));
        return NoContent();
    }

    [HttpGet("{id}/download-pdf")]
    public async Task<IActionResult> DownloadPdf(Guid id)
    {
        var pdfBytes = await _mediator.Send(new GetEmployeePdfQuery(id));

        return File(pdfBytes, "application/pdf", $"funcionario_{id}.pdf");
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
