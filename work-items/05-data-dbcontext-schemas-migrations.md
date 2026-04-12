# Data: DbContext, Schemas e Migrations

## Parent PRD

**PRD.md**

## What to build

Criar o `DbContext` no projeto `Data` com configuração de schemas separados (`ibge`, `tse`, `pnad`), mapeamento das entities via Fluent API, e migrations iniciais. Configurar duas connection strings: uma read-write (para ETL/migrations) e uma read-only (para API). Implementar os repositórios concretos. Incluir testes de integração com Testcontainers verificando que schemas são criados corretamente e repositórios funcionam.

## Acceptance criteria

- [x] `DbContext` configurado com schemas separados: `ibge`, `tse`, `pnad`
- [x] Todas as entities dos slices 02, 03, 04 mapeadas via Fluent API
- [x] Migrations iniciais criadas e aplicáveis
- [x] Repositórios concretos implementados
- [x] Duas connection strings configuradas (read-write e read-only)
- [x] Testes de integração com Testcontainers: migrations aplicam, schemas existem, CRUD básico funciona
- [x] Todos os testes passam

## Blocked by

- `work-items/02-domain-entities-tse.md`
- `work-items/03-domain-entities-ibge.md`
- `work-items/04-domain-entities-pnad.md`

## User stories addressed

Nenhuma diretamente — este é o slice de infraestrutura de dados.

## Type

AFK
