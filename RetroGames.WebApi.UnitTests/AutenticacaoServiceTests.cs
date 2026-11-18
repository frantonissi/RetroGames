using Microsoft.AspNetCore.Identity;
using Moq;
using RetroGames.Model.Constantes;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services;

namespace RetroGames.WebApi.UnitTests;

public class AutenticacaoServiceTests
{
    private readonly Mock<IUsuarioRepository> _usuarios = new();
    private readonly PasswordHasher<Usuario> _hasher = new();
    private readonly AutenticacaoService _servico;
    private readonly string _senhaDeTeste = Guid.NewGuid().ToString();

    public AutenticacaoServiceTests()
    {
        var repositorios = new Mock<IRepositorySet>();
        repositorios.SetupGet(r => r.Usuario).Returns(_usuarios.Object);
        _servico = new AutenticacaoService(repositorios.Object, _hasher);
    }

    [Theory]
    [InlineData(false, false, false)]
    [InlineData(true, false, true)]
    [InlineData(true, true, false)]
    public async Task RetornaNuloParaFalhasDeAutenticacao(
        bool existe, bool ativo, bool senhaCorreta)
    {
        var usuario = CriarUsuario(ativo);
        _usuarios.Setup(r => r.BuscarPorEmailAsync(usuario.Email))
            .ReturnsAsync(existe ? usuario : null);

        var resultado = await _servico.AutenticarAsync(
            usuario.Email, senhaCorreta ? _senhaDeTeste : Guid.NewGuid().ToString());

        Assert.Null(resultado);
    }

    [Fact]
    public async Task AceitaSenhaCorretaDeUsuarioAtivo()
    {
        var usuario = CriarUsuario(ativo: true);
        _usuarios.Setup(r => r.BuscarPorEmailAsync(usuario.Email)).ReturnsAsync(usuario);

        var resultado = await _servico.AutenticarAsync(usuario.Email, _senhaDeTeste);

        Assert.Same(usuario, resultado);
    }

    [Fact]
    public async Task CriaAdministradorComSenhaEmHash()
    {
        Usuario? cadastrado = null;
        _usuarios.Setup(r => r.InserirAsync(It.IsAny<Usuario>()))
            .Callback<Usuario>(usuario => cadastrado = usuario)
            .ReturnsAsync(1);

        await _servico.GarantirAdministradorInicialAsync("admin@example.test", _senhaDeTeste);

        Assert.NotNull(cadastrado);
        Assert.Equal(PerfilIds.Administrador, cadastrado.IdPerfil);
        Assert.True(cadastrado.Ativo);
        Assert.NotEqual(_senhaDeTeste, cadastrado.SenhaHash);
        Assert.Equal(PasswordVerificationResult.Success,
            _hasher.VerifyHashedPassword(cadastrado, cadastrado.SenhaHash, _senhaDeTeste));
    }

    [Fact]
    public async Task PreservaAdministradorExistente()
    {
        _usuarios.Setup(r => r.ExisteAdministradorAsync()).ReturnsAsync(true);

        await _servico.GarantirAdministradorInicialAsync(null, null);

        _usuarios.Verify(r => r.InserirAsync(It.IsAny<Usuario>()), Times.Never);
    }

    private Usuario CriarUsuario(bool ativo)
    {
        var usuario = new Usuario { Email = "usuario@example.test", Ativo = ativo };
        usuario.SenhaHash = _hasher.HashPassword(usuario, _senhaDeTeste);
        return usuario;
    }
}
