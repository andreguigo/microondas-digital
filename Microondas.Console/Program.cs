using Microondas.Aplicacao;
using Microondas.Dominio.Servicos;
using Microondas.Dominio.Interfaces;
using Microondas.Infraestrutura.Repositorios;
using Microondas.Infraestrutura.Concorrencias;
using Microondas.Dominio.Enums;

IModoAquecimentoRepositorio repositorio = new ModoAquecimentoRepositorio();
var pause = new PauseTokenSource();
var dominioServico = new MicroondasServico(repositorio, pause);
var appService = new MicroondasAppServico(dominioServico);

var app = new ConsoleApp(appService);

await app.ExecutarAsync();