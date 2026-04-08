using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microondas.Aplicacao.Contratos;
using Microondas.Aplicacao.Dtos;
using Microondas.Aplicacao.Servicos;
using Microondas.Aplicacao.UseCases;
using Microondas.Dominio;
using Microondas.Infraestrutura.Concorrencias;
using Microondas.Infraestrutura.Repositorios;
using Microondas.Api.Auth;
using Microondas.Api.Exceptions;
using Microondas.Api.Logging;
using Microondas.Api.Middlewares;
using Microondas.Api.Modelos;
using Microondas.Api.Servicos;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<SessaoAutenticacaoOptions>(builder.Configuration.GetSection(SessaoAutenticacaoOptions.SectionName));
var authConfig = builder.Configuration.GetSection(SessaoAutenticacaoOptions.SectionName).Get<SessaoAutenticacaoOptions>()
    ?? new SessaoAutenticacaoOptions();

var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authConfig.ChaveJwt));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = authConfig.Emissor,
            ValidAudience = authConfig.Audiencia,
            IssuerSigningKey = key,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Sua API", Version = "v1" });

    // Configurar o uso de JWT Bearer no Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Informe o token JWT no formato: Bearer {seu_token}",
        Name = "Authorization",
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddSingleton<IModoAquecimentoRepositorio>(_ => new ModoAquecimentoRepositorio("modos.json"));
builder.Services.AddSingleton<IControlePausa, ControlePausa>();
builder.Services.AddSingleton<IMicroondasServico, MicroondasServico>();
builder.Services.AddSingleton<IExceptionTextLogger, ExceptionTextLogger>();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

var sessaoGroup = app.MapGroup("/api/sessao-autenticacao").WithTags("Sessão de Autenticação");

sessaoGroup.MapGet("/credenciais", (IConfiguration configuration) =>
{
    var sessao = configuration.GetSection(SessaoAutenticacaoOptions.SectionName).Get<SessaoAutenticacaoOptions>()
        ?? new SessaoAutenticacaoOptions();

    var response = new SessaoCredencialResponse
    {
        Usuario = sessao.Usuario,
        SenhaMascarada = Sha1HashService.Mask(sessao.SenhaSha1),
        AlgoritmoHash = "SHA1"
    };

    return Results.Ok(ApiResponse<SessaoCredencialResponse>.Ok(response, "Credenciais carregadas da sessão específica."));
});

app.MapPost("/api/autenticacao/login", (AutenticacaoRequest request, IConfiguration configuration) =>
{
    var sessao = configuration.GetSection(SessaoAutenticacaoOptions.SectionName).Get<SessaoAutenticacaoOptions>()
        ?? throw new RegraNegocioException("Sessão de autenticação não configurada.");

    if (!request.Usuario.Equals(sessao.Usuario, StringComparison.OrdinalIgnoreCase)
        || !Sha1HashService.EqualsHash(request.Senha, sessao.SenhaSha1))
    {
        return Results.Unauthorized();
    }

    var expires = DateTime.UtcNow.AddMinutes(sessao.ExpiracaoMinutos);

    var token = new JwtSecurityToken(
        issuer: sessao.Emissor,
        audience: sessao.Audiencia,
        claims: new[] { new Claim(ClaimTypes.Name, request.Usuario) },
        expires: expires,
        signingCredentials: new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(sessao.ChaveJwt)),
            SecurityAlgorithms.HmacSha256));

    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

    return Results.Ok(ApiResponse<TokenResponse>.Ok(new TokenResponse
    {
        Autenticado = true,
        Token = tokenString,
        ExpiraEmUtc = expires
    }, "Autenticação realizada com sucesso."));
}).WithTags("Autenticação");

app.MapGet("/api/autenticacao/status", [Microsoft.AspNetCore.Authorization.Authorize] () =>
        Results.Ok(ApiResponse<object>.Ok(new { autenticado = true }, "Autenticação válida.")))
    .WithTags("Autenticação");

var utilizacaoGroup = app.MapGroup("/api/utilizacao").RequireAuthorization().WithTags("Utilização");
utilizacaoGroup.MapGet("/modos", async (IMicroondasServico servico) =>
{
    var useCase = new ListarModosUseCase(servico);
    var resultado = await useCase.ExecutarAsync();
    return Results.Ok(ApiResponse<List<ModoDto>>.Ok(resultado));
});

var aquecimentoGroup = app.MapGroup("/api/aquecimento").RequireAuthorization().WithTags("Aquecimento");
aquecimentoGroup.MapPost("/manual", async (AquecerRequestDto request, IMicroondasServico servico) =>
{
    if (request.Tempo <= 0)
        throw new RegraNegocioException("Tempo deve ser maior que zero.");

    if (request.Potencia is < 1 or > 10)
        throw new RegraNegocioException("Potência deve estar entre 1 e 10.");

    await new AquecerUseCase(servico).ExecutarAsync(request);
    return Results.Ok(ApiResponse<string>.Ok("Aquecimento manual iniciado."));
});

aquecimentoGroup.MapPost("/inicio-rapido", async (IMicroondasServico servico) =>
{
    await new InicioRapidoUseCase(servico).ExecutarAsync();
    return Results.Ok(ApiResponse<string>.Ok("Início rápido executado."));
});

aquecimentoGroup.MapPost("/modo/{nome}", async (string nome, IMicroondasServico servico) =>
{
    if (string.IsNullOrWhiteSpace(nome))
        throw new RegraNegocioException("Nome do modo é obrigatório.");

    await new AquecerModoUseCase(servico).ExecutarAsync(nome);
    return Results.Ok(ApiResponse<string>.Ok("Aquecimento por modo executado."));
});

var cadastroGroup = app.MapGroup("/api/cadastro").RequireAuthorization().WithTags("Cadastro");
cadastroGroup.MapPost("/programa", async (CadastrarProgramaRequestDto request, IModoAquecimentoRepositorio repositorio) =>
{
    var result = await new CadastrarProgramaUseCase(repositorio).ExecutarAsync(request);

    if (!result.EhSucesso)
        throw new RegraNegocioException(result.Mensagem);

    return Results.Ok(ApiResponse<string>.Ok(result.Mensagem));
});

var manutencaoGroup = app.MapGroup("/api/manutencao").RequireAuthorization().WithTags("Manutenção");
manutencaoGroup.MapGet("/status", () =>
    Results.Ok(ApiResponse<object>.Ok(new
    {
        sistema = "Microondas.Api",
        versao = "1.0.0",
        utc = DateTime.UtcNow
    }, "Serviço operacional.")));

app.Run();
