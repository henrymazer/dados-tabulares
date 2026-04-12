## Problem Statement

O projeto precisa de um banco de desenvolvimento local previsível para migrations, ETL e validação manual. O PostgreSQL puro cobre o estado atual, mas os próximos datasets do IBGE e do TSE exigirão suporte geoespacial por causa de setores censitários, geometrias administrativas e endereços/locais de votação com potencial uso espacial.

Sem uma base local com PostGIS desde agora, a equipe tende a adiar uma dependência estrutural do modelo de dados e corre o risco de descobrir incompatibilidades só quando começar a persistir colunas espaciais.

## Solution

Padronizar um banco local Dockerizado com PostgreSQL + PostGIS para desenvolvimento, mantendo o código da aplicação pronto para usar o provider espacial do EF Core/Npgsql quando as primeiras colunas geométricas forem introduzidas.

O slice cobre apenas infraestrutura local e preparação do provider. Ele não adiciona ainda entidades espaciais, colunas geometry/geography, nem endpoints geográficos.

## User Stories

1. Como desenvolvedor, quero subir um banco local Postgres com PostGIS em um comando, para validar migrations e ETLs sem depender de infraestrutura externa.
2. Como desenvolvedor, quero ter uma configuração local previsível para o banco, para reduzir erro de ambiente entre máquina local e futuras evoluções geográficas do projeto.
3. Como desenvolvedor, quero que o `DbContext` já esteja preparado para o provider espacial do Npgsql, para evitar refactors desnecessários quando surgirem colunas geométricas.
4. Como mantenedor do ETL, quero um banco local persistente entre reinícios do container, para poder testar cargas incrementais e inspeção manual dos dados.
5. Como mantenedor do domínio de dados, quero o PostGIS habilitado no banco local desde já, para que migrations futuras com geometry/geography não dependam de troca de engine no meio do projeto.

## Implementation Decisions

- Adicionar uma infraestrutura local versionada para subir um container PostgreSQL com a extensão PostGIS habilitada.
- Manter a persistência do banco em volume Docker nomeado para facilitar ciclos de desenvolvimento.
- Documentar credenciais e comando de subida de forma objetiva junto da infraestrutura local.
- Preparar o `DbContext` e a factory de design-time para usar o provider NetTopologySuite do Npgsql, mesmo antes da existência de colunas espaciais.
- Não alterar ainda o modelo de domínio, migrations existentes, repositórios ou endpoints para incluir geometrias.
- Não mudar neste slice a política de configuração local via environment variables versus user secrets; isso fica para um slice separado.
- Assumir que, no desenvolvimento local, as connection strings `ReadOnly` e `ReadWrite` podem apontar temporariamente para a mesma instância local.

## Testing Decisions

- Validar por inspeção estática que a infraestrutura local existe, é reproduzível e inclui a ativação do PostGIS.
- Manter o padrão atual de testes automatizados sem ampliar o escopo para trocar toda a suíte de integração para imagem PostGIS neste slice.
- Priorizar testes futuros de comportamento externo quando surgirem as primeiras entidades e migrations com colunas espaciais.

## Out of Scope

- Modelagem de colunas `geometry` ou `geography`
- Ingestão de setores censitários, polígonos ou coordenadas
- Geocodificação de locais de votação
- Endpoints REST ou queries SQL espaciais
- Troca do setup local para `dotnet user-secrets`
- Separação automatizada de usuários read-only/read-write no banco local

## Further Notes

- Este é um slice de fundação e fica melhor como `5.5`, entre migrations base e ingestões seguintes.
- Depois dele, o caminho natural é quebrar o tema geoespacial em slices menores, por exemplo: `5.6` modelagem espacial IBGE, `5.7` ingestão espacial IBGE, `5.8` suporte espacial TSE.
