# Domain: Entities e Models da PNAD

## Parent PRD

**PRD.md**

## What to build

Criar as entities e value objects do domínio PNAD Contínua no projeto `Domain`, modelando os dados trimestrais: taxa de desemprego, renda média, informalidade, por UF e trimestre. Definir as interfaces dos repositórios. Incluir testes unitários para validações e transformações.

## Acceptance criteria

- [x] Entities criadas: `DadoDesemprego`, `DadoRendaMedia`, `DadoInformalidade`
- [x] Value objects: `Trimestre` (ano + trimestre), reutilizar `UF`
- [x] Interfaces de repositório definidas
- [x] Testes unitários para validações de entities
- [x] Todos os testes passam

## Blocked by

- `work-items/01-setup-solution-infraestrutura-base.md`

## User stories addressed

- User story 3
- User story 14

## Type

AFK
