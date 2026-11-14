
using Microsoft.AspNetCore.Diagnostics;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Middlewares;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is BusinessRuleException erroNegocio)
        {
            httpContext.Response.StatusCode =
                (int)erroNegocio.Erro.Status;

            await httpContext.Response.WriteAsJsonAsync(
                new RetroGamesResponseApiModel
                {
                    ResponseCode = erroNegocio.Erro.Code,
                    ErrorMessage = erroNegocio.Message
                },
                cancellationToken);

            return true;
        }

        _logger.LogError(exception,
            "Erro inesperado ao processar a requisição.");

        httpContext.Response.StatusCode = 500;

        await httpContext.Response.WriteAsJsonAsync(
            new RetroGamesResponseApiModel
            {
                ResponseCode = ResponseCodes.ERRO,
                ErrorMessage = "Ocorreu um erro interno no servidor."
            },
            cancellationToken);

        return true;
    }
}
