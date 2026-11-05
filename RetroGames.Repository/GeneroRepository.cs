
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class GeneroRepository : RepositoryBase, IGeneroRepository
{
    public GeneroRepository(RepositoryConfig config)
        : base(config)
    {
    }

    public async Task<List<Genero>> ListarAsync()
    {
        const string sql = """
            SELECT co_seq_genero, no_genero
            FROM tb_genero
            ORDER BY no_genero;
            """;

        var generos = new List<Genero>();

        await using var conexao = await CriarConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        await using var leitor = await comando.ExecuteReaderAsync();

        while (await leitor.ReadAsync())
        {
            var genero = new Genero
            {
                Id = leitor.GetInt32("co_seq_genero"),
                Nome = leitor.GetString("no_genero")
            };

            generos.Add(genero);
        }

        return generos;
    }
}
