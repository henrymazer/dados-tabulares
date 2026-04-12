# API Query: Validação SQL

## Parent PRD

**PRD.md**

## What to build

Implementar o módulo de validação SQL no projeto `Api` que parseia e valida queries SQL recebidas dos agentes de IA antes da execução. Deve garantir defesa em profundidade: só permitir `SELECT`, bloquear operações perigosas, aplicar whitelist de views e forçar `LIMIT`. Este módulo é o componente de segurança mais crítico do serviço. Incluir testes unitários extensivos.

## Acceptance criteria

- [x] Parser SQL implementado que analisa a query recebida
- [x] Aceita apenas statements `SELECT`
- [x] Rejeita `INSERT`, `UPDATE`, `DELETE`, `DROP`, `CREATE`, `ALTER`, `TRUNCATE`
- [x] Rejeita `SELECT ... INTO`
- [x] Rejeita CTEs com side effects (`INSERT`, `UPDATE`, `DELETE` dentro de CTE)
- [x] Whitelist de views/tabelas: só permite acesso a views dos schemas `ibge`, `tse`, `pnad`
- [x] Se a query não tem `LIMIT`, adiciona `LIMIT 10000` automaticamente
- [x] Se a query tem `LIMIT > 10000`, reduz para `LIMIT 10000`
- [x] Retorna erro claro e descritivo quando query é rejeitada
- [x] Testes unitários extensivos cobrindo: queries válidas, cada tipo de statement bloqueado, SQL injection patterns, edge cases (subqueries, unions, CTEs válidas)
- [x] Todos os testes passam

## Blocked by

- `work-items/01-setup-solution-infraestrutura-base.md`

## User stories addressed

- User story 6
- User story 16
- User story 17
- User story 21

## Type

AFK
