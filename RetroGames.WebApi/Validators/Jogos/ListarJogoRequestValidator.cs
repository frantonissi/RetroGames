using FluentValidation;
using RetroGames.WebApi.ApiModels;
// Importe aqui o namespace do ListarJogoRequestApiModel

namespace RetroGames.WebApi.Validators.Jogo;

public class ListarJogoRequestValidator
    : AbstractValidator<ListarJogoRequestApiModel>
{
    public ListarJogoRequestValidator()
    {
        RuleFor(x => x.Pagina)
            .GreaterThan(0)
            .WithMessage("A página deve ser maior que zero.");

        RuleFor(x => x.TamanhoPagina)
            .InclusiveBetween(1, 50)
            .WithMessage("O tamanho da página deve estar entre 1 e 50.");
    }
}