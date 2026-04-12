# API Metadata: Endpoint GET /api/metadata

## Parent PRD

**PRD.md**

## What to build

Implementar endpoint `GET /api/metadata` no projeto `Api` que retorna a lista de views materializadas disponíveis para consulta, com suas colunas, tipos de dados e descrições em linguagem natural. Este endpoint é usado pelos agentes de IA como contexto antes de gerar SQL. Incluir testes unitários e de integração.

## Acceptance criteria

- [ ] Endpoint `GET /api/metadata` implementado
- [ ] Retorna lista de views com: nome, schema, colunas (nome, tipo, descrição)
- [ ] Descrições em linguagem natural (ex: "Taxa de desemprego por UF e trimestre")
- [ ] Requer autenticação JWT
- [ ] Response cacheável (dados mudam apenas em ingestão)
- [ ] Testes unitários: formato de resposta correto, descrições não vazias
- [ ] Testes de integração: metadata reflete views reais do banco
- [ ] Todos os testes passam

## Blocked by

- `work-items/12-views-materializadas.md`

## User stories addressed

- User story 15

## Type

AFK
