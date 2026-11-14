
using RetroGames.WebApi.Configuration;

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
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

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
