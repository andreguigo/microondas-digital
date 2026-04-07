using Microondas.Aplicacao.Dtos;
using Microondas.Dominio;
using Microondas.Dominio.Entidades;

namespace Microondas.Aplicacao.UseCases;

public class CadastrarProgramaUseCase
{
    private readonly IModoAquecimentoRepositorio _repositorio;

    public CadastrarProgramaUseCase(IModoAquecimentoRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<ResultadoCadastro> ExecutarAsync(CadastrarProgramaRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Nome))
            return ResultadoCadastro.Falha("O nome do programa é obrigatório.");

        if (string.IsNullOrWhiteSpace(request.Alimento))
            return ResultadoCadastro.Falha("O alimento é obrigatório.");

        if (request.Potencia < 1 || request.Potencia > 10)
            return ResultadoCadastro.Falha("A potência deve estar entre 1 e 10.");

        if (request.Tempo <= 0)
            return ResultadoCadastro.Falha("O tempo deve ser maior que zero.");

        if (request.Caractere == '\0' || char.IsWhiteSpace(request.Caractere))
            return ResultadoCadastro.Falha("O caractere de aquecimento é obrigatório.");

        var caractereExiste = await _repositorio.CaractereExisteAsync(request.Caractere);
        if (caractereExiste)
            return ResultadoCadastro.Falha(
                $"O caractere '{request.Caractere}' já está em uso por outro programa.");

        var novoModo = new ModoAquecimento(
            request.Nome,
            request.Alimento,
            request.Potencia,
            request.Tempo,
            request.Caractere,
            request.Instrucoes
        );

        await _repositorio.AdicionarAsync(novoModo);

        return ResultadoCadastro.Sucesso(
            $"Programa '{request.Nome}' cadastrado com sucesso!");
    }
}

public class ResultadoCadastro
{
    public bool EhSucesso { get; private set; }
    public string Mensagem { get; private set; } = string.Empty;

    private ResultadoCadastro() { }

    public static ResultadoCadastro Sucesso(string mensagem) =>
        new() { EhSucesso = true, Mensagem = mensagem };

    public static ResultadoCadastro Falha(string mensagem) =>
        new() { EhSucesso = false, Mensagem = mensagem };
}