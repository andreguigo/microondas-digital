using Microondas.Aplicacao.DTOs;
using Microondas.Dominio.Interfaces;
using Microondas.Aplicacao.Contratos;

namespace Microondas.Aplicacao.UseCases;

public class AquecerUseCase
{
    private readonly IMicroondasServico _servico;

    public AquecerUseCase(IMicroondasServico servico)
    {
        _servico = servico;
    }

    public async Task Executar(AquecerRequestDto dto)
    {
        await _servico.AquecerAsync(dto.Tempo, dto.Potencia);
    }
}