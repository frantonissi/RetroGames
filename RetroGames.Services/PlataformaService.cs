
using RetroGames.Model.Model;
using RetroGames.Repository;

namespace RetroGames.Services;

public class PlataformaService : IPlataformaService
{
    private readonly IRepositorySet repositorio;

    // Construtor: recebe o conjunto de repositórios
    public PlataformaService(IRepositorySet repositorio)
    {
        this.repositorio = repositorio;
    }

    // Lista todas as plataformas
    public Task<List<Plataforma>> ListarAsync()
    {
        return repositorio.Plataforma.ListarAsync();
    }

    // Busca uma plataforma pelo ID
    public async Task<Plataforma> BuscarPorIdAsync(int id)
    {
        var plataforma = await repositorio.Plataforma.BuscarPorIdAsync(id);

        if (plataforma is null)
        {
            throw new BusinessRuleException(
                BusinessErrors.PLATAFORMA_NAO_ENCONTRADA
            );
        }

        return plataforma;
    }

    // Insere uma nova plataforma
    public async Task<Plataforma> InserirAsync(Plataforma plataforma)
    {
        plataforma.Nome = plataforma.Nome.Trim();

        bool existe = await repositorio.Plataforma.ExisteComNomeAsync(
            plataforma.Nome
        );

        if (existe)
        {
            throw new BusinessRuleException(
                BusinessErrors.PLATAFORMA_NOME_DUPLICADO
            );
        }

        try
        {
            plataforma.Id = await repositorio.Plataforma.InserirAsync(
                plataforma
            );
        }
        catch (RegistroDuplicadoException)
        {
            throw new BusinessRuleException(
                BusinessErrors.PLATAFORMA_NOME_DUPLICADO
            );
        }

        return plataforma;
    }

    // Altera uma plataforma existente
    public async Task AlterarAsync(Plataforma plataforma)
    {
        var plataformaExistente = await repositorio.Plataforma.BuscarPorIdAsync(
            plataforma.Id
        );

        if (plataformaExistente is null)
        {
            throw new BusinessRuleException(
                BusinessErrors.PLATAFORMA_NAO_ENCONTRADA
            );
        }

        plataforma.Nome = plataforma.Nome.Trim();

        bool existe = await repositorio.Plataforma.ExisteComNomeAsync(
            plataforma.Nome,
            plataforma.Id
        );

        if (existe)
        {
            throw new BusinessRuleException(
                BusinessErrors.PLATAFORMA_NOME_DUPLICADO
            );
        }

        try
        {
            await repositorio.Plataforma.AlterarAsync(plataforma);
        }
        catch (RegistroDuplicadoException)
        {
            throw new BusinessRuleException(
                BusinessErrors.PLATAFORMA_NOME_DUPLICADO
            );
        }
    }

    // Exclui uma plataforma sem jogos vinculados
    public async Task ExcluirAsync(int id)
    {
        var plataforma = await repositorio.Plataforma.BuscarPorIdAsync(id);

        if (plataforma is null)
        {
            throw new BusinessRuleException(
                BusinessErrors.PLATAFORMA_NAO_ENCONTRADA
            );
        }

        bool possuiJogos = await repositorio.Plataforma.PossuiJogosAsync(id);

        if (possuiJogos)
        {
            throw new BusinessRuleException(
                BusinessErrors.PLATAFORMA_POSSUI_JOGOS
            );
        }

        await repositorio.Plataforma.ExcluirAsync(id);
    }
}
