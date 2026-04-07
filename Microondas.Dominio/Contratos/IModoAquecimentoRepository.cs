using Microondas.Dominio.Entidades;

namespace Microondas.Dominio;

public interface IModoAquecimentoRepositorio
{
    Task<List<ModoAquecimento>> ListarAsync();
}