using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Extensions;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AutenticacaoController(IAutenticacaoService autenticacao) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestApiModel requisicao)
    {
        var usuario = await autenticacao.AutenticarAsync(requisicao.Email!, requisicao.Senha!);

        if (usuario is null)
        {
            return Unauthorized(new RetroGamesResponseApiModel
            {
                ResponseCode = ResponseCodes.CREDENCIAIS_INVALIDAS.Code,
                ErrorMessage = ResponseCodes.CREDENCIAIS_INVALIDAS.DefaultMessage
            });
        }

        var principal = usuario.ParaClaimsPrincipal();
        await HttpContext.SignInAsync(principal);
        return ResponderUsuario(principal);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return Ok(new RetroGamesResponseApiModel());
    }

    [Authorize]
    [HttpGet("eu")]
    public IActionResult Eu()
    {
        return ResponderUsuario(User);
    }

    private IActionResult ResponderUsuario(ClaimsPrincipal principal)
    {
        return Ok(new RetroGamesResponseApiModel<UsuarioLogadoResponseApiModel>
        {
            Data = new UsuarioLogadoResponseApiModel
            {
                Id = principal.ObterId()!.Value,
                Nome = principal.FindFirstValue(ClaimTypes.Name)!,
                Email = principal.FindFirstValue(ClaimTypes.Email)!,
                Perfil = principal.FindFirstValue(ClaimTypes.Role)!
            }
        });
    }
}
