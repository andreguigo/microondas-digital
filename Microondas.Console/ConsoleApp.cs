using Microondas.Aplicacao;
using Microondas.Aplicacao.Dtos;

namespace Microondas.App;

public class ConsoleApp
{
    private readonly MicroondasAppServico _appService;

    public ConsoleApp(MicroondasAppServico appService)
    {
        _appService = appService;
    }

    public async Task ExecutarAsync()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== MICROONDAS ===");
            Console.WriteLine("1 - Aquecimento manual");
            Console.WriteLine("2 - Início rápido (+30s)");
            Console.WriteLine("3 - Modos pré-definidos");
            Console.WriteLine("0 - Sair");

            var opcao = Console.ReadLine();

            switch (opcao)
            {
                case "1":
                    await IniciarAquecimentoManualAsync();
                    break;

                case "2":
                    await _appService.InicioRapidoAsync();
                    PausarFluxo();
                    break;

                case "3":
                    await IniciarAquecimentoPorModoAsync();
                    break;

                case "0":
                    return;

                default:
                    Console.WriteLine("Opção inválida!");
                    PausarFluxo();
                    break;
            }
        }
    }

    private async Task IniciarAquecimentoManualAsync()
    {
        Console.Write("Tempo (segundos): ");
        if (!int.TryParse(Console.ReadLine(), out var tempo))
        {
            Console.WriteLine("Tempo inválido!");
            PausarFluxo();
            return;
        }

        Console.Write("Potência (1-10, opcional): ");
        var entradaPotencia = Console.ReadLine();

        int? potencia = null;

        if (!string.IsNullOrWhiteSpace(entradaPotencia))
        {
            if (!int.TryParse(entradaPotencia, out var potenciaInformada))
            {
                Console.WriteLine("Potência inválida!");
                PausarFluxo();
                return;
            }

            potencia = potenciaInformada;
        }

        var request = new AquecerRequestDto
        {
            Tempo = tempo,
            Potencia = potencia
        };

        await _appService.AquecerAsync(request);
        PausarFluxo();
    }

    private async Task IniciarAquecimentoPorModoAsync()
    {
        var modos = await _appService.ListarModosAsync();

        if (!modos.Any())
        {
            Console.WriteLine("Nenhum modo disponível.");
            PausarFluxo();
            return;
        }

        for (var i = 0; i < modos.Count; i++)
        {
            Console.WriteLine($"{i + 1} - {modos[i].Nome}");
        }

        Console.Write("Escolha um modo: ");
        if (!int.TryParse(Console.ReadLine(), out var escolha) || escolha < 1 || escolha > modos.Count)
        {
            Console.WriteLine("Opção inválida!");
            PausarFluxo();
            return;
        }

        await _appService.AquecerModoAsync(modos[escolha - 1].Nome);
        PausarFluxo();
    }
    
    private static void PausarFluxo()
    {
        Console.WriteLine("\nPressione qualquer tecla para continuar...");
        Console.ReadKey();
    }
}