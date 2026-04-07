using Microondas.Aplicacao.Contratos;

namespace Microondas.Aplicacao.UseCases;

public class InicioRapidoUseCase
{
    private readonly IMicroondasServico _servico;

    public InicioRapidoUseCase(IMicroondasServico servico)
    {
        _servico = servico;
    }

    public Task ExecutarAsync()
    {
        return _servico.InicioRapidoAsync();
    }
}