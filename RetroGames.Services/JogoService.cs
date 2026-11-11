
using System.Transactions;
using RetroGames.Model.Model;
using RetroGames.Repository;

namespace RetroGames.Services;

public class JogoService : IJogoService
{
    private readonly IRepositorySet repositorio;

    public JogoService(IRepositorySet repositorio)
    {
        this.repositorio = repositorio;
    }

    // Lista os jogos com filtros e paginação
    public async Task<(List<JogoDetalheServiceModel> Jogos, int Total)>
        ListarAsync(
            string? busca = null,
            int? idPlataforma = null,
            int? idGenero = null,
            bool incluirInativos = false,
            int pagina = 1,
            int tamanhoPagina = 10)
    {
        IReadOnlyCollection<int>? idsJogos = null;

        // Se existe filtro por gênero, busca os IDs dos jogos
        if (idGenero.HasValue)
        {
            var vinculos = await repositorio.JogoGenero.ListarTodosAsync();

            var idsFiltrados = vinculos
                .Where(v => v.IdGenero == idGenero.Value)
                .Select(v => v.IdJogo)
                .Distinct()
                .ToArray();

            // Evita listar todos os jogos caso o filtro não encontre nenhum
            if (idsFiltrados.Length == 0)
            {
                return (new List<JogoDetalheServiceModel>(), 0);
            }

            idsJogos = idsFiltrados;
        }

        // Filtros restantes e paginação ficam no Repository
        var (jogos, total) = await repositorio.Jogo.ListarAsync(
            busca,
            idPlataforma,
            idsJogos,
            incluirInativos,
            pagina,
            tamanhoPagina
        );

        var detalhes = await MontarDetalhesAsync(jogos);

        return (detalhes, total);
    }

    // Busca um jogo pelo ID
    public async Task<JogoDetalheServiceModel> BuscarPorIdAsync(int id)
    {
        var jogo = await repositorio.Jogo.BuscarPorIdAsync(id);

        if (jogo is null)
        {
            throw new BusinessRuleException(
                BusinessErrors.JOGO_NAO_ENCONTRADO
            );
        }

        var detalhes = await MontarDetalhesAsync(new List<Jogo> { jogo });

        return detalhes[0];
    }

    // Insere jogo e seus gêneros em uma transação
    public async Task<Jogo> InserirAsync(
        Jogo jogo,
        IReadOnlyCollection<int> idsGeneros)
    {
        jogo.Nome = jogo.Nome.Trim();
        jogo.Ativo = true;
        jogo.DataCadastro = DateTime.UtcNow;

        var generosDistintos = idsGeneros.Distinct().ToArray();

        await ValidarReferenciasAsync(jogo, generosDistintos);

        var transactionOptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout
        };

        using (var transactionScope = new TransactionScope(
            TransactionScopeOption.Required,
            transactionOptions,
            TransactionScopeAsyncFlowOption.Enabled))
        {
            jogo.Id = await repositorio.Jogo.InserirAsync(jogo);

            await repositorio.JogoGenero.InserirAsync(
                jogo.Id,
                generosDistintos
            );

            transactionScope.Complete();
        }

