using Microondas.Aplicacao.Contratos;
using Microondas.Aplicacao.Dtos;

namespace Microondas.Aplicacao.UseCases;

public class AquecerUseCase
{
    private readonly IMicroondasServico _servico;

    public AquecerUseCase(IMicroondasServico servico)
    {
        _servico = servico;
    }

    public Task ExecutarAsync(AquecerRequestDto request)
    {
        return _servico.AquecerAsync(request.Tempo, request.Potencia);
    }
}