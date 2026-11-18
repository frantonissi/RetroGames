using FluentValidation;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Validators.Autenticacao;

public class LoginRequestValidator : AbstractValidator<LoginRequestApiModel>
{
    public LoginRequestValidator()
    {
        RuleFor(modelo => modelo.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Informe o e-mail.")
            .MaximumLength(255).WithMessage("O e-mail deve ter até 255 caracteres.")
            .EmailAddress().WithMessage("Informe um e-mail válido.");

        RuleFor(modelo => modelo.Senha)
            .NotEmpty().WithMessage("Informe a senha.");
    }
}
