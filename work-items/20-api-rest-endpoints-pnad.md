# API REST: Endpoints PNAD

## Parent PRD

**PRD.md**

## What to build

Implementar endpoints REST específicos para consulta de dados da PNAD Contínua no projeto `Api`. Endpoints para: desemprego, renda média e informalidade, com filtros por UF e trimestre. Retornar tanto dados granulares quanto séries temporais. Incluir paginação. Requer autenticação JWT. Incluir testes de integração.

## Acceptance criteria

- [ ] `GET /api/pnad/desemprego` com filtros: UF, trimestre, ano
- [ ] `GET /api/pnad/renda` com filtros: UF, trimestre, ano
- [ ] `GET /api/pnad/informalidade` com filtros: UF, trimestre, ano
- [ ] Endpoints suportam séries temporais (retornar vários trimestres para visualização de tendência)
- [ ] Paginação implementada
- [ ] Todos os endpoints requerem JWT válido
- [ ] Cache headers configurados
- [ ] Testes de integração com dados de teste carregados
- [ ] Todos os testes passam

## Blocked by

- `work-items/08-etl-ingestao-pnad.md`
- `work-items/10-auth-middleware-jwt.md`
- `work-items/16-cache-imemorycache-invalidacao.md`

## User stories addressed

- User story 14
- User story 18

## Type

AFK
