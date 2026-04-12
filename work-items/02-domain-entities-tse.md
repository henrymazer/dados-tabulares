# Domain: Entities e Models do TSE

## Parent PRD

**PRD.md**

## What to build

Criar as entities e value objects do domínio TSE no projeto `Domain`, modelando os dados eleitorais: resultados por candidato/partido/zona/seção, perfil do eleitorado (faixa etária, escolaridade, gênero por zona), bens declarados, prestação de contas e coligações. Definir as interfaces dos repositórios (`IResultadoEleitoral`, `IPerfilEleitorado`, `IBensDeclarados`, `IPrestacaoContas`). Incluir testes unitários para validações e transformações.

## Acceptance criteria

- [x] Entities criadas: `ResultadoEleitoral`, `Candidato`, `Partido`, `Coligacao`, `PerfilEleitorado`, `BemDeclarado`, `PrestacaoContas`, `ZonaEleitoral`, `SecaoEleitoral`
- [x] Value objects criados onde aplicável (ex: `Municipio`, `UF`, `AnoEleicao`)
- [x] Interfaces de repositório definidas para cada aggregate root
- [x] Testes unitários para validações de entities (dados obrigatórios, ranges válidos)
- [x] Todos os testes passam

## Blocked by

- `work-items/01-setup-solution-infraestrutura-base.md`

## User stories addressed

- User story 2
- User story 8
- User story 9
- User story 13
- User story 23

## Type

AFK
