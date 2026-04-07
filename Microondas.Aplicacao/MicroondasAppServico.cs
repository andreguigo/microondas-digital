using Microondas.Aplicacao.Dtos;
using Microondas.Aplicacao.UseCases;
using Microondas.Aplicacao.Contratos;
using Microondas.Dominio;

namespace Microondas.Aplicacao;

public class MicroondasAppServico
{
    private readonly AquecerUseCase _aquecerUseCase;
    private readonly InicioRapidoUseCase _inicioRapidoUseCase;
    private readonly ListarModosUseCase _listarModosUseCase;
    private readonly AquecerModoUseCase _aquecerModoUseCase;
    private readonly CadastrarProgramaUseCase _cadastrarProgramaUseCase;

    public MicroondasAppServico(IMicroondasServico servico, IModoAquecimentoRepositorio modoAquecimentoRepositorio)
    {
        _aquecerUseCase = new AquecerUseCase(servico);
        _inicioRapidoUseCase = new InicioRapidoUseCase(servico);
        _listarModosUseCase = new ListarModosUseCase(servico);
        _aquecerModoUseCase = new AquecerModoUseCase(servico);
        _cadastrarProgramaUseCase = new CadastrarProgramaUseCase(modoAquecimentoRepositorio);
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

    public Task CadastrarProgramaAsync(CadastrarProgramaRequestDto request)
    {
        return _cadastrarProgramaUseCase.ExecutarAsync(request);
    }
}