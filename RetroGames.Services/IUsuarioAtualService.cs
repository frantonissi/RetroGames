namespace RetroGames.Services;

public interface IUsuarioAtualService
{
    bool EstaAutenticado { get; }
    int? Id { get; }
    bool EhAdministrador { get; }
}
