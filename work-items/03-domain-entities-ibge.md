# Domain: Entities e Models do IBGE

## Parent PRD

**PRD.md**

## What to build

Criar as entities e value objects do domínio IBGE no projeto `Domain`, modelando os dados do Censo: população por faixa etária, renda, escolaridade, raça, saneamento, urbanização e infraestrutura, com granularidade por município e estado. Definir as interfaces dos repositórios. Incluir testes unitários para validações e transformações.

## Acceptance criteria

- [x] Entities criadas: `DadoPopulacional`, `DadoRenda`, `DadoEscolaridade`, `DadoSaneamento`, `DadoUrbanizacao`, `DadoInfraestrutura`
- [x] Value objects reutilizados onde possível (ex: `Municipio`, `UF` do slice TSE)
- [x] Interfaces de repositório definidas para cada aggregate root
- [x] Testes unitários para validações de entities
- [x] Todos os testes passam

## Blocked by

- `work-items/01-setup-solution-infraestrutura-base.md`

## User stories addressed

- User story 1
- User story 12
- User story 24
- User story 25

## Type

AFK
