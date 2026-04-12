# ETL: Upload de Arquivos Brutos para GCS

## Parent PRD

**PRD.md**

## What to build

Adicionar ao projeto `Etl` a funcionalidade de upload dos arquivos brutos originais (antes da transformação) para um bucket no Google Cloud Storage como backup. O upload deve acontecer como parte do pipeline de ingestão, organizando os arquivos por fonte e data (ex: `tse/2024/`, `ibge/censo-2022/`, `pnad/2025-q1/`). Incluir testes unitários com mock do client GCS.

## Acceptance criteria

- [ ] Upload dos arquivos brutos para GCS implementado
- [ ] Arquivos organizados por fonte e data no bucket
- [ ] Upload acontece automaticamente durante o pipeline de ingestão
- [ ] Falha no upload não impede a ingestão no banco (log de warning, continua)
- [ ] Testes unitários com mock do client GCS
- [ ] Todos os testes passam

## Blocked by

- `work-items/06-etl-ingestao-tse.md`
- `work-items/07-etl-ingestao-ibge.md`
- `work-items/08-etl-ingestao-pnad.md`

## User stories addressed

- User story 4

## Type

AFK
