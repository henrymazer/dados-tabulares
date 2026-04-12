# ETL: Ingestão PNAD

## Parent PRD

**PRD.md**

## What to build

Implementar no projeto `Etl` o pipeline de ingestão dos dados da PNAD Contínua: leitura dos arquivos brutos trimestrais, transformação para as entities do domínio, e carga no banco no schema `pnad`. Deve poder ser executado independentemente das outras fontes. Incluir testes unitários para parsing/transformação e testes de integração para carga.

## Acceptance criteria

- [x] Parsing de arquivos da PNAD Contínua implementado (desemprego, renda média, informalidade)
- [x] Transformação de dados brutos para entities do domínio
- [ ] Carga no banco no schema `pnad` via repositórios
- [x] Ingestão pode ser executada independentemente (`--source pnad`)
- [x] Ingestão é idempotente (atualiza trimestres existentes sem duplicar)
- [x] Testes unitários para parsing e transformação
- [x] Testes de integração com Testcontainers para carga
- [x] Todos os testes passam

## Blocked by

- `work-items/05-data-dbcontext-schemas-migrations.md`

## User stories addressed

- User story 3
- User story 22

## Type

AFK
