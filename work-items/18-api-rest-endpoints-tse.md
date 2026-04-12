# API REST: Endpoints TSE

## Parent PRD

**PRD.md**

## What to build

Implementar endpoints REST específicos para consulta de dados do TSE no projeto `Api`. Endpoints para: resultados eleitorais (filtros por município, zona, partido, ano, candidato), perfil do eleitorado (filtros por zona, faixa etária, escolaridade, gênero), bens declarados e prestação de contas. Retornar tanto dados granulares quanto agregados. Incluir paginação. Requer autenticação JWT. Incluir testes de integração.

## Acceptance criteria

- [ ] `GET /api/tse/resultados` com filtros: município, zona, partido, ano, candidato
- [ ] `GET /api/tse/eleitorado` com filtros: zona, município, faixa etária, escolaridade, gênero
- [ ] `GET /api/tse/bens` com filtros: candidato, ano
- [ ] `GET /api/tse/prestacao-contas` com filtros: candidato, partido, ano
- [ ] Endpoints suportam agregações (totais, médias) via query parameter
- [ ] Paginação implementada (offset/limit ou cursor)
- [ ] Todos os endpoints requerem JWT válido
- [ ] Cache headers configurados
- [ ] Testes de integração com dados de teste carregados
- [ ] Todos os testes passam

## Blocked by

- `work-items/06-etl-ingestao-tse.md`
- `work-items/10-auth-middleware-jwt.md`
- `work-items/16-cache-imemorycache-invalidacao.md`

## User stories addressed

- User story 8
- User story 9
- User story 13
- User story 18
- User story 23

## Type

AFK
