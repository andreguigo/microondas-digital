using Microondas.Dominio.Interfaces;
using Microondas.Aplicacao.DTOs;
using Microondas.Aplicacao.Contratos;

namespace Microondas.Aplicacao;

public class MicroondasAppServico
{
    private readonly IMicroondasServico _servico;

    public MicroondasAppServico(IMicroondasServico servico)
    {
        _servico = servico;
    }

    public async Task AquecerAsync(AquecerRequestDto request)
    {
        await _servico.AquecerAsync(request.Tempo, request.Potencia);
    }

    public async Task InicioRapidoAsync()
    {
        await _servico.InicioRapidoAsync();
    }

    public async Task<List<ModoDto>> ListarModosAsync()
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

    public async Task AquecerModoAsync(string nome)
    {
        await _servico.AquecerModoAsync(nome);
    }
}