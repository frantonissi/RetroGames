
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class CondicaoJogoRepository : RepositoryBase, ICondicaoJogoRepository
{
    public CondicaoJogoRepository(RepositoryConfig config)
        : base(config)
    {
    }

    public async Task<List<CondicaoJogo>> ListarAsync()
    {
        const string sql = """
            SELECT co_condicao_jogo, ds_condicao_jogo
            FROM tb_condicao_jogo
            ORDER BY co_condicao_jogo;
            """;

        var condicoes = new List<CondicaoJogo>();

        await using var conexao = await CriarConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        await using var leitor = await comando.ExecuteReaderAsync();

        while (await leitor.ReadAsync())
        {
            var condicao = new CondicaoJogo
            {
                Id = leitor.GetInt32("co_condicao_jogo"),
                Descricao = leitor.GetString("ds_condicao_jogo")
            };

            condicoes.Add(condicao);
        }

        return condicoes;
    }
}
