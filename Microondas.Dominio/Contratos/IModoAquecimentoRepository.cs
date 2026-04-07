using Microondas.Dominio.Entidades;

namespace Microondas.Dominio;

public interface IModoAquecimentoRepositorio
{
    Task<List<ModoAquecimento>> ListarAsync();
    Task<bool> CaractereExisteAsync(char caractere);
    Task AdicionarAsync(ModoAquecimento modo);
}