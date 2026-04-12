# PRD — Serviço de Dados Públicos (IBGE, TSE, PNAD)

## Problem Statement

Candidatos, políticos eleitos, assessores, partidos políticos e profissionais contratados precisam acessar dados públicos (IBGE, TSE, PNAD Contínua) de forma centralizada, filtrada e cruzada para embasar decisões políticas, estratégias eleitorais e análises socioeconômicas. Hoje esses dados estão espalhados em fontes diferentes, em formatos brutos e de difícil consumo, exigindo trabalho manual para cada consulta.

Além disso, agentes de IA já existentes no ecossistema precisam consultar esses dados de forma dinâmica (via SQL) para gerar insights, resumos e sugestões automatizadas.

## Solution

Um microserviço de "Dados Públicos" em .NET 10/C# que:

1. Ingere, transforma e armazena dados do IBGE (Censo), TSE (eleições) e PNAD Contínua em um PostgreSQL modelado e normalizado
2. Expõe endpoints REST específicos para o frontend (tabelas, gráficos, dashboards)
3. Expõe um endpoint de query SQL dinâmica para agentes de IA, com validação de segurança em profundidade
4. Expõe metadata das views disponíveis para que agentes de IA saibam o que podem consultar
5. Implementa cache de longa duração com invalidação explícita pós-ingestão

O projeto vive em um monorepo com dois entregáveis: a API (deploy no Google Cloud Run) e o ETL (console app rodado localmente).

## User Stories

1. Como administrador do sistema, quero ingerir dados completos do Censo IBGE a partir de arquivos brutos, para que todos os clientes tenham acesso a dados populacionais e socioeconômicos.
2. Como administrador do sistema, quero ingerir dados completos de eleições do TSE a partir de arquivos brutos, para que todos os clientes tenham acesso a resultados eleitorais históricos.
3. Como administrador do sistema, quero ingerir dados trimestrais da PNAD Contínua, para que os clientes tenham dados atualizados de emprego e renda.
4. Como administrador do sistema, quero que os arquivos brutos originais sejam armazenados em um bucket (GCS) como backup, para que eu possa reprocessar os dados se necessário.
5. Como administrador do sistema, quero invalidar o cache do serviço após uma ingestão, para que os clientes vejam os dados atualizados imediatamente.
6. Como administrador do sistema, quero ser alertado quando uma query SQL for rejeitada pela validação, para identificar possíveis tentativas de ataque ou prompt injection.
7. Como administrador do sistema, quero visualizar logs estruturados de todas as queries SQL executadas (query, tempo, tenant, rows retornados), para monitorar o uso e diagnosticar problemas.
8. Como assessor político, quero consultar resultados eleitorais filtrados por município, zona eleitoral, partido e ano, para analisar o desempenho histórico do meu candidato.
9. Como assessor político, quero consultar o perfil do eleitorado (faixa etária, escolaridade, gênero) por zona eleitoral, para entender a composição demográfica dos eleitores.
10. Como assessor político, quero cruzar dados eleitorais com dados socioeconômicos do IBGE, para identificar correlações entre perfil da população e votação.
11. Como candidato, quero ver dashboards com dados agregados (totais, médias, séries temporais) do meu município, para entender o contexto socioeconômico da minha região.
12. Como candidato, quero filtrar dados do IBGE por município, estado e faixa etária, para planejar estratégias de campanha segmentadas.
13. Como partido político, quero visualizar dados de todas as eleições anteriores por partido e coligação, para análise estratégica de longo prazo.
14. Como partido político, quero acessar dados da PNAD Contínua (desemprego, renda, informalidade) por UF e trimestre, para embasar discursos e propostas.
15. Como agente de IA, quero consultar um endpoint de metadata que retorne as views disponíveis, colunas, tipos e descrições, para saber quais dados posso consultar antes de gerar SQL.
16. Como agente de IA, quero enviar uma query SQL SELECT para o serviço e receber os resultados em JSON, para gerar análises e insights dinâmicos.
17. Como agente de IA, quero que queries com mais de 10.000 rows sejam paginadas, para evitar respostas excessivamente grandes.
18. Como profissional contratado, quero consumir a API REST para integrar dados públicos em relatórios e ferramentas externas, para entregar análises aos meus clientes políticos.
19. Como qualquer usuário autenticado, quero que meu acesso seja validado via JWT em toda requisição, para garantir que só clientes autorizados acessem os dados.
20. Como qualquer usuário autenticado, quero que as respostas da API sejam cacheadas e rápidas, para ter uma experiência fluida mesmo com grandes volumes de dados.
21. Como administrador do sistema, quero que queries SQL anômalas (acima de 60 segundos) gerem alertas, para proteger o banco de dados de sobrecarga.
22. Como administrador do sistema, quero que o ETL de cada fonte (IBGE, TSE, PNAD) possa ser executado independentemente, para ingerir apenas os dados que foram atualizados.
23. Como assessor político, quero acessar dados de bens declarados e prestação de contas dos candidatos (TSE), para análise de transparência.
24. Como candidato, quero ver dados de saneamento, urbanização e infraestrutura do IBGE por município, para identificar demandas da população.
25. Como partido político, quero visualizar dados de renda e escolaridade por município e bairro (quando disponível), para segmentação de campanha.

