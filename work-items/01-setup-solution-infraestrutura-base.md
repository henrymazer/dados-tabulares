# Setup da Solution e Infraestrutura Base

## Parent PRD

**PRD.md**

## What to build

Criar a solution .NET 10 com a estrutura de projetos definida no PRD: `Api`, `Etl`, `Domain`, `Data` e projetos de teste correspondentes (`Domain.Tests`, `Data.Tests`, `Api.Tests`, `Etl.Tests`). Configurar o `Api` como web API com Serilog, o `Etl` como console app, e o `Domain` e `Data` como class libraries. Adicionar os pacotes base: Npgsql/EF Core para PostgreSQL, xUnit + Testcontainers para testes, Serilog para logging. Configurar `appsettings.json` com placeholders para connection strings (read-only e read-write). Incluir `Dockerfile` para o projeto `Api` otimizado para Cloud Run.

## Acceptance criteria

- [x] Solution .NET 10 criada com projetos `Api`, `Etl`, `Domain`, `Data`
- [x] Projetos de teste criados: `Domain.Tests`, `Data.Tests`, `Api.Tests`, `Etl.Tests`
- [x] Referências entre projetos configuradas (Api → Domain, Data; Etl → Domain, Data; Data → Domain)
- [x] Pacotes NuGet base instalados (Npgsql.EntityFrameworkCore.PostgreSQL, Serilog, xUnit, Testcontainers)
- [x] `Api` configurado como web API mínima com Serilog
- [x] `Etl` configurado como console app
- [x] `Dockerfile` para o projeto `Api` funcional
- [x] `dotnet build` compila sem erros
- [x] `dotnet test` roda sem erros (mesmo sem testes ainda)

## Blocked by

None - can start immediately

## User stories addressed

Nenhuma diretamente — este é o slice de fundação.

## Type

AFK
