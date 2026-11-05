
namespace RetroGames.Repository;

public class RegistroDuplicadoException : Exception
{
    public RegistroDuplicadoException()
        : base("Registro duplicado no banco de dados.")
    {
    }

    public RegistroDuplicadoException(string mensagem, Exception innerException)
        : base(mensagem, innerException)
    {
    }
}
