
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Configuration;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlataformaController : ControllerBase
{
    private readonly IPlataformaService _plataformaService;

    public PlataformaController(IPlataformaService plataformaService)
    {
        _plataformaService = plataformaService;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var plataformas = await _plataformaService.ListarAsync();

        var resposta = plataformas
            .Select(plataforma => plataforma.ParaResponse())
            .ToList();

        return Ok(new RetroGamesResponseApiModel<List<ListarPlataformaResponseApiModel>>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = resposta
        });
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<IActionResult> ListarPorId(int id)
    {
        var plataforma = await _plataformaService.BuscarPorIdAsync(id);

        return Ok(new RetroGamesResponseApiModel<ListarPlataformaResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = plataforma.ParaResponse()
        });
    }

    [Authorize(Policy = Politicas.Administrador)]
    [HttpPost]
public async Task<IActionResult> Inserir(
    [FromBody] InserirPlataformaRequestApiModel request)
{
    var criada = await _plataformaService.InserirAsync(
        request.ParaModel());

    var envelope =
        new RetroGamesResponseApiModel<ListarPlataformaResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = criada.ParaResponse()
        };

    return CreatedAtAction(
        nameof(ListarPorId),
        new { id = criada.Id },
        envelope);
}

[Authorize(Policy = Politicas.Administrador)]
[HttpPut("{id}")]
public async Task<IActionResult> Alterar(
    int id,
    [FromBody] AlterarPlataformaRequestApiModel request)
{
    await _plataformaService.AlterarAsync(request.ParaModel(id));

    var envelope = new RetroGamesResponseApiModel
    {
        ResponseCode = ResponseCodes.SUCESSO
    };

    return Ok(envelope);
}

[Authorize(Policy = Politicas.Administrador)]
[HttpDelete("{id}")]
public async Task<IActionResult> Excluir(int id)
{
    await _plataformaService.ExcluirAsync(id);

    var envelope = new RetroGamesResponseApiModel
    {
        ResponseCode = ResponseCodes.SUCESSO
    };

    return Ok(envelope);
}

}
