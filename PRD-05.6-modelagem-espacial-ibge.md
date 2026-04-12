# PRD — Modelagem Espacial IBGE

## Problem Statement

O IBGE possui datasets geográficos que precisam de uma representação estável em PostGIS antes da ingestão. Sem um modelo espacial canônico, a próxima etapa vai precisar decidir de forma ad hoc sobre tipo de geometria, SRID, nomes de tabelas e índices, o que tende a gerar retrabalho e migrations instáveis.

## Solution

Definir a modelagem espacial do IBGE no schema `ibge`, com uma entidade geográfica canônica para setores censitários e os vínculos necessários com os identificadores municipais já existentes. O slice fixa a estrutura do dado espacial, o SRID e os índices, mas não ingere arquivos nem expõe endpoints.

## User Stories

1. Como desenvolvedor, quero um modelo espacial canônico para o IBGE, para que o ETL futuro persista geometrias sem refatoração estrutural.
2. Como desenvolvedor, quero que o tipo de geometria e o SRID fiquem definidos nas migrations, para que os dados espaciais sejam armazenados de forma consistente.
3. Como desenvolvedor, quero índices espaciais nas tabelas do IBGE, para que consultas geográficas futuras tenham desempenho adequado.
4. Como analista, quero que a geometria do IBGE esteja vinculada aos identificadores municipais existentes, para que os dados espaciais possam ser cruzados com o restante do domínio.
5. Como mantenedor, quero validar as migrations espaciais em um banco local com PostGIS, para detectar incompatibilidades antes da ingestão.

## Implementation Decisions

- Usar o schema `ibge` como dono da modelagem espacial do IBGE.
- Modelar `setores_censitarios` como a primeira entidade geográfica canônica.
- Persistir a geometria como `MultiPolygon` em SRID `4674`.
- Manter os vínculos com município/UF alinhados aos identificadores já usados no modelo atual.
- Criar índices B-tree por município/UF e índice espacial GiST na coluna geométrica.
- Não incluir ingestão de arquivos geográficos nem endpoints REST neste slice.

## Testing Decisions

- Validar as migrations contra um banco local real com PostGIS.
- Verificar que o modelo persiste e lê colunas espaciais com o provider atual do Npgsql.
- Focar testes no comportamento externo do schema e da persistência, não em detalhes internos do EF.

## Out of Scope

- Ingestão/parsing dos arquivos geográficos do IBGE
- Modelagem espacial do TSE
- Endpoints REST ou query SQL espacial
- Cruzamentos entre IBGE e TSE
- Configuração de ambiente local

## Further Notes

Este slice vem depois do banco local com PostGIS e antes da ingestão espacial do IBGE.
