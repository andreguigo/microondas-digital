using Microondas.Dominio.Entidades;
using Microondas.Dominio.Enums;

namespace Microondas.Aplicacao.Contratos;

public interface IMicroondasServico
{
    Task AquecerAsync(
        int tempo,
        int? potencia, 
        TipoAquecimento tipo = TipoAquecimento.Manual,
        string? instrucoes = null, 
        char? caractere = null
    );
    Task InicioRapidoAsync();
    Task<List<ModoAquecimento>> ListarModosAsync();
    Task AquecerModoAsync(string nome);
}