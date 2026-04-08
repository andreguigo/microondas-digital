using Microondas.Aplicacao.Contratos;
using Microondas.Aplicacao.UseCases;
using Microondas.Dominio.Entidades;
using Microondas.Dominio.Enums;
using Xunit;

namespace Microondas.Testes.UseCases;

public class ListarModosUseCaseTestes
{
    [Fact]
    public async Task Deve_mapear_entidades_para_dto()
    {
        var servico = new MicroondasServicoFake
        {
            Modos =
            [
                new ModoAquecimento("Pipoca", "Milho", 7, 90, '#', "Cobrir recipiente")
            ]
        };

        var useCase = new ListarModosUseCase(servico);

        var resultado = await useCase.ExecutarAsync();

        var modo = Assert.Single(resultado);
        Assert.Equal("Pipoca", modo.Nome);
        Assert.Equal("Milho", modo.Alimento);
        Assert.Equal(7, modo.Potencia);
        Assert.Equal(90, modo.Tempo);
        Assert.Equal('#', modo.Caractere);
        Assert.Equal("Cobrir recipiente", modo.Instrucoes);
    }

    private sealed class MicroondasServicoFake : IMicroondasServico
    {
        public List<ModoAquecimento> Modos { get; set; } = [];

        public Task AquecerAsync(int tempo, int? potencia, TipoAquecimento tipo = TipoAquecimento.Manual, string? instrucoes = null, char? caractere = null)
            => Task.CompletedTask;

        public Task InicioRapidoAsync() => Task.CompletedTask;

        public Task<List<ModoAquecimento>> ListarModosAsync() => Task.FromResult(Modos);

        public Task AquecerModoAsync(string nome) => Task.CompletedTask;
    }
}