## Implementation Decisions

### Arquitetura Geral

- Microserviço único de "Dados Públicos" cobrindo IBGE, TSE e PNAD Contínua
- Monorepo com uma solution .NET contendo os projetos: `Api`, `Etl`, `Domain`, `Data`
- API deployada no Google Cloud Run
- ETL é um console app .NET rodado localmente
- Banco PostgreSQL gerenciado na Digital Ocean

### Banco de Dados

- PostgreSQL único com schemas separados por domínio: `ibge`, `tse`, `pnad`
- Zero acesso ao schema `core` (multi-tenant/usuários) — isolamento total
- Dados armazenados já modelados e normalizados após ETL na ingestão
- Views materializadas como superfície de consulta para os agentes de IA (não acessam tabelas reais)
- Dados públicos são compartilhados (sem multi-tenancy nos dados) — filtragem via query parameters
- User de banco read-only para o serviço de API

### ETL

- Projeto console .NET que lê CSVs/arquivos brutos, transforma e carrega no Postgres
- Cada fonte (IBGE, TSE, PNAD) pode ser ingerida independentemente
- Arquivos brutos originais são enviados para um bucket GCS como backup
- Frequência: TSE a cada 2 anos, IBGE Censo a cada 10 anos, PNAD Contínua trimestralmente
- Após ingestão, o ETL chama o endpoint de invalidação de cache do serviço

### API REST (Frontend)

- Endpoints REST específicos por caso de uso com filtros predefinidos (município, estado, ano, zona eleitoral)
- Retorna tanto dados granulares (rows) quanto dados agregados (totais, médias, séries temporais)
- Cache HTTP com `Cache-Control` — TTL de dias/semanas para Censo/TSE, horas após ingestão de PNAD

### API Query (Agentes de IA)

- Endpoint `POST /api/query` que recebe SQL como string
- BAML roda no lado do agente (serviço separado) para gerar e validar SQL estruturalmente
- O serviço de Dados Públicos faz validação própria redundante:
  - Parse do SQL: rejeita tudo que não seja `SELECT`
  - Whitelist de views/tabelas permitidas
  - Bloqueio de `INSERT`, `UPDATE`, `DELETE`, `DROP`, DDL, CTEs com side effects, `INTO`
  - `LIMIT` máximo forçado (10.000 rows) em toda query
  - Timeout de execução para queries anômalas
- Execução com user PostgreSQL read-only, acessando apenas views nos schemas `ibge`, `tse`, `pnad`

### Metadata

- Endpoint `GET /api/metadata` retorna lista de views disponíveis, colunas, tipos e descrições em linguagem natural
- Usado pelos agentes de IA como contexto antes de gerar SQL

### Cache

- `IMemoryCache` do .NET com TTL longo (~7 dias)
- Invalidação explícita via `POST /api/admin/cache/invalidate` (chamado pelo ETL pós-ingestão)
- Cache de query SQL com chave = hash da query
- Sem Redis — `IMemoryCache` é suficiente dado o padrão de uso

### Autenticação e Autorização

