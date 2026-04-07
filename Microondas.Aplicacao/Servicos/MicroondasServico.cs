using Microondas.Aplicacao.Contratos;
using Microondas.Dominio.Entidades;
using Microondas.Dominio.Enums;
using Microondas.Dominio;

namespace Microondas.Aplicacao.Servicos;

public class MicroondasServico : IMicroondasServico
{
    private readonly IModoAquecimentoRepositorio _repositorio;
    private readonly IControlePausa _controlePausa;

    private CancellationTokenSource? _cancelamento;
    private int _tempoRestante;
    private Aquecimento? _aquecimentoAtual;
    private EstadoExecucao _estado;
    
    public MicroondasServico(IModoAquecimentoRepositorio repositorio, IControlePausa controlePausa)
    {
        _repositorio = repositorio;
        _controlePausa = controlePausa;
    }

    public async Task AquecerAsync(int tempo, int? potencia, TipoAquecimento tipo = TipoAquecimento.Manual)
    {
        var potenciaEfetiva = potencia ?? 10;

        try
        {
            var aquecimento = new Aquecimento(tempo, potenciaEfetiva, tipo);

            Console.WriteLine($"Tempo: {aquecimento.ObterTempoFormatado()}");
            Console.WriteLine($"Potência: {potenciaEfetiva}");

            await ExecutarAquecimentoAsync(aquecimento);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public Task InicioRapidoAsync()
    {
        return AquecerAsync(30, 10);
    }

    public Task<List<ModoAquecimento>> ListarModosAsync()
    {
        return _repositorio.ListarAsync();
    }

    public async Task AquecerModoAsync(string nome)
    {
        var modo = (await _repositorio.ListarAsync())
            .FirstOrDefault(x => x.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase));
        
        if (modo is null)
        {
            Console.WriteLine("Modo de aquecimento não encontrado.");
            return;
        }

        await AquecerAsync(modo.Tempo, modo.Potencia, TipoAquecimento.PreDefinido);
    }

    private async Task ExecutarAquecimentoAsync(Aquecimento aquecimento)
    {
        _aquecimentoAtual = aquecimento;
        _cancelamento = new CancellationTokenSource();

        var token = _cancelamento.Token;
        _tempoRestante = aquecimento.TempoSegundos;
        _estado = EstadoExecucao.Aquecendo;

        Console.WriteLine("SPACE = Pausar/Continuar | C = Cancelar | Enter = +30s");

        var aquecimentoTask = Task.Run(async () =>
        {
            while (_tempoRestante > 0)
            {
                token.ThrowIfCancellationRequested();

                await _controlePausa.AguardarSePausadoAsync();

                var indicadorPotencia = new string('.', aquecimento.Potencia);
                
                Console.Write($"{indicadorPotencia} Tempo restante: {new Aquecimento(_tempoRestante).ObterTempoFormatado()} ");

                await Task.Delay(1000, token);

                Interlocked.Decrement(ref _tempoRestante);
            }

            _estado = EstadoExecucao.Concluido;
        }, token);

        var tecladoTask = MonitorarTecladoAsync();

        await Task.WhenAny(aquecimentoTask, tecladoTask);
        _cancelamento.Cancel();

        try
        {
            await aquecimentoTask;

            if (_estado == EstadoExecucao.Concluido) 
                Console.WriteLine("\nAquecimento concluído!");
        }
        catch (OperationCanceledException)
        {
            if (_estado == EstadoExecucao.Cancelado)
                Console.WriteLine("\nAquecimento cancelado");
        }
    }

    private Task MonitorarTecladoAsync()
    {
        return Task.Run(() =>
        {
            while (true)
            {
                var tecla = Console.ReadKey(true).Key;

                switch (tecla)
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
        if (_controlePausa.EstaPausado)
        {
            _controlePausa.Retomar();
            _estado = EstadoExecucao.Aquecendo;
            Console.WriteLine("\nRetomado");
        }
        else
        {
            _controlePausa.Pausar();
            _estado = EstadoExecucao.Pausado;
            Console.WriteLine("\nPausado");
        }
    }

    private void Cancelar()
    {
        _estado = EstadoExecucao.Cancelado;
        _cancelamento?.Cancel();
        Console.WriteLine("\nCancelado");
    }

    private void AdicionarTempo()
    {
        if (_aquecimentoAtual is null)
            return;

        try
        {
            _tempoRestante = _aquecimentoAtual.AdicionarTempo(_tempoRestante, 30);
            Console.WriteLine("\n+30 segundos adicionados");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"\n{ex.Message}");
        }
    }
}