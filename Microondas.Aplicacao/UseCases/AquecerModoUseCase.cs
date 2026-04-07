using Microondas.Aplicacao.Contratos;

namespace Microondas.Aplicacao.UseCases;

public class AquecerModoUseCase
{
    private readonly IMicroondasServico _servico;

    public AquecerModoUseCase(IMicroondasServico servico)
    {
        _servico = servico;
    }

    public Task ExecutarAsync(string nomeModo)
    {
        return _servico.AquecerModoAsync(nomeModo);
    }
}