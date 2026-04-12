# Banco local com PostGIS

Suba o banco local de desenvolvimento com:

```bash
docker compose -f infra/local-db/compose.yaml up -d
```

O container expõe PostgreSQL + PostGIS em `localhost:5432` com:
O container expõe PostgreSQL + PostGIS em `localhost:5433` com:

- Database: `dados_publicos_dev`
- Username: `postgres`
- Password: `postgres`

Enquanto a separação local de usuários `ReadOnly` e `ReadWrite` não for automatizada, a aplicação pode apontar ambas as connection strings para a mesma instância local:

```bash
ConnectionStrings__ReadOnly=Host=localhost;Port=5433;Database=dados_publicos_dev;Username=postgres;Password=postgres
ConnectionStrings__ReadWrite=Host=localhost;Port=5433;Database=dados_publicos_dev;Username=postgres;Password=postgres
```

Para verificar se o PostGIS foi habilitado:

```bash
docker compose -f infra/local-db/compose.yaml exec postgres psql -U postgres -d dados_publicos_dev -c "SELECT PostGIS_Version();"
```
