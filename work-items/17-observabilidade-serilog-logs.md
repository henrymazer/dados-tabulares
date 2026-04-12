# Observabilidade: Serilog + Logs de Query

## Parent PRD

**PRD.md**

## What to build

Configurar logging estruturado com Serilog no projeto `Api`, otimizado para Cloud Logging do Google. Logar toda query SQL executada com: query, tempo de execução, tenant (do JWT), rows retornados. Logar queries rejeitadas pela validação com nível Warning/Error. Configurar alertas básicos: queries rejeitadas e queries acima de 60 segundos. Incluir testes unitários verificando que os logs são emitidos corretamente.

## Acceptance criteria

- [ ] Serilog configurado com sink para console (JSON estruturado, compatível com Cloud Logging)
- [ ] Toda query SQL executada é logada: query, duração, tenant, rowCount
- [ ] Queries rejeitadas pela validação logadas como Warning com motivo da rejeição
- [ ] Queries acima de 60 segundos logadas como Warning
- [ ] Informações sensíveis não são logadas (tokens, connection strings)
- [ ] Testes unitários verificando emissão dos logs nos cenários corretos
- [ ] Todos os testes passam

## Blocked by

- `work-items/15-api-query-execucao-sql.md`

## User stories addressed

- User story 6
- User story 7
- User story 21

## Type

AFK
