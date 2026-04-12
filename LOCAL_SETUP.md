# Local Setup

The API, ETL, and EF design-time factory now read connection strings from environment variables instead of committed credentials.

## Environment variables

Set the runtime connection strings before running the API or ETL:

```bash
export ConnectionStrings__ReadOnly="Host=localhost;Port=5432;Database=dados_publicos;Username=api_readonly;Password=..."
export ConnectionStrings__ReadWrite="Host=localhost;Port=5432;Database=dados_publicos;Username=api_readwrite;Password=..."
```

Set the design-time connection string before running `dotnet ef`:

```bash
export ConnectionStrings__ReadWrite="Host=localhost;Port=5432;Database=dados_publicos;Username=api_readwrite;Password=..."
```