        return jogo;
    }

    // Altera o jogo e substitui seus vínculos com gêneros
    public async Task AlterarAsync(
        Jogo jogo,
        IReadOnlyCollection<int> idsGeneros)
    {
        var existente = await repositorio.Jogo.BuscarPorIdAsync(jogo.Id);

        if (existente is null)
        {
            throw new BusinessRuleException(
                BusinessErrors.JOGO_NAO_ENCONTRADO
            );
        }

        jogo.Nome = jogo.Nome.Trim();

        // Preserva as informações que não devem mudar na edição
        jogo.Ativo = existente.Ativo;
        jogo.DataCadastro = existente.DataCadastro;

        var generosDistintos = idsGeneros.Distinct().ToArray();

        await ValidarReferenciasAsync(jogo, generosDistintos);

        var transactionOptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout
        };

        using (var transactionScope = new TransactionScope(
            TransactionScopeOption.Required,
            transactionOptions,
            TransactionScopeAsyncFlowOption.Enabled))
        {
            await repositorio.Jogo.AlterarAsync(jogo);

            await repositorio.JogoGenero.RemoverPorJogoAsync(jogo.Id);

            await repositorio.JogoGenero.InserirAsync(
                jogo.Id,
                generosDistintos
            );

            transactionScope.Complete();
        }
    }

    // Desativa o jogo sem excluí-lo fisicamente
    public async Task DesativarAsync(int id)
    {
        var jogo = await repositorio.Jogo.BuscarPorIdAsync(id);

        if (jogo is null)
        {
            throw new BusinessRuleException(
                BusinessErrors.JOGO_NAO_ENCONTRADO
            );
        }

        await repositorio.Jogo.DesativarAsync(id);
    }

    // Confere se plataforma, condição e gêneros existem
    private async Task ValidarReferenciasAsync(
        Jogo jogo,
        IReadOnlyCollection<int> idsGeneros)
    {
        var plataforma = await repositorio.Plataforma.BuscarPorIdAsync(
            jogo.IdPlataforma
        );

        if (plataforma is null)
        {
            throw new BusinessRuleException(
                BusinessErrors.PLATAFORMA_NAO_ENCONTRADA
            );
        }

        var condicoes = await repositorio.CondicaoJogo.ListarAsync();

        if (!condicoes.Any(c => c.Id == jogo.IdCondicaoJogo))
        {
            throw new BusinessRuleException(
                BusinessErrors.CONDICAO_NAO_ENCONTRADA
            );
        }

        var generos = await repositorio.Genero.ListarAsync();

        var idsExistentes = generos
            .Select(g => g.Id)
            .ToHashSet();

        if (idsGeneros.Count == 0 ||
            idsGeneros.Any(id => !idsExistentes.Contains(id)))
        {
            throw new BusinessRuleException(
                BusinessErrors.GENERO_NAO_ENCONTRADO
            );
        }
    }

    // Monta os detalhes de vários jogos usando LINQ
    private async Task<List<JogoDetalheServiceModel>> MontarDetalhesAsync(
        List<Jogo> jogos)
    {
        if (jogos.Count == 0)
        {
            return new List<JogoDetalheServiceModel>();
        }

        var idsJogos = jogos.Select(j => j.Id).ToArray();

        // Busca os dados necessários em consultas por conjunto
        var plataformas = await repositorio.Plataforma.ListarAsync();
        var condicoes = await repositorio.CondicaoJogo.ListarAsync();
        var generos = await repositorio.Genero.ListarAsync();
        var vinculos = await repositorio.JogoGenero.ListarPorJogosAsync(
            idsJogos
        );

        var plataformasPorId = plataformas.ToDictionary(p => p.Id);
        var condicoesPorId = condicoes.ToDictionary(c => c.Id);

        // Associa cada jogo à sua lista de gêneros
        var generosPorJogo = vinculos
            .Join(
                generos,
                vinculo => vinculo.IdGenero,
                genero => genero.Id,
                (vinculo, genero) => new
                {
                    vinculo.IdJogo,
                    Genero = genero
                }
            )
            .GroupBy(item => item.IdJogo)
            .ToDictionary(
                grupo => grupo.Key,
                grupo => grupo.Select(item => item.Genero).ToList()
            );

        // Constrói os objetos de detalhe
        return jogos.Select(jogo =>
        {
            plataformasPorId.TryGetValue(
                jogo.IdPlataforma,
                out var plataforma
            );

            condicoesPorId.TryGetValue(
                jogo.IdCondicaoJogo,
                out var condicao
            );

            generosPorJogo.TryGetValue(
                jogo.Id,
                out var generosDoJogo
            );

            return new JogoDetalheServiceModel
            {
                Jogo = jogo,
                NomePlataforma = plataforma?.Nome ?? string.Empty,
                DescricaoCondicao = condicao?.Descricao ?? string.Empty,
                Generos = generosDoJogo ?? new List<Genero>()
            };
        }).ToList();
    }
}
