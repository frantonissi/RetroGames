
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class PlataformaRepository : RepositoryBase, IPlataformaRepository
{
    public PlataformaRepository(RepositoryConfig config)
        : base(config)
    {
    }

    public async Task<List<Plataforma>> ListarAsync()
    {
        var plataformas = new List<Plataforma>();

        const string sql = """
            SELECT co_seq_plataforma, no_plataforma,
                   no_fabricante, nu_ano_lancamento
            FROM tb_plataforma
            ORDER BY no_plataforma;
            """;

        await using var conexao = await CriarConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        await using var leitor = await comando.ExecuteReaderAsync();

        while (await leitor.ReadAsync())
        {
            var plataforma = new Plataforma
            {
                Id = leitor.GetInt32(0),
                Nome = leitor.GetString(1),
                Fabricante = leitor.GetString(2),
                AnoLancamento = leitor.GetInt32(3)
            };

            plataformas.Add(plataforma);
        }

        return plataformas;
    }

    public async Task<Plataforma?> BuscarPorIdAsync(int id)
    {
        const string sql = """
            SELECT co_seq_plataforma, no_plataforma,
                   no_fabricante, nu_ano_lancamento
            FROM tb_plataforma
            WHERE co_seq_plataforma = @id;
            """;

        await using var conexao = await CriarConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@id", id);

        await using var leitor = await comando.ExecuteReaderAsync();

        if (await leitor.ReadAsync())
        {
            return new Plataforma
            {
                Id = leitor.GetInt32(0),
                Nome = leitor.GetString(1),
                Fabricante = leitor.GetString(2),
                AnoLancamento = leitor.GetInt32(3)
            };
        }

        return null;
    }

    public async Task<bool> ExisteComNomeAsync(
        string nome,
        int? idIgnorar = null)
    {
        const string sql = """
            SELECT COUNT(*)
            FROM tb_plataforma
            WHERE no_plataforma = @nome
              AND (@idIgnorar IS NULL
                   OR co_seq_plataforma <> @idIgnorar);
            """;

        await using var conexao = await CriarConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@nome", nome);
        AdicionarParametro(comando, "@idIgnorar", idIgnorar);

        var resultado = await comando.ExecuteScalarAsync();

        return Convert.ToInt64(resultado) > 0;
    }

    public async Task<bool> PossuiJogosAsync(int id)
    {
        const string sql = """
            SELECT COUNT(*)
            FROM tb_jogo
            WHERE co_plataforma = @id;
            """;

        await using var conexao = await CriarConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@id", id);

        var resultado = await comando.ExecuteScalarAsync();

        return Convert.ToInt64(resultado) > 0;
    }

    public async Task<int> InserirAsync(Plataforma plataforma)
    {
        const string sql = """
            INSERT INTO tb_plataforma
                (no_plataforma, no_fabricante, nu_ano_lancamento)
            VALUES
                (@nome, @fabricante, @ano);
            """;

        await using var conexao = await CriarConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@nome", plataforma.Nome);
        AdicionarParametro(comando, "@fabricante", plataforma.Fabricante);
        AdicionarParametro(comando, "@ano", plataforma.AnoLancamento);

        await ExecutarGravacaoAsync(comando);

        return checked((int)comando.LastInsertedId);
    }

    public async Task AlterarAsync(Plataforma plataforma)
    {
        const string sql = """
            UPDATE tb_plataforma
            SET no_plataforma = @nome,
                no_fabricante = @fabricante,
                nu_ano_lancamento = @ano
            WHERE co_seq_plataforma = @id;
            """;

        await using var conexao = await CriarConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@id", plataforma.Id);
        AdicionarParametro(comando, "@nome", plataforma.Nome);
        AdicionarParametro(comando, "@fabricante", plataforma.Fabricante);
        AdicionarParametro(comando, "@ano", plataforma.AnoLancamento);

        await ExecutarGravacaoAsync(comando);
    }

    public async Task ExcluirAsync(int id)
    {
        const string sql = """
            DELETE FROM tb_plataforma
            WHERE co_seq_plataforma = @id;
            """;

        await using var conexao = await CriarConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@id", id);

        await comando.ExecuteNonQueryAsync();
    }
}
    