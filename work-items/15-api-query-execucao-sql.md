# API Query: Execução SQL com User Read-Only

## Parent PRD

**PRD.md**

## What to build

Implementar o endpoint `POST /api/query` que recebe SQL como string, passa pela validação (slice 14), executa contra o PostgreSQL usando o user read-only, e retorna resultados em JSON. Configurar timeout de execução para queries anômalas. Incluir testes de integração com Testcontainers.

## Acceptance criteria

- [ ] Endpoint `POST /api/query` implementado, recebe `{ "sql": "SELECT ..." }`
- [ ] Integração com módulo de validação SQL (slice 14) — query inválida retorna 400
- [ ] Execução via connection string read-only (user PostgreSQL sem permissão de escrita)
- [ ] Resultados retornados em JSON: `{ "columns": [...], "rows": [...], "rowCount": N }`
- [ ] Timeout de execução configurável (default: 60s) — query que excede retorna 408
- [ ] Requer autenticação JWT
- [ ] Testes de integração: query válida retorna dados, query inválida retorna 400, user read-only não consegue INSERT/UPDATE/DELETE, timeout funciona
- [ ] Todos os testes passam

## Blocked by

- `work-items/12-views-materializadas.md`
- `work-items/14-api-query-validacao-sql.md`

## User stories addressed

- User story 16
- User story 17

## Type

AFK
