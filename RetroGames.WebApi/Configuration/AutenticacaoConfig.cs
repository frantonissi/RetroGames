using Microsoft.AspNetCore.Authentication.Cookies;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Configuration;

public static class AutenticacaoConfig
{
    public static IServiceCollection AddAutenticacao(
        this IServiceCollection servicos, bool desenvolvimento)
    {
        servicos.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(opcoes =>
            {
                opcoes.Cookie.Name = "retrogames.sessao";
                opcoes.Cookie.HttpOnly = true;
                opcoes.Cookie.SameSite = SameSiteMode.Strict;
                opcoes.Cookie.SecurePolicy = desenvolvimento
                    ? CookieSecurePolicy.SameAsRequest
                    : CookieSecurePolicy.Always;
                opcoes.ExpireTimeSpan = TimeSpan.FromHours(2);
                opcoes.SlidingExpiration = true;

                opcoes.Events.OnRedirectToLogin = contexto => EscreverErroAsync(
                    contexto.HttpContext, StatusCodes.Status401Unauthorized, ResponseCodes.NAO_AUTENTICADO);
                opcoes.Events.OnRedirectToAccessDenied = contexto => EscreverErroAsync(
                    contexto.HttpContext, StatusCodes.Status403Forbidden, ResponseCodes.ACESSO_NEGADO);
            });

        return servicos;
    }

    private static Task EscreverErroAsync(
        HttpContext contexto, int status, (int Code, string DefaultMessage) erro)
    {
        contexto.Response.StatusCode = status;
        return contexto.Response.WriteAsJsonAsync(new RetroGamesResponseApiModel
        {
            ResponseCode = erro.Code,
            ErrorMessage = erro.DefaultMessage
        }, contexto.RequestAborted);
    }
}
