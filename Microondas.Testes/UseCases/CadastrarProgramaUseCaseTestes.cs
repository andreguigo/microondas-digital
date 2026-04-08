using Microondas.Aplicacao.Dtos;
using Microondas.Aplicacao.UseCases;
using Microondas.Dominio;
using Microondas.Dominio.Entidades;
using Xunit;

namespace Microondas.Testes.UseCases;

public class CadastrarProgramaUseCaseTestes
{
    [Fact]
    public async Task Deve_retornar_falha_quando_nome_for_vazio()
    {
        var repositorio = new RepositorioFake();
        var useCase = new CadastrarProgramaUseCase(repositorio);
        var request = CriarRequestValido();
        request.Nome = " ";

        var resultado = await useCase.ExecutarAsync(request);

        Assert.False(resultado.EhSucesso);
        Assert.Equal("O nome do programa é obrigatório.", resultado.Mensagem);
    }

    [Fact]
    public async Task Deve_retornar_falha_quando_caractere_ja_estiver_em_uso()
    {
        var repositorio = new RepositorioFake(existeCaractere: true);
        var useCase = new CadastrarProgramaUseCase(repositorio);

        var resultado = await useCase.ExecutarAsync(CriarRequestValido());

        Assert.False(resultado.EhSucesso);
        Assert.Equal("O caractere '#' já está em uso por outro programa.", resultado.Mensagem);
        Assert.Empty(repositorio.ModosAdicionados);
    }

    [Fact]
    public async Task Deve_cadastrar_programa_quando_dados_forem_validos()
    {
        var repositorio = new RepositorioFake();
        var useCase = new CadastrarProgramaUseCase(repositorio);

        var resultado = await useCase.ExecutarAsync(CriarRequestValido());

        Assert.True(resultado.EhSucesso);
        Assert.Equal("Programa 'Pipoca' cadastrado com sucesso!", resultado.Mensagem);

        var modoSalvo = Assert.Single(repositorio.ModosAdicionados);
        Assert.Equal("Pipoca", modoSalvo.Nome);
        Assert.Equal("Milho", modoSalvo.Alimento);
        Assert.Equal(7, modoSalvo.Potencia);
        Assert.Equal(90, modoSalvo.Tempo);
        Assert.Equal('#', modoSalvo.Caractere);
        Assert.Equal("Use recipiente próprio", modoSalvo.Instrucoes);
    }

    private static CadastrarProgramaRequestDto CriarRequestValido() => new()
    {
        Nome = "Pipoca",
        Alimento = "Milho",
        Potencia = 7,
        Tempo = 90,
        Caractere = '#',
        Instrucoes = "Use recipiente próprio"
    };

    private sealed class RepositorioFake : IModoAquecimentoRepositorio
    {
        private readonly bool _existeCaractere;

        public RepositorioFake(bool existeCaractere = false)
        {
            _existeCaractere = existeCaractere;
        }

        public List<ModoAquecimento> ModosAdicionados { get; } = [];

        public Task<List<ModoAquecimento>> ListarAsync() => Task.FromResult(new List<ModoAquecimento>());

        public Task<bool> CaractereExisteAsync(char caractere) => Task.FromResult(_existeCaractere);

        public Task AdicionarAsync(ModoAquecimento modo)
        {
            ModosAdicionados.Add(modo);
            return Task.CompletedTask;
        }
    }
}
