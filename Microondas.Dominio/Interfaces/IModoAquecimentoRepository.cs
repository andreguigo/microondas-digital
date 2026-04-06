using Microondas.Dominio.Entidades;

namespace Microondas.Dominio.Interfaces;

public interface IModoAquecimentoRepositorio
{
    Task<List<ModoAquecimento>> ListarAsync();
}