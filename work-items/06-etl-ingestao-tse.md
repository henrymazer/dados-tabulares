# ETL: Ingestão TSE

## Parent PRD

**PRD.md**

## What to build

Implementar no projeto `Etl` o pipeline de ingestão dos dados do TSE: leitura dos CSVs brutos do TSE (resultados eleitorais, perfil do eleitorado, bens declarados, prestação de contas, coligações), transformação para as entities do domínio, e carga no banco no schema `tse`. Cada fonte do TSE deve poder ser ingerida independentemente via argumentos de linha de comando. Incluir testes unitários para parsing/transformação e testes de integração para carga.

## Acceptance criteria

- [x] Parsing de CSVs do TSE implementado (resultados, eleitorado, bens, contas, coligações)
- [x] Transformação de dados brutos para entities do domínio
- [ ] Carga no banco no schema `tse` via repositórios
- [x] Cada dataset do TSE pode ser ingerido independentemente (`--source resultados`, `--source eleitorado`, etc.)
- [x] Ingestão é idempotente (re-executar não duplica dados)
- [x] Testes unitários para parsing e transformação (incluindo dados faltantes, formatos inesperados)
- [x] Testes de integração com Testcontainers para carga
- [x] Todos os testes passam

## Blocked by

- `work-items/05-data-dbcontext-schemas-migrations.md`

## User stories addressed

- User story 2
- User story 22

## Type

AFK
