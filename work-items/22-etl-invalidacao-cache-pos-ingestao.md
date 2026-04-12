# ETL: Invalidação de Cache Pós-Ingestão

## Parent PRD

**PRD.md**

## What to build

Adicionar ao projeto `Etl` a chamada automática ao endpoint `POST /api/admin/cache/invalidate` ao final de cada pipeline de ingestão, garantindo que os clientes vejam os dados atualizados imediatamente após uma nova carga. Também deve triggar o refresh das views materializadas. Incluir testes unitários com mock do HTTP client.

## Acceptance criteria

- [ ] Ao final da ingestão (qualquer fonte), o ETL chama `POST /api/admin/cache/invalidate`
- [ ] Ao final da ingestão, o ETL triggera refresh das views materializadas
- [ ] URL do endpoint configurável via `appsettings.json` ou argumento de linha de comando
- [ ] Falha na invalidação gera log de warning mas não falha o pipeline
- [ ] Testes unitários com mock do HTTP client
- [ ] Todos os testes passam

## Blocked by

- `work-items/09-etl-upload-brutos-gcs.md`
- `work-items/16-cache-imemorycache-invalidacao.md`

## User stories addressed

- User story 5

## Type

AFK
