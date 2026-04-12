# ETL: Ingestão IBGE

## Parent PRD

**PRD.md**

## What to build

Implementar no projeto `Etl` o pipeline de ingestão dos dados do IBGE Censo: leitura dos arquivos brutos (CSVs, formato fixo), transformação para as entities do domínio, e carga no banco no schema `ibge`. Cada dataset do IBGE deve poder ser ingerido independentemente via argumentos de linha de comando. Incluir testes unitários para parsing/transformação e testes de integração para carga.

## Acceptance criteria

- [x] Parsing de arquivos do IBGE implementado (população, renda, escolaridade, saneamento, urbanização, infraestrutura)
- [x] Transformação de dados brutos para entities do domínio
- [ ] Carga no banco no schema `ibge` via repositórios
- [x] Cada dataset pode ser ingerido independentemente
- [x] Ingestão é idempotente
- [x] Testes unitários para parsing e transformação (incluindo edge cases)
- [x] Testes de integração com Testcontainers para carga
- [x] Todos os testes passam

## Blocked by

- `work-items/05-data-dbcontext-schemas-migrations.md`

## User stories addressed

- User story 1
- User story 22

## Type

AFK
