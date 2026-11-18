using RetroGames.Model.Constantes;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class UsuarioRepository : RepositoryBase, IUsuarioRepository
{
    public UsuarioRepository(RepositoryConfig config) : base(config)
    {
    }

    public Task<Usuario?> BuscarPorEmailAsync(string email)
    {
        const string sql = """
            SELECT co_seq_usuario, no_usuario, ds_email, ds_senha_hash,
                   st_ativo, dt_cadastro, co_perfil
            FROM tb_usuario WHERE ds_email = @valor;
            """;
        return BuscarAsync(sql, email);
    }

    public Task<Usuario?> BuscarPorIdAsync(int id)
    {
        const string sql = """
            SELECT co_seq_usuario, no_usuario, ds_email, ds_senha_hash,
                   st_ativo, dt_cadastro, co_perfil
            FROM tb_usuario WHERE co_seq_usuario = @valor;
            """;
        return BuscarAsync(sql, id);
    }

    private async Task<Usuario?> BuscarAsync(string sql, object valor)
    {
        await using var conexao = await CriarConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@valor", valor);
        await using var leitor = await comando.ExecuteReaderAsync();

        if (!await leitor.ReadAsync())
            return null;

        return new Usuario
        {
            Id = leitor.GetInt32(0),
            Nome = leitor.GetString(1),
            Email = leitor.GetString(2),
            SenhaHash = leitor.GetString(3),
            Ativo = leitor.GetBoolean(4),
            DataCadastro = leitor.GetDateTime(5),
            IdPerfil = leitor.GetInt32(6)
        };
    }

    public async Task<bool> ExisteAdministradorAsync()
    {
        const string sql = """
            SELECT 1 FROM tb_usuario WHERE co_perfil = @perfil LIMIT 1;
            """;
        await using var conexao = await CriarConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@perfil", PerfilIds.Administrador);
        return await comando.ExecuteScalarAsync() is not null;
    }

    public async Task<int> InserirAsync(Usuario usuario)
    {
        const string sql = """
            INSERT INTO tb_usuario
                (no_usuario, ds_email, ds_senha_hash, st_ativo, dt_cadastro, co_perfil)
            VALUES (@nome, @email, @hash, @ativo, @cadastro, @perfil);
            """;
        await using var conexao = await CriarConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@nome", usuario.Nome);
        AdicionarParametro(comando, "@email", usuario.Email);
        AdicionarParametro(comando, "@hash", usuario.SenhaHash);
        AdicionarParametro(comando, "@ativo", usuario.Ativo);
        AdicionarParametro(comando, "@cadastro", usuario.DataCadastro);
        AdicionarParametro(comando, "@perfil", usuario.IdPerfil);
        await ExecutarGravacaoAsync(comando);
        return checked((int)comando.LastInsertedId);
    }
}
