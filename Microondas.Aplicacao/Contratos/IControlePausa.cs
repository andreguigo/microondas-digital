namespace Microondas.Aplicacao.Contratos;

public interface IControlePausa
{
    bool EstaPausado { get; }
    Task AguardarSePausadoAsync();
    void Pausar();
    void Retomar();
}