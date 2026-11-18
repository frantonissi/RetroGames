using System.Security.Claims;
using RetroGames.Model.Constantes;
using RetroGames.Services;
using RetroGames.WebApi.Extensions;

namespace RetroGames.WebApi.Services;

public class UsuarioAtualService(IHttpContextAccessor contexto) : IUsuarioAtualService
{
    private ClaimsPrincipal? Usuario => contexto.HttpContext?.User;

    public bool EstaAutenticado => Usuario?.Identity?.IsAuthenticated == true;

    public int? Id => EstaAutenticado ? Usuario?.ObterId() : null;

    public bool EhAdministrador =>
        EstaAutenticado && Usuario?.IsInRole(Perfis.Administrador) == true;
}
