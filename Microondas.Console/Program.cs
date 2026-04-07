using Microondas.Aplicacao;
using Microondas.App;
using Microondas.Aplicacao.Servicos;
using Microondas.Infraestrutura.Concorrencias;
using Microondas.Infraestrutura.Repositorios;

var repositorio = new ModoAquecimentoRepositorio("modos.json");
var controlePausa = new ControlePausa();
var servicoMicroondas = new MicroondasServico(repositorio, controlePausa);
var servicoAplicacao = new MicroondasAppServico(servicoMicroondas, repositorio);

var app = new ConsoleApp(servicoAplicacao);

await app.ExecutarAsync();