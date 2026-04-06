namespace Microondas.Infraestrutura.Concorrencias;

public class PauseTokenSource
{
    private volatile TaskCompletionSource<bool>? _paused;

    public bool IsPaused => _paused != null;

    public PauseToken Token => new(this);

    public void Pause()
    {
        Interlocked.CompareExchange(ref _paused, new TaskCompletionSource<bool>(), null);
    }

    public void Resume()
    {
        while (true)
        {
            var paused = _paused;
            if (paused == null)
                return;

            if (Interlocked.CompareExchange(ref _paused, null, paused) == paused)
            {
                paused.SetResult(true);
                break;
            }
        }
    }

    internal Task WaitIfPausedAsync()
    {
        return _paused?.Task ?? Task.CompletedTask;
    }
}

public readonly struct PauseToken
{
    private readonly PauseTokenSource _source;

    public PauseToken(PauseTokenSource source)
    {
        _source = source;
    }

    public Task WaitIfPausedAsync() => _source?.WaitIfPausedAsync() ?? Task.CompletedTask;
}