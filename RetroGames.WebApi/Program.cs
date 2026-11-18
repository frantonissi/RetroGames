
using RetroGames.WebApi.Configuration;
using RetroGames.WebApi.Middlewares;
using FluentValidation;
using RetroGames.WebApi.ActionFilters;
using RetroGames.WebApi.ApiModels; // Ajuste ao namespace do ResponseCodes
using Microsoft.AspNetCore.Mvc;

// Carrega as variáveis do arquivo .env
DotNetEnv.Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

// Lê a connection string
var connectionString = builder.Configuration.GetConnectionString("Default");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "ConnectionStrings:Default não foi configurada.");
}

// Registra as dependências
builder.Services.AddRepositorios(connectionString);
builder.Services.AddServicosDeNegocio();

// Registra os controllers e o OpenAPI
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddControllers(opcoes =>
{
    opcoes.Filters.Add<FluentValidationFilter>();
})
.ConfigureApiBehaviorOptions(opcoes =>
{
    opcoes.InvalidModelStateResponseFactory = contexto =>
    {
        return new BadRequestObjectResult(new
        {
            responseCode = ResponseCodes.MODEL_STATE_INVALIDO.Code,
            errorMessage = ResponseCodes.MODEL_STATE_INVALIDO.DefaultMessage
        });
    };
});
builder.Services.AddOpenApi(options =>
{
    options.OpenApiVersion =
        Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0;
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();

// Disponibiliza o Swagger apenas em desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "RetroGames API v1");
    });
}

// Mapeia as rotas dos controllers
app.MapControllers();

// Inicia a aplicação
app.Run();
