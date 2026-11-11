using RetroGames.Model.Model;
using RetroGames.Repository;

namespace RetroGames.Services;

public class CondicaoJogoService : ICondicaoJogoService
{
    private readonly IRepositorySet repositorio;

    public CondicaoJogoService(IRepositorySet repositorio)
    {
        this.repositorio = repositorio;
    }

    public Task<List<CondicaoJogo>> ListarAsync()
    {
        return repositorio.CondicaoJogo.ListarAsync();
    }
}