
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class JogoGeneroRepository : RepositoryBase, IJogoGeneroRepository
{
    public JogoGeneroRepository(RepositoryConfig config)
        : base(config)
    {
    }

    public async Task<List<JogoGenero>> ListarTodosAsync()
    {
        const string sql = """
            SELECT co_jogo, co_genero
            FROM tb_jogo_genero
            ORDER BY co_jogo, co_genero;
            """;

        var vinculos = new List<JogoGenero>();

        await using var conexao = await CriarConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        await using var leitor = await comando.ExecuteReaderAsync();

        while (await leitor.ReadAsync())
        {
            vinculos.Add(new JogoGenero
            {
                IdJogo = leitor.GetInt32("co_jogo"),
                IdGenero = leitor.GetInt32("co_genero")
            });
        }

        return vinculos;
    }

    public async Task<List<JogoGenero>> ListarPorJogosAsync(
        IReadOnlyCollection<int> idsJogos)
    {
        var vinculos = new List<JogoGenero>();

        if (idsJogos.Count == 0)
            return vinculos;

        var parametros = idsJogos
            .Select((id, indice) => $"@id{indice}");

        var sql = $"""
            SELECT co_jogo, co_genero
            FROM tb_jogo_genero
            WHERE co_jogo IN ({string.Join(", ", parametros)})
            ORDER BY co_jogo, co_genero;
            """;

        await using var conexao = await CriarConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        int indice = 0;

        foreach (var id in idsJogos)
        {
            AdicionarParametro(comando, $"@id{indice}", id);
            indice++;
        }

        await using var leitor = await comando.ExecuteReaderAsync();

        while (await leitor.ReadAsync())
        {
            vinculos.Add(new JogoGenero
            {
                IdJogo = leitor.GetInt32("co_jogo"),
                IdGenero = leitor.GetInt32("co_genero")
            });
        }

        return vinculos;
    }

    public async Task InserirAsync(
        int idJogo,
        IReadOnlyCollection<int> idsGeneros)
    {
        const string sql = """
            INSERT INTO tb_jogo_genero (co_jogo, co_genero)
            VALUES (@idJogo, @idGenero);
            """;

        await using var conexao = await CriarConexaoAsync();

        foreach (var idGenero in idsGeneros)
        {
            await using var comando = CriarComando(conexao, sql);

            AdicionarParametro(comando, "@idJogo", idJogo);
            AdicionarParametro(comando, "@idGenero", idGenero);

            await ExecutarGravacaoAsync(comando);
        }
    }

    public async Task RemoverPorJogoAsync(int idJogo)
    {
        const string sql = """
            DELETE FROM tb_jogo_genero
            WHERE co_jogo = @idJogo;
            """;

        await using var conexao = await CriarConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@idJogo", idJogo);

        await comando.ExecuteNonQueryAsync();
    }
}
