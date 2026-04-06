namespace Microondas.Dominio.Entidades;

public class ModoAquecimento
{
    public string Nome { get; set; } = string.Empty;
    public string Alimento { get; set; } = string.Empty;
    public int Potencia { get; set; }
    public int Tempo { get; set; }
    public char Caractere { get; set; }
    public string? Instrucoes { get; set; }

    public ModoAquecimento(
        string nome, 
        string alimento,
        int potencia, 
        int tempo, 
        char caractere,
        string? instrucoes = null)
    {
        Nome = nome;
        Alimento = alimento;
        Potencia = potencia;
        Tempo = tempo;
        Caractere = caractere;
        Instrucoes = instrucoes;
    }
}