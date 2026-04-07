using System.Text.Json;
using Microondas.Dominio.Entidades;
using Microondas.Dominio;

namespace Microondas.Infraestrutura.Repositorios;

public class ModoAquecimentoRepositorio : IModoAquecimentoRepositorio
{
    private const string FilePath = "modos.json";

    public async Task<List<ModoAquecimento>> ListarAsync()
    {
        if (!File.Exists(FilePath))
            await CriarPadraoAsync();

        var json = await File.ReadAllTextAsync(FilePath);

        return JsonSerializer.Deserialize<List<ModoAquecimento>>(json)!;
    }

    private async Task CriarPadraoAsync()
    {
        var modos = new List<ModoAquecimento>
        {
            new("Pipoca", "Pipoca", 7, 180, '*', "Observar o barulho de estouros do milho, caso houver um intervalo de mais de 10 segundos entre um estouro e outro, interrompa o aquecimento."),
            new("Leite", "Leite", 5, 300, '*', "Cuidado com aquecimento de líquidos, o choque térmico aliado ao movimento do recipiente pode causar fervura imediata causando risco de queimaduras."),
            new("Carne", "Carne", 4, 840, '*', "Interrompa o processo na metade e vire o conteúdo com a parte de baixo para cima para o descongelamento uniforme."),
            new("Frango", "Frango", 7, 480, '*', "Interrompa o processo na metade e vire o conteúdo com a parte de baixo para cima para o descongelamento uniforme."),
            new("Feijão", "Feijão", 9, 480, '*', "Deixe o recipiente destampado e em casos de plástico, cuidado ao retirar o recipiente pois o mesmo pode perder resistência em altas temperaturas.")
        };

        var json = JsonSerializer.Serialize(modos, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        await File.WriteAllTextAsync(FilePath, json);
    }
}