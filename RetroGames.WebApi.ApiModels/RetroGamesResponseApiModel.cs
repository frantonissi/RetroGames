
using System.Text.Json.Serialization;

namespace RetroGames.WebApi.ApiModels;

public class RetroGamesResponseApiModel
{
    public int ResponseCode { get; set; } = ResponseCodes.SUCESSO;

    public string? ErrorMessage { get; set; }
}

public class RetroGamesResponseApiModel<T> : RetroGamesResponseApiModel
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public T? Data { get; set; }

    public static implicit operator RetroGamesResponseApiModel<T>(RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel> v)
    {
        throw new NotImplementedException();
    }
}
