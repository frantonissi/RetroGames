using Microsoft.AspNetCore.Identity;
using RetroGames.Model.Constantes;
using RetroGames.Model.Model;
using RetroGames.Repository;

namespace RetroGames.Services;

public class AutenticacaoService(
    IRepositorySet repositorios,
    IPasswordHasher<Usuario> hasher) : IAutenticacaoService
{
    public async Task<Usuario?> AutenticarAsync(string email, string senha)
    {
        var usuario = await repositorios.Usuario.BuscarPorEmailAsync(email);

        if (usuario is null || !usuario.Ativo)
            return null;

        var resultado = hasher.VerifyHashedPassword(usuario, usuario.SenhaHash, senha);
        return resultado == PasswordVerificationResult.Failed ? null : usuario;
    }

    public async Task GarantirAdministradorInicialAsync(string? email, string? senha)
    {
        if (await repositorios.Usuario.ExisteAdministradorAsync())
            return;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
            throw new InvalidOperationException("Configure o e-mail e a senha do administrador inicial.");

        var administrador = new Usuario
        {
            Nome = Perfis.Administrador,
            Email = email.Trim(),
            Ativo = true,
            IdPerfil = PerfilIds.Administrador,
            DataCadastro = DateTime.UtcNow
        };
        administrador.SenhaHash = hasher.HashPassword(administrador, senha);

        try
        {
            await repositorios.Usuario.InserirAsync(administrador);
        }
        catch (RegistroDuplicadoException)
        {
            // Outra instância pode ter criado o administrador durante a inicialização.
            if (!await repositorios.Usuario.ExisteAdministradorAsync())
                throw new InvalidOperationException("O e-mail do administrador inicial já está em uso.");
        }
    }
}
