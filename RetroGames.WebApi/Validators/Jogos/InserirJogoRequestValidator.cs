using FluentValidation;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Validators.Jogo;

public class InserirJogoRequestValidator
    : AbstractValidator<InserirJogoRequestApiModel>
{
    public InserirJogoRequestValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Informe o nome.")
            .MaximumLength(150).WithMessage("Nome muito longo.");

        RuleFor(x => x.Descricao)
            .MaximumLength(1000)
            .WithMessage("Descrição muito longa.");

        RuleFor(x => x.AnoLancamento)
            .InclusiveBetween(1970, 2100)
            .WithMessage("Ano de lançamento inválido.");

        RuleFor(x => x.Preco)
            .GreaterThan(0).WithMessage("O preço deve ser maior que zero.")
            .PrecisionScale(10, 2, true)
            .WithMessage("O preço deve ter até 2 casas decimais.");

        RuleFor(x => x.Estoque)
            .GreaterThanOrEqualTo(0)
            .WithMessage("O estoque não pode ser negativo.");

        RuleFor(x => x.IdPlataforma)
            .GreaterThan(0)
            .WithMessage("Informe a plataforma.");

        RuleFor(x => x.IdCondicaoJogo)
            .GreaterThan(0)
            .WithMessage("Informe a condição do jogo.");

        RuleFor(x => x.IdsGeneros)
            .NotEmpty()
            .WithMessage("Informe pelo menos um gênero.");
    }
}