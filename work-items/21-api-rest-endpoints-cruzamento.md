# API REST: Endpoints de Cruzamento TSE × IBGE

## Parent PRD

**PRD.md**

## What to build

Implementar endpoints REST que cruzam dados eleitorais do TSE com dados socioeconômicos do IBGE, permitindo análises como "desempenho eleitoral × perfil da população por município/zona". Estes endpoints combinam dados de ambos os schemas via views ou queries internas. Requer autenticação JWT. Incluir testes de integração.

## Acceptance criteria

- [ ] `GET /api/cruzamento/resultado-x-populacao` com filtros: município, estado, ano da eleição
- [ ] `GET /api/cruzamento/resultado-x-renda` com filtros: município, estado, ano da eleição
- [ ] `GET /api/cruzamento/eleitorado-x-escolaridade` com filtros: município, zona
- [ ] Endpoints retornam dados combinados de ambas as fontes
- [ ] Paginação implementada
- [ ] Todos os endpoints requerem JWT válido
- [ ] Cache headers configurados
- [ ] Testes de integração com dados de teste de ambas as fontes carregados
- [ ] Todos os testes passam

## Blocked by

- `work-items/18-api-rest-endpoints-tse.md`
- `work-items/19-api-rest-endpoints-ibge.md`

## User stories addressed

- User story 10

## Type

AFK
