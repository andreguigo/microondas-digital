using Microondas.Aplicacao;
using Microondas.Aplicacao.Dtos;
using Microondas.Infraestrutura.Repositorios;

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
            Console.WriteLine("4 - Cadastrar programa");
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

                case "4":
                    await CadastrarProgramaAsync();
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

    private async Task CadastrarProgramaAsync()
    {
        Console.Write("Nome do programa: ");
        var nome = Console.ReadLine();

        Console.Write("Alimento: ");
        var alimento = Console.ReadLine();

        Console.Write("Potência (1-10): ");
        if (!int.TryParse(Console.ReadLine(), out var potencia))
        {
            Console.WriteLine("Potência inválida!");
            PausarFluxo();
            return;
        }

        Console.Write("Tempo (segundos): ");
        if (!int.TryParse(Console.ReadLine(), out var tempo))
        {
            Console.WriteLine("Tempo inválido!");
            PausarFluxo();
            return;
        }

        Console.Write("Caractere de aquecimento: ");
        var caractereInput = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(caractereInput) || caractereInput.Length != 1)
        {
            Console.WriteLine("Caractere de aquecimento inválido!");
            PausarFluxo();
            return;
        }

        var caractere = caractereInput[0];

        Console.Write("Instruções adicionais (opcional): ");
        var instrucoes = Console.ReadLine();

        var request = new CadastrarProgramaRequestDto
        {
            Nome = nome,
            Alimento = alimento,
            Potencia = potencia,
            Tempo = tempo,
            Caractere = caractere,
            Instrucoes = instrucoes
        };

        await _appService.CadastrarProgramaAsync(request);

        PausarFluxo();
    }
    
    private static void PausarFluxo()
    {
        Console.WriteLine("\nPressione qualquer tecla para continuar...");
        Console.ReadKey();
    }
}