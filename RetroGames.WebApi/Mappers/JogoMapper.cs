
using RetroGames.Model.Model;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Mappers;

public static class JogoMapper
{
    // Converte para o modelo da listagem
    public static ListarJogoResponseApiModel ParaResponse(
        this JogoDetalheServiceModel detalhe)
    {
        var jogo = detalhe.Jogo;

        return new ListarJogoResponseApiModel
        {
            Id = jogo.Id,
            Nome = jogo.Nome,
            AnoLancamento = jogo.AnoLancamento,
            Preco = jogo.Preco,
            Estoque = jogo.Estoque,
            Ativo = jogo.Ativo,
            NomePlataforma = detalhe.NomePlataforma,
            DescricaoCondicao = detalhe.DescricaoCondicao,
            Generos = detalhe.Generos
                .Select(genero => genero.Nome)
                .ToList()
        };
    }

    // Converte para o modelo de detalhe
    public static ListarPorIdJogoResponseApiModel ParaDetalheResponse(
        this JogoDetalheServiceModel detalhe)
    {
        var jogo = detalhe.Jogo;

        return new ListarPorIdJogoResponseApiModel
        {
            Id = jogo.Id,
            Nome = jogo.Nome,
            Descricao = jogo.Descricao,
            AnoLancamento = jogo.AnoLancamento,
            Preco = jogo.Preco,
            Estoque = jogo.Estoque,
            Ativo = jogo.Ativo,
            IdPlataforma = jogo.IdPlataforma,
            NomePlataforma = detalhe.NomePlataforma,
            IdCondicaoJogo = jogo.IdCondicaoJogo,
            DescricaoCondicao = detalhe.DescricaoCondicao,
            Generos = detalhe.Generos
                .Select(genero => genero.ParaResponse())
                .ToList()
        };
    }

    public static Jogo ParaModel(
    this InserirJogoRequestApiModel request)
{
    return new Jogo
    {
        Nome = request.Nome,
        Descricao = string.IsNullOrWhiteSpace(request.Descricao)
            ? null : request.Descricao,
        AnoLancamento = request.AnoLancamento,
        Preco = request.Preco,
        Estoque = request.Estoque,
        IdPlataforma = request.IdPlataforma,
        IdCondicaoJogo = request.IdCondicaoJogo
    };
}

public static Jogo ParaModel(
    this AlterarJogoRequestApiModel request, int id)
{
    return new Jogo
    {
        Id = id,
        Nome = request.Nome,
        Descricao = string.IsNullOrWhiteSpace(request.Descricao)
            ? null : request.Descricao,
        AnoLancamento = request.AnoLancamento,
        Preco = request.Preco,
        Estoque = request.Estoque,
        IdPlataforma = request.IdPlataforma,
        IdCondicaoJogo = request.IdCondicaoJogo,
        Ativo = request.Ativo
    };
}

}
