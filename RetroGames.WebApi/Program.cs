
using RetroGames.Services;
using RetroGames.WebApi.Configuration;
using RetroGames.WebApi.Middlewares;
using FluentValidation;
using RetroGames.WebApi.ActionFilters;
using RetroGames.WebApi.ApiModels; // Ajuste ao namespace do ResponseCodes
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    DotNetEnv.Env.NoClobber().TraversePath().Load();
    builder.Configuration.AddEnvironmentVariables();
}

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
builder.Services.AddAutenticacao(builder.Environment.IsDevelopment());
builder.Services.AddAuthorization();

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

await using (var escopo = app.Services.CreateAsyncScope())
{
    var autenticacao = escopo.ServiceProvider.GetRequiredService<IAutenticacaoService>();
    await autenticacao.GarantirAdministradorInicialAsync(
        app.Configuration["AdminInicial:Email"],
        app.Configuration["AdminInicial:Senha"]);
}

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

app.UseAuthentication();
app.UseAuthorization();

// Mapeia as rotas dos controllers
app.MapControllers();

// Inicia a aplicação
app.Run();
