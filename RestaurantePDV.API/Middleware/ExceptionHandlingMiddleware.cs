using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace RestaurantePDV.API.Middleware;

public class ExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");

            var (status, title) = ex switch
            {
                ArgumentException => (StatusCodes.Status404NotFound, "Recurso não encontrado"),
                InvalidOperationException => (StatusCodes.Status400BadRequest, "Requisição inválida"),
                _ => (StatusCodes.Status500InternalServerError, "Erro interno do servidor")
            };

            var problem = new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = ex.Message,
                Instance = context.Request.Path
            };

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = status;
            await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
        }
    }
}
