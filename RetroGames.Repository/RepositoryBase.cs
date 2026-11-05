
using MySqlConnector;

namespace RetroGames.Repository;

public abstract class RepositoryBase
{
    private readonly RepositoryConfig _config;

    protected RepositoryBase(RepositoryConfig config)
    {
        _config = config;
    }

    protected async Task<MySqlConnection> CriarConexaoAsync()
    {
        var conexao = new MySqlConnection(_config.ConnectionString);

        try
        {
            await conexao.OpenAsync();
            return conexao;
        }
        catch
        {
            await conexao.DisposeAsync();
            throw;
        }
    }

    protected MySqlCommand CriarComando(
        MySqlConnection conexao,
        string sql)
    {
        return new MySqlCommand(sql, conexao);
    }

    protected void AdicionarParametro(
        MySqlCommand comando,
        string nome,
        object? valor)
    {
        comando.Parameters.AddWithValue(
            nome,
            valor ?? DBNull.Value
        );
    }

    protected async Task<int> ExecutarGravacaoAsync(
        MySqlCommand comando)
    {
        try
        {
            return await comando.ExecuteNonQueryAsync();
        }
        catch (MySqlException erro) when (erro.Number == 1062)
        {
            throw new RegistroDuplicadoException(
                "Já existe um registro com esse valor.",
                erro
            );
        }
    }
}
