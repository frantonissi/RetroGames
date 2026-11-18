using Microsoft.AspNetCore.Identity;
using RetroGames.Model.Model;

using Microsoft.Extensions.DependencyInjection;
using RetroGames.Repository;
using RetroGames.Services;
using RetroGames.WebApi.Services;

namespace RetroGames.WebApi.Configuration;

public static class InjecaoDependenciaConfig
{
    public static IServiceCollection AddRepositorios(
        this IServiceCollection servicos,
        string connectionString)
    {
        servicos.AddSingleton(
    new RepositoryConfig(connectionString)
        );

        servicos.AddScoped<IPlataformaRepository, PlataformaRepository>();
        servicos.AddScoped<IJogoRepository, JogoRepository>();
        servicos.AddScoped<IJogoGeneroRepository, JogoGeneroRepository>();
        servicos.AddScoped<IGeneroRepository, GeneroRepository>();
        servicos.AddScoped<ICondicaoJogoRepository, CondicaoJogoRepository>();
        servicos.AddScoped<IUsuarioRepository, UsuarioRepository>();
        servicos.AddScoped<IRepositorySet, RepositorySet>();

        return servicos;
    }

    public static IServiceCollection AddServicosDeNegocio(
        this IServiceCollection servicos)
    {
        servicos.AddHttpContextAccessor();
        servicos.AddScoped<IUsuarioAtualService, UsuarioAtualService>();
        servicos.AddScoped<IPlataformaService, PlataformaService>();
        servicos.AddScoped<IJogoService, JogoService>();
        servicos.AddScoped<IGeneroService, GeneroService>();
        servicos.AddScoped<ICondicaoJogoService, CondicaoJogoService>();
        servicos.AddScoped<IAutenticacaoService, AutenticacaoService>();
        servicos.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();

        return servicos;
    }
}
