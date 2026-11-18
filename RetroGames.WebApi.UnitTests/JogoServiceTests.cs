using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services;

namespace RetroGames.WebApi.UnitTests;

public class JogoServiceTests
{
    [Theory]
    [InlineData(false, false, false)]
    [InlineData(false, true, false)]
    [InlineData(true, false, false)]
    [InlineData(true, true, true)]
    public async Task ConsultaInativosSomenteQuandoAdministradorSolicita(
        bool administrador, bool incluirInativos, bool filtroEsperado)
    {
        var jogos = new Mock<IJogoRepository>();
        jogos.Setup(r => r.ListarAsync(null, null, null, filtroEsperado, 1, 10))
            .ReturnsAsync((new List<Jogo>(), 0));
        var servico = CriarServico(jogos, administrador);

        await servico.ListarAsync(incluirInativos: incluirInativos);

        jogos.Verify(r => r.ListarAsync(null, null, null, filtroEsperado, 1, 10), Times.Once);
    }

    [Fact]
    public async Task OcultaDetalheInativoParaQuemNaoEAdministrador()
    {
        var jogos = new Mock<IJogoRepository>();
        jogos.Setup(r => r.BuscarPorIdAsync(1))
            .ReturnsAsync(new Jogo { Id = 1, Ativo = false });
        var servico = CriarServico(jogos, administrador: false);

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => servico.BuscarPorIdAsync(1));

        Assert.Equal(BusinessErrors.JOGO_NAO_ENCONTRADO.Code, erro.Erro.Code);
    }

    private static JogoService CriarServico(Mock<IJogoRepository> jogos, bool administrador)
    {
        var repositorios = new Mock<IRepositorySet>();
        repositorios.SetupGet(r => r.Jogo).Returns(jogos.Object);
        var usuario = new Mock<IUsuarioAtualService>();
        usuario.SetupGet(u => u.EhAdministrador).Returns(administrador);
        return new JogoService(repositorios.Object, usuario.Object);
    }
}
