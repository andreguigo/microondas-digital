using Microondas.Dominio.Enums;

namespace Microondas.Dominio.Entidades;

public class Aquecimento
{
    public int TempoSegundos { get; }
    public int Potencia { get; }
    public char Caractere { get; set; }
    public TipoAquecimento Tipo { get; }

    private const int TempoMinimo = 1;
    private const int TempoMaximo = 120;
    private const int PotenciaMinima = 1;
    private const int PotenciaMaxima = 10;

    public Aquecimento(int tempoSegundos, int potencia, TipoAquecimento tipo)
    {
        Validar(tempoSegundos, potencia, tipo);

        TempoSegundos = tempoSegundos;
        Potencia = potencia;
        Tipo = tipo;
    }

    public Aquecimento(int tempoSegundos)
    {
        TempoSegundos = tempoSegundos;
    }

    private static void Validar(int tempoSegundos, int potencia, TipoAquecimento tipo)
    {
        if (tipo == TipoAquecimento.PreDefinido)
            return;

        if (tempoSegundos < TempoMinimo || tempoSegundos > TempoMaximo)
            throw new ArgumentException($"Tempo deve estar entre {TempoMinimo} e {TempoMaximo} segundos");

        if (potencia < PotenciaMinima || potencia > PotenciaMaxima)
            throw new ArgumentException($"Potência deve estar entre {PotenciaMinima} e {PotenciaMaxima}");
    }

    public int AdicionarTempo(int tempoAtual, int segundos)
    {
        if (Tipo == TipoAquecimento.PreDefinido)
            throw new InvalidOperationException("Não é possível alterar o tempo para este modo.");

        return tempoAtual + segundos;
    }

    public string ObterTempoFormatado()
    {
        var minutos = TempoSegundos / 60;
        var segundos = TempoSegundos % 60;

        return $"{minutos}:{segundos:D2}";
    }
}