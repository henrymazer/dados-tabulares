# Views Materializadas para Agentes

## Parent PRD

**PRD.md**

## What to build

Criar views materializadas no PostgreSQL como superfície de consulta para os agentes de IA. Os agentes nunca acessam tabelas reais — apenas views. As views devem cobrir os principais casos de uso: resultados eleitorais agregados, perfil do eleitorado, dados populacionais, dados de renda/emprego, dados de infraestrutura. Incluir migration para criação das views e testes de integração verificando que as views retornam dados corretos após ingestão.

## Acceptance criteria

- [ ] Views materializadas criadas nos schemas `tse`, `ibge`, `pnad`
- [ ] Views TSE: resultados por candidato/partido/município, perfil eleitorado por zona, bens e contas
- [ ] Views IBGE: população por município/UF, renda, escolaridade, saneamento, infraestrutura
- [ ] Views PNAD: desemprego, renda média, informalidade por UF/trimestre
- [ ] Migration para criação das views
- [ ] Script/comando para refresh das views materializadas (chamado pós-ETL)
- [ ] Testes de integração: após ingestão de dados de teste, views retornam dados corretos
- [ ] Todos os testes passam

## Blocked by

- `work-items/06-etl-ingestao-tse.md`
- `work-items/07-etl-ingestao-ibge.md`
- `work-items/08-etl-ingestao-pnad.md`

## User stories addressed

- User story 15
- User story 16

## Type

AFK
