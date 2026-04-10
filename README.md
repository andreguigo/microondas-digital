# Microondas Digital

Documentação técnica do projeto **Microondas Digital**, com visão de arquitetura, instruções de execução e funcionalidades da API/Console.

### 1. Visão geral

A aplicação simula um microondas com as seguintes implementações:

- Aquecimento manual;
- Início rápido (+30s);
- Modos pré-definidos (em `modos.json`);
- Cadastro de programas customizáveis de aquecimento;
- Execução por API HTTP (JWT) e por aplicação de Console.

A aplicação está organizada em camadas de domínio, aplicação, infraestrutura, API e testes unitários. Os projetos usam .NET 8. 

### 2. Arquitetura da aplicação

#### 2.1 Projetos

- `Microondas.Dominio`: entidades e regras centrais (`Aquecimento`, `ModoAquecimento` e enums).  
- `Microondas.Aplicacao`: casos de uso (`Aquecer`, `Início Rápido`, `Listar Modos`, `Aquecer por Modo`, `Cadastrar Programa`) e contratos.  
- `Microondas.Infraestrutura`: repositório de leitura/gravação JSON para modos e controle de pausa/retomada.
- `Microondas.Api`: Minimal API com autenticação JWT, middlewares, logging e endpoints.  
- `Microondas.Console`: interface no terminal para uso local.  
- `Microondas.Testes`: xUnit para domínio e casos de uso.

#### 2.2 Dependências

- API referencia Aplicação + Infraestrutura + Domínio.
- Aplicação referencia Domínio.
- Infraestrutura referencia Domínio + Aplicação.
- Console referencia Aplicação + Infraestrutura.

**Importante:** Essa separação permite trocar interface (API/Console) sem alterar as regras de domínio.

### 3. Regras de negócio

#### 3.1 Aquecimento manual

No modo manual, as validações da entidade `Aquecimento` são:

- Tempo entre **1 e 120 segundos**;
- Potência entre **1 e 10**.

Ao tentar adicionar mais tempo:

- No modo manual é permitido incrementando segundos;
- No modo pré-definido não é permitido lançando exceção de regra.

#### 3.2 Modos pré-definidos

O repositório de modos usa arquivo JSON (`modos.json`) e cria dados padronizados quando o arquivo não existe. Por padrão, inclui modos como Pipoca, Leite, Carne, Frango e Feijão.

No aquecimento por modo (`TipoAquecimento.PreDefinido`), a validação de faixa de tempo/potência da entidade é ignorada porque o modo já vem definido pelo cadastro.

#### 3.3 Cadastro de programas customizáveis

Validações aplicadas:

- `Nome` obrigatório;
- `Alimento` obrigatório;
- `Potencia` de 1 a 10;
- `Tempo` > 0;
- `Caractere` obrigatório e não vazio;
- `Caractere` deve ser único entre os modos já cadastrados.

### 4. API HTTP (Minimal API + JWT)

### 4.1 Autenticação

A API configura JWT Bearer com validação de emissão, audiência, assinatura e expiração. Os dados da sessão de autenticação vêm da seção `SessaoAutenticacao` do `appsettings.json`.

Credenciais de exemplo:

- Usuário: `admin`
- Senha em SHA1: `40bd001563085fc35165329ea1ff5c5ecbdbbeef`

> Observação: o hash acima corresponde à senha "123".

### 4.2 Fluxo para consumir a API - recomendado ;)

1. **Consultar credenciais (mascaradas)**: `GET /api/sessao-autenticacao/credenciais`
2. **Fazer login**: `POST /api/autenticacao/login`
3. Usar `Authorization: Bearer <token>` para os endpoints protegidos.

#### 4.3 Endpoints

##### Sessão/Autenticação

- `GET /api/sessao-autenticacao/credenciais` (público)
- `POST /api/autenticacao/login` (público)
- `GET /api/autenticacao/status` (**autenticado**)

##### Utilização

- `GET /api/utilizacao/modos` (**autenticado**)

##### Aquecimento

- `POST /api/aquecimento/manual` (**autenticado**)
  - body: `{ "tempo": number, "potencia": number? }`
- `POST /api/aquecimento/inicio-rapido` (**autenticado**)
- `POST /api/aquecimento/modo/{nome}` (**autenticado**)

##### Cadastro

- `POST /api/cadastro/programa` (**autenticado**)
  - body: `{ "nome": string, "alimento": string, "potencia": number, "tempo": number, "caractere": char, "instrucoes": string? }`

##### Manutenção

- `GET /api/manutencao/status` (**autenticado**)

#### 4.4 Formato padrão de resposta

A API tem respostas padronizadas:

```json
{
  "sucesso": true,
  "mensagem": "Operação realizada com sucesso.",
  "dados": {}
}
```

Erros de regra retornam `400` e erros inesperados retornam `500`. Exceções não tratadas são registradas em `logs/exceptions.txt`.

### 5. Aplicação Console

A aplicação Console fornece menu para:

1. Aquecimento manual;
2. Início rápido;
3. Seleção de modo pré-definido;
4. Cadastro de programa;
0. Sair.

Durante o aquecimento, os atalhos de teclado são:

- `SPACE`: pausar/retomar;
- `ENTER`: iniciar/adicionar +30s (apenas quando permitido);
- `C`: cancelar.

### 6. Como executar a aplicação

### 6.1 Pré-requisitos

- .NET SDK 8.0+

### 6.2 Restaurar e compilar

```bash
dotnet restore
dotnet build Microondas.sln
```

### 6.3 Rodar API

```bash
dotnet run --project Microondas.Api
```

*Em ambiente de desenvolvimento (Development), o Swagger é habilitado automaticamente.*

### 6.4 Rodar Console

```bash
dotnet run --project Microondas.Console
```

### 6.5 Executar testes

```bash
dotnet test Microondas.Testes
```

ou simplesmente
```bash
dotnet test
```

### 7. Estrutura de arquivos relevante

```text
.
├── Microondas.Api/
├── Microondas.Aplicacao/
├── Microondas.Console/
├── Microondas.Dominio/
├── Microondas.Infraestrutura/
├── Microondas.Testes/
├── modos.json
└── Microondas.sln
```

### 8. Observações técnicas

- O estado de aquecimento é mantido em um serviço singleton (`MicroondasServico`), logo execuções simultâneas compartilham estado na API.
- O repositório de modos persiste diretamente em JSON local; em produção, é bom considerar banco de dados e controle de concorrência de escrita.
- Existe um `Dockerfile` no repositório, mas o estágio final referencia aliases (`base`/`publish`) não definidos no próprio arquivo, exigindo ajuste para build de imagem funcionar como está.
