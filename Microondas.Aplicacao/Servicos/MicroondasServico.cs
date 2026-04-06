using Microondas.Dominio.Entidades;
using Microondas.Dominio.Enums;
using Microondas.Dominio.Interfaces;
using Microondas.Aplicacao.Contratos;
using Microondas.Infraestrutura.Concorrencias;

namespace Microondas.Dominio.Servicos;

public class MicroondasServico : IMicroondasServico
{
    private readonly IModoAquecimentoRepositorio _repositorio;
    private CancellationTokenSource? _cts;
    private int _tempoRestante;
    private Aquecimento? _aquecimento;
    
    private readonly PauseTokenSource _pause = new();   
    private EstadoExecucao _estado;

    public MicroondasServico(IModoAquecimentoRepositorio repositorio, PauseTokenSource pause)
    {
        _repositorio = repositorio;
        _pause = pause;
    }

    public async Task AquecerAsync(int tempo, int? potencia, TipoAquecimento tipo = TipoAquecimento.Manual)
    {
        potencia ??= 30;

        try
        {
            var aquecimento = new Aquecimento(tempo, potencia.Value, tipo);

            Console.WriteLine($"Tempo: {aquecimento.ObterTempoFormatado()}");
            Console.WriteLine($"Potência: {potencia}");

            await ExecutarAsync(aquecimento);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public async Task InicioRapidoAsync()
    {
        await AquecerAsync(30, 10);
    }

    public async Task<List<ModoAquecimento>> ListarModosAsync()
    {
        return await _repositorio.ListarAsync();
    }

    public async Task AquecerModoAsync(string nome)
    {
        var modo = (await _repositorio.ListarAsync())
            .First(x => x.Nome == nome);

        await AquecerAsync(modo.Tempo, modo.Potencia, TipoAquecimento.PreDefinido);
    }

    private async Task ExecutarAsync(Aquecimento aquecimento)
    {
        _aquecimento = aquecimento;

        _cts = new CancellationTokenSource();
        var token = _cts.Token;

        _tempoRestante = aquecimento.TempoSegundos;
        _estado = EstadoExecucao.Aquecendo;

        Console.WriteLine("SPACE = Pausar/Continuar | C = Cancelar | Enter = +30s");

        var tarefa = Task.Run(async () =>
        {
            while (_tempoRestante > 0)
            {
                token.ThrowIfCancellationRequested();

                await _pause.Token.WaitIfPausedAsync();

                var indicadorPotencia = new string('.', aquecimento.Potencia);
                Console.Write($"{indicadorPotencia} ");

                Console.WriteLine($"Tempo restante: {new Aquecimento(_tempoRestante).ObterTempoFormatado()}   ");

                await Task.Delay(1000, token);

                Interlocked.Decrement(ref _tempoRestante);
            }

            _estado = EstadoExecucao.Concluido;
        }, token);

        var tecladoTask = MonitorarTeclado();

        await Task.WhenAny(tarefa, tecladoTask);

        _cts.Cancel();

        try
        {
            await tarefa;

            if (_estado == EstadoExecucao.Concluido)
                Console.WriteLine("\nAquecimento concluído!");
        }
        catch (OperationCanceledException)
        {
            if (_estado == EstadoExecucao.Cancelado)
                Console.WriteLine("\nAquecimento cancelado");
        }
    }

    private async Task MonitorarTeclado()
    {
        await Task.Run(() =>
        {
            while (true)
            {
                var key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.Spacebar:
                        AlternarPausa();
                        break;

                    case ConsoleKey.Enter:
                        AdicionarTempo();
                        break;

                    case ConsoleKey.C:
                        Cancelar();
                        return;
                }
            }
        });
    }

    private void AlternarPausa()
    {
        if (_estado == EstadoExecucao.Aquecendo)
        {
            _pause.Pause();
            _estado = EstadoExecucao.Pausado;
            Console.WriteLine("\nPausado");
        }
        else if (_estado == EstadoExecucao.Pausado)
        {
            _pause.Resume();
            _estado = EstadoExecucao.Aquecendo;
            Console.WriteLine("\nRetomado");
        }
    }

    private void Cancelar()
    {
        _estado = EstadoExecucao.Cancelado;
        _cts?.Cancel();
        Console.WriteLine("\nCancelado");
    }

    private void AdicionarTempo()
    {
        if (_aquecimento is null)
            return;

        try
        {
            _tempoRestante = _aquecimento.AdicionarTempo(_tempoRestante, 30);
            Console.WriteLine("\n+30 segundos adicionados");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"\n{ex.Message}");
        }
    }
}