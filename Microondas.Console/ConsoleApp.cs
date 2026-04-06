using Microondas.Aplicacao.DTOs;

namespace Microondas.Aplicacao;

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

            var op = Console.ReadLine();

            switch (op)
            {
                case "1":
                    await ManualAsync();
                    break;

                case "2":
                    await _appService.InicioRapidoAsync();
                    Pausar();
                    break;

                case "3":
                    await ModosAsync();
                    break;

                case "0":
                    return;

                default:
                    Console.WriteLine("Opção inválida!");
                    Pausar();
                    break;
            }
        }
    }

    private async Task ManualAsync()
    {
        Console.Write("Tempo (segundos): ");
        if (!int.TryParse(Console.ReadLine(), out int tempo))
        {
            Console.WriteLine("Tempo inválido!");
            Pausar();
            return;
        }

        Console.Write("Potência (1-10, opcional): ");
        var entradaPot = Console.ReadLine();

        int? potencia = null;

        if (!string.IsNullOrWhiteSpace(entradaPot))
        {
            if (int.TryParse(entradaPot, out int pot))
                potencia = pot;
            else
            {
                Console.WriteLine("Potência inválida!");
                Pausar();
                return;
            }
        }

        var request = new AquecerRequestDto
        {
            Tempo = tempo,
            Potencia = potencia
        };

        await _appService.AquecerAsync(request);
        Pausar();
    }

    private async Task ModosAsync()
    {
        var modos = await _appService.ListarModosAsync();

        if (!modos.Any())
        {
            Console.WriteLine("Nenhum modo disponível.");
            Pausar();
            return;
        }

        for (int i = 0; i < modos.Count; i++)
        {
            Console.WriteLine($"{i + 1} - {modos[i].Nome}");
        }

        Console.Write("Escolha um modo: ");
        if (!int.TryParse(Console.ReadLine(), out int escolha) ||
            escolha < 1 || escolha > modos.Count)
        {
            Console.WriteLine("Opção inválida!");
            Pausar();
            return;
        }

        await _appService.AquecerModoAsync(modos[escolha - 1].Nome);
        Pausar();
    }
    
    private void Pausar()
    {
        Console.WriteLine("\nPressione qualquer tecla para continuar...");
        Console.ReadKey();
    }
}