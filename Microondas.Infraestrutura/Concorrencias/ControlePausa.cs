using Microondas.Aplicacao.Contratos;

namespace Microondas.Infraestrutura.Concorrencias;

public class ControlePausa : IControlePausa
{
    private volatile TaskCompletionSource<bool>? _pausado;

    public bool EstaPausado => _pausado != null;

    public void Pausar()
    {
        Interlocked.CompareExchange(ref _pausado, new TaskCompletionSource<bool>(), null);
    }

    public void Retomar()
    {
        while (true)
        {
            var pausado = _pausado;

            if (pausado is null)
                return;

            if (Interlocked.CompareExchange(ref _pausado, null, pausado) == pausado)
            {
                pausado.SetResult(true);
                return;
            }
        }
    }

    public Task AguardarSePausadoAsync()
    {
        return _pausado?.Task ?? Task.CompletedTask;
    }
}