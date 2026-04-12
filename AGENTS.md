# Repository Guidelines

## Project Structure & Module Organization
`DadosTabulares.slnx` groups four production projects under `src/` and matching test projects under `tests/`.

- `src/Api`: ASP.NET Core API entrypoint, auth, health checks, and query validation.
- `src/Data`: EF Core `DbContext`, repository implementations, and migrations.
- `src/Domain`: core domain models for TSE, IBGE, and PNAD.
- `src/Etl`: console ETL pipelines and parsers for each dataset.
- `tests/*`: xUnit test suites aligned to each production project.
- `work-items/` and `PRD.md`: planning artifacts; update them only when scope changes.

## Build, Test, and Development Commands
- `dotnet restore DadosTabulares.slnx`: restore solution dependencies.
- `dotnet build src/Api/Api.csproj -m:1`: preferred Codex build command; compile the API project with a single MSBuild worker.
- `dotnet test DadosTabulares.slnx`: run the full test suite.
- `dotnet run --project src/Api/Api.csproj`: start the API locally.
- `dotnet run --project src/Etl/Etl.csproj -- --help`: inspect ETL commands.
- `dotnet ef database update --project src/Data/Data.csproj`: apply EF migrations.

Local environment variables and connection-string setup live in `LOCAL_SETUP.md`. Keep setup details there instead of duplicating them in this guide.

Codex may run tests for this repository. When build validation is needed, prefer `dotnet build src/Api/Api.csproj -m:1` instead of building the full solution; this is the supported Codex build path for the current environment.

## Coding Style & Naming Conventions
Use standard C# formatting: 4-space indentation, file-scoped namespaces, PascalCase for types and methods, camelCase for parameters, and one public type per file. Nullable reference types are enabled across projects; treat new warnings as real issues. Keep module boundaries intact: domain rules belong in `Domain`, persistence details in `Data`, transport concerns in `Api`, and ingestion flow in `Etl`.

## Testing Guidelines
Tests use xUnit. Name test files after the subject type, for example `CandidatoTests.cs` or `SqlQueryValidatorTests.cs`. Existing tests use descriptive Portuguese method names such as `Deve_criar_candidato_valido`; follow that pattern. Integration tests use Testcontainers with PostgreSQL, so Docker must be available when running suites in `Api.Tests`, `Data.Tests`, or ETL integration tests.

## Commit & Pull Request Guidelines
Recent history uses short, imperative subjects, often with prefixes like `feat:`. Keep commits focused and descriptive, for example `feat: add PNAD ingestion parser`. PRs should explain the affected module, summarize behavior changes, link the relevant work item or issue, and note schema or environment-variable changes. Include sample requests or logs when API or ETL behavior changes.
