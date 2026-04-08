using System.Net;
using Microondas.Api.Exceptions;
using Microondas.Api.Logging;
using Microondas.Api.Modelos;

namespace Microondas.Api.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IExceptionTextLogger logger)
    {
        try
        {
            await _next(context);
        }
        catch (RegraNegocioException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await context.Response.WriteAsJsonAsync(ApiResponse<string>.Falha(ex.Message));
        }
        catch (Exception ex)
        {
            await logger.LogAsync(ex, context);
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsJsonAsync(ApiResponse<string>.Falha("Erro interno não tratado."));
        }
    }
}