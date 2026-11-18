using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Validators.Autenticacao;

namespace RetroGames.WebApi.UnitTests;

public class AutenticacaoValidatorsTests
{
    [Theory]
    [InlineData(null, "fixture", nameof(LoginRequestApiModel.Email))]
    [InlineData("email-invalido", "fixture", nameof(LoginRequestApiModel.Email))]
    [InlineData("teste@example.test", null, nameof(LoginRequestApiModel.Senha))]
    [InlineData("teste@example.test", "  ", nameof(LoginRequestApiModel.Senha))]
    public void RejeitaCamposInvalidos(string? email, string? senha, string campo)
    {
        var resultado = new LoginRequestValidator().Validate(new LoginRequestApiModel
        {
            Email = email,
            Senha = senha
        });

        Assert.Contains(resultado.Errors, erro => erro.PropertyName == campo);
    }

    [Fact]
    public void AceitaCamposValidos()
    {
        var resultado = new LoginRequestValidator().Validate(new LoginRequestApiModel
        {
            Email = "teste@example.test",
            Senha = new string('x', 16)
        });

        Assert.True(resultado.IsValid);
    }
}
