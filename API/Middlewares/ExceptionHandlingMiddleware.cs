using System.Net;
using MinhaApiCQRS.Domain.Exceptions;

namespace MinhaApiCQRS.API.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var code = HttpStatusCode.InternalServerError;
        var message = "Ocorreu um erro interno no servidor.";

        if (exception is EntityNotFoundException)
        {
            code = HttpStatusCode.NotFound;
            message = exception.Message;
        }
        else if (exception is InvalidEmployeeException || exception is ArgumentException)
        {
            code = HttpStatusCode.BadRequest;
            message = exception.Message;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;

        await context.Response.WriteAsJsonAsync(new { error = message });
    }
}