- JWT emitido pelo serviço de admin existente
- Validação local no serviço de Dados Públicos (sem chamada ao serviço de admin por request)
- Claims no JWT definem quais módulos o cliente tem acesso

### Observabilidade

- Logs estruturados com Serilog → Cloud Logging do Google (automático no Cloud Run)
- Toda query SQL logada: query, tempo de execução, tenant, rows retornados
- Alertas no Cloud Monitoring:
  - Queries rejeitadas pela validação (possível ataque/prompt injection)
  - Queries acima de 60 segundos (proteção do banco, não UX — agentes de IA têm latência esperada)

### Health Check

- `GET /health` com readiness check do banco de dados
- Necessário para o Cloud Run rotear requests corretamente

## Testing Decisions

### Filosofia de Testes

- Desenvolvimento via TDD (red-green-refactor) para todos os módulos
- Testar comportamento externo, não detalhes de implementação
- Cada módulo tem interface clara que permite teste isolado

### Módulos e Testes

| Módulo | Tipo de Teste | O que testar |
|---|---|---|
| **Domain** | Unitário | Entities, value objects, regras de validação, transformações de dados |
| **Data** | Integração | Repositórios contra PostgreSQL real (testcontainers), migrations, queries |
| **Etl (Transformação)** | Unitário | Parsing de CSVs, transformação de dados brutos em entities do domínio, edge cases (dados faltantes, formatos inesperados) |
| **Etl (Carga)** | Integração | Carga de dados transformados no banco, idempotência, verificação de integridade |
| **Api.Query (Validação SQL)** | Unitário | Parser rejeita INSERT/UPDATE/DELETE/DROP/DDL, aceita SELECT válido, aplica LIMIT, whitelist de views, detecção de SQL injection patterns |
| **Api.Query (Execução)** | Integração | Query SQL válida retorna dados corretos do banco, timeout funciona, user read-only não consegue escrever |
| **Api.Rest** | Integração | Endpoints retornam dados filtrados corretamente, agregações estão corretas, paginação funciona |
| **Api.Metadata** | Unitário | Retorna views, colunas e tipos corretos, descrições não estão vazias |
| **Api.Cache** | Unitário | Cache hit/miss, TTL respeitado, invalidação limpa tudo, hash de query funciona corretamente |
| **Api.Auth** | Unitário | JWT válido aceito, JWT expirado rejeitado, claims extraídas corretamente, request sem token retorna 401 |
| **Api.Health** | Integração | Retorna 200 quando banco acessível, retorna 503 quando banco inacessível |

### Ferramentas

- xUnit como framework de testes (.NET padrão)
- Testcontainers para testes de integração com PostgreSQL
- Mocks/fakes para isolamento de dependências nos testes unitários

## Out of Scope

- **Módulo de administração multi-tenant** — já existe e está pronto
- **Agentes de IA e BAML** — já existem em serviço separado; este PRD cobre apenas o serviço que recebe as queries
- **RAG e banco vetorial** — já existe em serviço separado
- **Módulo de redes sociais** — já existe em serviço separado
- **Frontend/webapp** — este PRD cobre apenas o backend (API + ETL); o frontend será definido separadamente
- **API Gateway centralizado** — pode ser adicionado no futuro quando houver mais serviços
- **Bloco de pesquisa (planos de pesquisa, coleta, visualização)** — módulo futuro, fora deste escopo
- **Dados de leis, projetos de lei e leis eleitorais** — módulos futuros, fora deste escopo
- **Dados de prefeitura/governo do estado/governo federal** — módulos futuros, fora deste escopo

## Further Notes

- A ingestão de dados é um trabalho de uma vez (ou esporádico), então simplicidade no ETL é mais importante que performance extrema
- Os dados públicos são iguais para todos os clientes — o controle de acesso é sobre quem pode usar o módulo, não sobre quais dados cada cliente vê
- A escolha de SQL dinâmico para agentes é deliberada: a flexibilidade compensa o risco, que é mitigado por defesa em profundidade (BAML + validação no serviço + user read-only + views como superfície)
- A migração futura para banco separado por serviço é possível sem mudança de código, apenas de connection string, graças ao isolamento por schemas
- O Cloud Run escala a zero quando não há requests, o que mantém o custo baixo dado o uso esporádico esperado inicialmente
