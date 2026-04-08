namespace Microondas.Api.Modelos;

public class SessaoCredencialResponse
{
    public string Usuario { get; set; } = string.Empty;
    public string SenhaMascarada { get; set; } = string.Empty;
    public string AlgoritmoHash { get; set; } = "SHA1";
}