# Cache: IMemoryCache + Invalidação

## Parent PRD

**PRD.md**

## What to build

Implementar cache usando `IMemoryCache` do .NET para respostas da API (tanto REST quanto query SQL). Cache de query SQL usa hash da query como chave. TTL longo (~7 dias). Endpoint admin `POST /api/admin/cache/invalidate` para invalidação explícita pós-ETL. Cache HTTP com `Cache-Control` headers nos endpoints REST. Incluir testes unitários.

## Acceptance criteria

- [ ] Cache implementado com `IMemoryCache`
- [ ] Queries SQL cacheadas com chave = hash do SQL
- [ ] TTL padrão de 7 dias
- [ ] Endpoint `POST /api/admin/cache/invalidate` limpa todo o cache
- [ ] Endpoint de invalidação requer autenticação (apenas admin)
- [ ] Headers `Cache-Control` nos endpoints REST
- [ ] Cache miss executa query e armazena resultado
- [ ] Cache hit retorna resultado sem executar query
- [ ] Testes unitários: hit/miss, TTL respeitado, invalidação limpa tudo, hash consistente
- [ ] Todos os testes passam

## Blocked by

- `work-items/15-api-query-execucao-sql.md`

## User stories addressed

- User story 5
- User story 20

## Type

AFK
