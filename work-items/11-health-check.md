# Health Check

## Parent PRD

**PRD.md**

## What to build

Implementar endpoint `GET /health` no projeto `Api` com readiness check do banco de dados PostgreSQL. O endpoint deve retornar 200 quando o banco está acessível e 503 quando não está. Necessário para o Cloud Run rotear requests corretamente. Incluir testes de integração.

## Acceptance criteria

- [x] Endpoint `GET /health` implementado
- [x] Retorna 200 com body `{ "status": "healthy" }` quando banco acessível
- [x] Retorna 503 com body `{ "status": "unhealthy" }` quando banco inacessível
- [x] Endpoint não requer autenticação
- [x] Testes de integração: banco acessível → 200, banco inacessível → 503
- [x] Todos os testes passam

## Blocked by

- `work-items/05-data-dbcontext-schemas-migrations.md`

## User stories addressed

Nenhuma diretamente — requisito de infraestrutura para Cloud Run.

## Type

AFK
