using FluentValidation;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using RetroGames.WebApi.ApiModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;



namespace RetroGames.WebApi.ActionFilters;

public class FluentValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        foreach (var parametro in context.ActionDescriptor.Parameters)
{
    if (parametro.BindingInfo?.BindingSource != BindingSource.Body)
        continue;

    if (!context.ActionArguments.TryGetValue(parametro.Name, out var modelo)
        || modelo is null)
    {
        context.Result = new BadRequestObjectResult(new
        {
            responseCode = ResponseCodes.MODEL_NULL.Code,
            errorMessage = ResponseCodes.MODEL_NULL.DefaultMessage
        });

        return;
    }
}
        foreach (var argumento in context.ActionArguments.Values)
        {
            if (argumento is null)
                continue;

            var tipoValidador = typeof(IValidator<>)
                .MakeGenericType(argumento.GetType());

            var validador = context.HttpContext.RequestServices
                .GetService(tipoValidador) as IValidator;

            if (validador is null)
                continue;

            var resultado = await validador.ValidateAsync(
                new ValidationContext<object>(argumento),
                context.HttpContext.RequestAborted);

            if (!resultado.IsValid)
    {
        var erros = resultado.Errors
        .GroupBy(erro =>
            JsonNamingPolicy.CamelCase.ConvertName(erro.PropertyName))
        .ToDictionary(
            grupo => grupo.Key,
            grupo => grupo.Select(erro => erro.ErrorMessage).ToArray()
        );

    context.Result = new BadRequestObjectResult(new
    {
        responseCode = ResponseCodes.MODEL_STATE_INVALIDO.Code,
        data = erros,
        errorMessage = ResponseCodes.MODEL_STATE_INVALIDO.DefaultMessage
    });

    return;
    }
        }

        await next();
    }
}