
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class JogoRepository : RepositoryBase, IJogoRepository
{
    public JogoRepository(RepositoryConfig config)
        : base(config)
    {
    }

    private static Jogo LerJogo(MySqlConnector.MySqlDataReader leitor)
    {
        return new Jogo
        {
            Id = leitor.GetInt32("co_seq_jogo"),
            Nome = leitor.GetString("no_jogo"),
            Descricao = leitor.IsDBNull(
                leitor.GetOrdinal("ds_jogo"))
                ? null
                : leitor.GetString("ds_jogo"),
            AnoLancamento = leitor.GetInt32("nu_ano_lancamento"),
            Preco = leitor.GetDecimal("vl_preco"),
            Estoque = leitor.GetInt32("qt_estoque"),
            Ativo = leitor.GetBoolean("st_ativo"),
            DataCadastro = leitor.GetDateTime("dt_cadastro"),
            IdPlataforma = leitor.GetInt32("co_plataforma"),
            IdCondicaoJogo = leitor.GetInt32("co_condicao_jogo")
        };
    }

    public async Task<(List<Jogo> Jogos, int Total)> ListarAsync(
        string? busca = null,
        int? idPlataforma = null,
        IReadOnlyCollection<int>? ids = null,
        bool incluirInativos = false,
        int pagina = 1,
        int tamanhoPagina = 10)
    {
        var jogos = new List<Jogo>();

        if (pagina < 1 || tamanhoPagina < 1)
            throw new ArgumentOutOfRangeException(
                "A página e o tamanho devem ser positivos.");

        // Lista vazia significa nenhum jogo correspondente.
        if (ids is { Count: 0 })
            return (jogos, 0);

        var filtros = new List<string>();

        if (!incluirInativos)
            filtros.Add("st_ativo = 1");

        if (!string.IsNullOrWhiteSpace(busca))
            filtros.Add("no_jogo LIKE CONCAT('%', @busca, '%')");

        if (idPlataforma.HasValue)
            filtros.Add("co_plataforma = @idPlataforma");

        if (ids is { Count: > 0 })
        {
            var parametrosIds = ids
                .Select((id, indice) => $"@id{indice}");

            filtros.Add(
                $"co_seq_jogo IN ({string.Join(", ", parametrosIds)})");
        }

        var where = filtros.Count == 0
            ? ""
            : "WHERE " + string.Join(" AND ", filtros);

        var sqlContagem = $"""
            SELECT COUNT(*)
            FROM tb_jogo
            {where};
            """;

        var sqlListagem = $"""
            SELECT co_seq_jogo, no_jogo, ds_jogo,
                   nu_ano_lancamento, vl_preco,
                   qt_estoque, st_ativo, dt_cadastro,
                   co_plataforma, co_condicao_jogo
            FROM tb_jogo
            {where}
            ORDER BY no_jogo, co_seq_jogo
            LIMIT @limite OFFSET @deslocamento;
            """;

        void ConfigurarFiltros(MySqlConnector.MySqlCommand comando)
        {
            if (!string.IsNullOrWhiteSpace(busca))
                AdicionarParametro(comando, "@busca", busca);

            if (idPlataforma.HasValue)
                AdicionarParametro(
                    comando, "@idPlataforma", idPlataforma.Value);

            if (ids is { Count: > 0 })
            {
                int indice = 0;

                foreach (var id in ids)
                {
                    AdicionarParametro(comando, $"@id{indice}", id);
                    indice++;
                }
            }
        }

        await using var conexao = await CriarConexaoAsync();

        await using var comandoContagem =
            CriarComando(conexao, sqlContagem);

        ConfigurarFiltros(comandoContagem);

        var resultado = await comandoContagem.ExecuteScalarAsync();
        int total = Convert.ToInt32(resultado);

        await using var comandoListagem =
            CriarComando(conexao, sqlListagem);

        ConfigurarFiltros(comandoListagem);

        AdicionarParametro(
            comandoListagem, "@limite", tamanhoPagina);

        AdicionarParametro(
            comandoListagem, "@deslocamento",
            checked((pagina - 1) * tamanhoPagina));

        await using var leitor =
            await comandoListagem.ExecuteReaderAsync();

        while (await leitor.ReadAsync())
        {
            jogos.Add(LerJogo(leitor));
        }

        return (jogos, total);
    }

    public async Task<Jogo?> BuscarPorIdAsync(int id)
    {
        const string sql = """
            SELECT co_seq_jogo, no_jogo, ds_jogo,
                   nu_ano_lancamento, vl_preco,
                   qt_estoque, st_ativo, dt_cadastro,
                   co_plataforma, co_condicao_jogo
            FROM tb_jogo
            WHERE co_seq_jogo = @id;
            """;

        await using var conexao = await CriarConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@id", id);

        await using var leitor = await comando.ExecuteReaderAsync();

        if (await leitor.ReadAsync())
            return LerJogo(leitor);

        return null;
    }

    public async Task<int> InserirAsync(Jogo jogo)
    {
        const string sql = """
            INSERT INTO tb_jogo
                (no_jogo, ds_jogo, nu_ano_lancamento,
                 vl_preco, qt_estoque, st_ativo,
                 dt_cadastro, co_plataforma, co_condicao_jogo)
            VALUES
                (@nome, @descricao, @ano,
                 @preco, @estoque, @ativo,
                 @data, @plataforma, @condicao);
            """;

        await using var conexao = await CriarConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@nome", jogo.Nome);
        AdicionarParametro(comando, "@descricao", jogo.Descricao);
        AdicionarParametro(comando, "@ano", jogo.AnoLancamento);
        AdicionarParametro(comando, "@preco", jogo.Preco);
        AdicionarParametro(comando, "@estoque", jogo.Estoque);
        AdicionarParametro(comando, "@ativo", jogo.Ativo);
        AdicionarParametro(comando, "@data", jogo.DataCadastro);
        AdicionarParametro(comando, "@plataforma", jogo.IdPlataforma);
        AdicionarParametro(comando, "@condicao", jogo.IdCondicaoJogo);

        await ExecutarGravacaoAsync(comando);

        return checked((int)comando.LastInsertedId);
    }

    public async Task AlterarAsync(Jogo jogo)
    {
        const string sql = """
            UPDATE tb_jogo
            SET no_jogo = @nome,
                ds_jogo = @descricao,
                nu_ano_lancamento = @ano,
                vl_preco = @preco,
                qt_estoque = @estoque,
                st_ativo = @ativo,
                co_plataforma = @plataforma,
                co_condicao_jogo = @condicao
            WHERE co_seq_jogo = @id;
            """;

        await using var conexao = await CriarConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@id", jogo.Id);
        AdicionarParametro(comando, "@nome", jogo.Nome);
        AdicionarParametro(comando, "@descricao", jogo.Descricao);
        AdicionarParametro(comando, "@ano", jogo.AnoLancamento);
        AdicionarParametro(comando, "@preco", jogo.Preco);
        AdicionarParametro(comando, "@estoque", jogo.Estoque);
        AdicionarParametro(comando, "@ativo", jogo.Ativo);
        AdicionarParametro(comando, "@plataforma", jogo.IdPlataforma);
        AdicionarParametro(comando, "@condicao", jogo.IdCondicaoJogo);

        await ExecutarGravacaoAsync(comando);
    }

    public async Task DesativarAsync(int id)
    {
        const string sql = """
            UPDATE tb_jogo
            SET st_ativo = 0
            WHERE co_seq_jogo = @id;
            """;

        await using var conexao = await CriarConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@id", id);

        await ExecutarGravacaoAsync(comando);
    }
}
