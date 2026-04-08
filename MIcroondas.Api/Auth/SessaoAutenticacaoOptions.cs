namespace Microondas.Api.Auth;

public class SessaoAutenticacaoOptions
{
    public const string SectionName = "SessaoAutenticacao";

    public string Usuario { get; set; } = "admin";
    public string SenhaSha1 { get; set; } = string.Empty;
    public string ChaveJwt { get; set; } = "microondas-chave-jwt-minimo-32-caracteres";
    public string Emissor { get; set; } = "Microondas.Api";
    public string Audiencia { get; set; } = "Microondas.Clientes";
    public int ExpiracaoMinutos { get; set; } = 60;
}