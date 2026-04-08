using Microondas.Dominio.Entidades;
using Microondas.Dominio.Enums;
using Xunit;

namespace Microondas.Testes.Dominio;

public class AquecimentoTestes
{
    [Theory]
    [InlineData(0)]
    [InlineData(121)]
    public void Deve_lancar_excecao_quando_tempo_manual_for_invalido(int tempoInvalido)
    {
        var acao = () => new Aquecimento(tempoInvalido, 10, '.', TipoAquecimento.Manual);

        var excecao = Assert.Throws<ArgumentException>(acao);
        Assert.Contains("Tempo deve estar entre 1 e 120 segundos", excecao.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    public void Deve_lancar_excecao_quando_potencia_manual_for_invalida(int potenciaInvalida)
    {
        var acao = () => new Aquecimento(30, potenciaInvalida, '.', TipoAquecimento.Manual);

        var excecao = Assert.Throws<ArgumentException>(acao);
        Assert.Contains("Potência deve estar entre 1 e 10", excecao.Message);
    }

    [Fact]
    public void Deve_permitir_tempo_fora_do_intervalo_para_aquecimento_predefinido()
    {
        var aquecimento = new Aquecimento(300, 15, '#', TipoAquecimento.PreDefinido);

        Assert.Equal(300, aquecimento.TempoSegundos);
        Assert.Equal(15, aquecimento.Potencia);
    }

    [Fact]
    public void Deve_adicionar_tempo_no_aquecimento_manual()
    {
        var aquecimento = new Aquecimento(30, 8, '.', TipoAquecimento.Manual);

        var tempoAtualizado = aquecimento.AdicionarTempo(30, 30);

        Assert.Equal(60, tempoAtualizado);
    }

    [Fact]
    public void Deve_lancar_excecao_ao_adicionar_tempo_no_modo_predefinido()
    {
        var aquecimento = new Aquecimento(60, 5, '*', TipoAquecimento.PreDefinido);

        void acao() => aquecimento.AdicionarTempo(60, 30);

        var excecao = Assert.Throws<InvalidOperationException>(acao);
        Assert.Equal("Não é possível alterar o tempo para este modo.", excecao.Message);
    }

    [Fact]
    public void Deve_formatar_tempo_em_minutos_e_segundos()
    {
        var aquecimento = new Aquecimento(125);

        var tempoFormatado = aquecimento.ObterTempoFormatado();

        Assert.Equal("2:05", tempoFormatado);
    }
}
