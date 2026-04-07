using Microondas.Aplicacao.Contratos;
using Microondas.Aplicacao.Dtos;

namespace Microondas.Aplicacao.UseCases;

public class ListarModosUseCase
{
    private readonly IMicroondasServico _servico;

    public ListarModosUseCase(IMicroondasServico servico)
    {
        _servico = servico;
    }

    public async Task<List<ModoDto>> ExecutarAsync()
    {
        var modos = await _servico.ListarModosAsync();

        return modos.Select(m => new ModoDto
        {
            Nome = m.Nome,
            Alimento = m.Alimento,
            Tempo = m.Tempo,
            Potencia = m.Potencia,
            Instrucoes = m.Instrucoes,
            Caractere = m.Caractere
        }).ToList();
    }
}