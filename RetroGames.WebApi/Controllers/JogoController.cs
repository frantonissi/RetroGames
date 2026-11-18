
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Configuration;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JogoController : ControllerBase
{
    private readonly IJogoService _jogoService;

    public JogoController(IJogoService jogoService)
    {
        _jogoService = jogoService;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] ListarJogoRequestApiModel request)
    {
        var (jogos, total) = await _jogoService.ListarAsync(
            busca: request.Busca,
            idPlataforma: request.IdPlataforma,
            idGenero: request.IdGenero,
            incluirInativos: request.IncluirInativos,
            pagina: request.Pagina,
            tamanhoPagina: request.TamanhoPagina
        );

        var itens = jogos
            .Select(jogo => jogo.ParaResponse())
            .ToList();

        var pagina = new PaginaResponseApiModel<ListarJogoResponseApiModel>
        {
            Itens = itens,
            Pagina = request.Pagina,
            TamanhoPagina = request.TamanhoPagina,
            TotalItens = total,
            TotalPaginas = (int)Math.Ceiling(
                (double)total / request.TamanhoPagina)
        };

        return Ok(new RetroGamesResponseApiModel<PaginaResponseApiModel<ListarJogoResponseApiModel>>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = pagina
        });
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<IActionResult> ListarPorId(int id)
    {
        var jogo = await _jogoService.BuscarPorIdAsync(id);

        return Ok(new RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = jogo.ParaDetalheResponse()
        });
    }

    [Authorize(Policy = Politicas.Administrador)]
    [HttpPost]
public async Task<IActionResult> Inserir(
    [FromBody] InserirJogoRequestApiModel request)
{
    var criado = await _jogoService.InserirAsync(
        request.ParaModel(),
        request.IdsGeneros);

    var detalhe = await _jogoService.BuscarPorIdAsync(criado.Id);

    var envelope =
        new RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = detalhe.ParaDetalheResponse()
        };

    return CreatedAtAction(
        nameof(ListarPorId),
        new { id = criado.Id },
        envelope);
}

    [Authorize(Policy = Politicas.Administrador)]
    [HttpPut("{id}")]
public async Task<IActionResult> Alterar(
    int id,
    [FromBody] AlterarJogoRequestApiModel request)
{
    await _jogoService.AlterarAsync(
        request.ParaModel(id),
        request.IdsGeneros);

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
    await _jogoService.DesativarAsync(id);

    var envelope = new RetroGamesResponseApiModel
    {
        ResponseCode = ResponseCodes.SUCESSO
    };

    return Ok(envelope);
}

}
