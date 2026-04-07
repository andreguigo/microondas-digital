using Microondas.Aplicacao.Dtos;
using Microondas.Aplicacao.UseCases;
using Microondas.Aplicacao.Contratos;

namespace Microondas.Aplicacao;

public class MicroondasAppServico
{
    private readonly AquecerUseCase _aquecerUseCase;
    private readonly InicioRapidoUseCase _inicioRapidoUseCase;
    private readonly ListarModosUseCase _listarModosUseCase;
    private readonly AquecerModoUseCase _aquecerModoUseCase;

    public MicroondasAppServico(IMicroondasServico servico)
    {
        _aquecerUseCase = new AquecerUseCase(servico);
        _inicioRapidoUseCase = new InicioRapidoUseCase(servico);
        _listarModosUseCase = new ListarModosUseCase(servico);
        _aquecerModoUseCase = new AquecerModoUseCase(servico);
    }

    public Task AquecerAsync(AquecerRequestDto request)
    {
        return _aquecerUseCase.ExecutarAsync(request);
    }

    public Task InicioRapidoAsync()
    {
        return _inicioRapidoUseCase.ExecutarAsync();
    }

    public Task<List<ModoDto>> ListarModosAsync()
    {
        return _listarModosUseCase.ExecutarAsync();
    }

    public Task AquecerModoAsync(string nome)
    {
        return _aquecerModoUseCase.ExecutarAsync(nome);
    }
}