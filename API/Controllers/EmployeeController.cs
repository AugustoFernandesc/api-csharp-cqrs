using Application.UseCases.Employee.GetEmployeePhoto;
using Microsoft.AspNetCore.Mvc;
using MinhaApiCQRS.Application.UseCases.Employee.CreateEmployee;
using MinhaApiCQRS.Application.UseCases.Employee.DeleteEmployee;
using MinhaApiCQRS.Application.UseCases.Employee.GetEmployee;
using MinhaApiCQRS.Application.UseCases.Employee.GetEmployeeById;
using MinhaApiCQRS.Application.UseCases.Employee.GetEmployeePhoto;
using MinhaApiCQRS.Application.UseCases.Employee.UpdateEmployee;

namespace MinhaApiCQRS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeeController : ControllerBase
{

    // 1. CREATE
    [HttpPost]
    public async Task<IActionResult> Add(
        [FromForm] CreateEmployeeCommand command,
        [FromServices] CreateEmployeeHandler handler)
    {
        var id = await handler.HandleAsync(command);
        return CreatedAtAction(nameof(GetById), new { id = id }, id);
    }

    // 2. GET PHOTO
    [HttpGet("{id:guid}/photo")]
    public async Task<IActionResult> GetPhoto(
        [FromForm] GetEmployeePhotoQuery getEmployeePhotoQuery,
        [FromServices] GetEmployeePhotoHandler handler)
    {
        var imageBytes = await handler.HandleAsync(getEmployeePhotoQuery);
        return File(imageBytes, "image/jpeg");
    }

    // 3. GET BY ID 
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        [FromServices] GetEmployeeByIdHandler handler)
    {
        var employee = await handler.HandleAsync(id);
        return Ok(employee);
    }

    // 4. LIST ALL
    [HttpGet]
    public async Task<IActionResult> Get(
        [FromServices] GetEmployeesHandler handler)
    {
        var employees = await handler.HandleAsync();
        return Ok(employees);
    }

    // 5. UPDATE
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromForm] UpdateEmployeeCommand command,
        [FromServices] UpdateEmployeeHandler handler)
    {
        var commandWithId = command with { Id = id };
        await handler.HandleAsync(commandWithId);
        return Ok("Atualizado com sucesso");
    }

    // 6. DELETE
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        [FromServices] DeleteEmployeeHandler handler)
    {
        await handler.HandleAsync(id);
        return NoContent();
    }
}
