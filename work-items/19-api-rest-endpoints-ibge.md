# API REST: Endpoints IBGE

## Parent PRD

**PRD.md**

## What to build

Implementar endpoints REST específicos para consulta de dados do IBGE Censo no projeto `Api`. Endpoints para: população (filtros por município, estado, faixa etária), renda, escolaridade, saneamento, urbanização e infraestrutura. Retornar tanto dados granulares quanto agregados (dashboards). Incluir paginação. Requer autenticação JWT. Incluir testes de integração.

## Acceptance criteria

- [ ] `GET /api/ibge/populacao` com filtros: município, estado, faixa etária
- [ ] `GET /api/ibge/renda` com filtros: município, estado
- [ ] `GET /api/ibge/escolaridade` com filtros: município, estado
- [ ] `GET /api/ibge/saneamento` com filtros: município, estado
- [ ] `GET /api/ibge/urbanizacao` com filtros: município, estado
- [ ] `GET /api/ibge/infraestrutura` com filtros: município, estado
- [ ] Endpoints suportam agregações via query parameter
- [ ] Paginação implementada
- [ ] Todos os endpoints requerem JWT válido
- [ ] Cache headers configurados
- [ ] Testes de integração com dados de teste carregados
- [ ] Todos os testes passam

## Blocked by

- `work-items/07-etl-ingestao-ibge.md`
- `work-items/10-auth-middleware-jwt.md`
- `work-items/16-cache-imemorycache-invalidacao.md`

## User stories addressed

- User story 11
- User story 12
- User story 18
- User story 24
- User story 25

## Type

AFK
