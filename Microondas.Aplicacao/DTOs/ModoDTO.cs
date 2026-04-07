namespace Microondas.Aplicacao.Dtos;

public class ModoDto
{
    public string Nome { get; set; } = string.Empty;
    public string Alimento { get; set; } = string.Empty;
    public int Potencia { get; set; }
    public int Tempo { get; set; }
    public char Caractere { get; set; }
    public string? Instrucoes { get; set; }
}