namespace Microondas.Api.Modelos;

public class TokenResponse
{
    public bool Autenticado { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiraEmUtc { get; set; }
}
