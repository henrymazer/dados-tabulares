# PRD 05.8: Ingestão Real Geoespacial IBGE/TSE

## Problem Statement

O repositório já possui fundação técnica para PostGIS, modelagem espacial inicial e uma prova de ingestão espacial. Ainda falta a trilha real de ingestão dos arquivos brutos que serão usados em produção. O IBGE entrega malhas e agregados por setor censitário, enquanto o TSE entrega cadastros e fatos eleitorais que precisam ser consolidados, georreferenciados e validados espacialmente. Sem uma ingestão real, versionada e auditável, o banco não se torna fonte confiável para a API nem para futuros agentes de IA.

## Solution

Implementar uma trilha de ingestão real geoespacial com três camadas: `staging` para preservar o bruto, `canonical` para consolidar identidades e vínculos, e `enriched` para geometrias e geocodificação. No IBGE, a malha municipal e a malha de setores serão carregadas a partir de arquivos oficiais, com agregados largos por tema vinculados por `CD_SETOR` e catálogo de variáveis derivado dos dicionários. No TSE, os arquivos de detalhe serão carregados em staging, consolidados no nível de local de votação, deduplicados por endereço e geocodificados com Nominatim self-hosted como provedor principal e Google Geocoding API como fallback. Os resultados geocodificados só serão aceitos quando caírem dentro do município oficial esperado, validado contra a malha municipal do IBGE.

## User Stories

1. Como mantenedor da base, quero carregar a malha municipal oficial do IBGE, para validar espacialmente dados de outros domínios.
2. Como mantenedor da base, quero carregar a malha oficial de setores censitários, para usar os setores como unidade espacial básica do projeto.
3. Como mantenedor da base, quero preservar `CD_SETOR` como chave textual de 15 dígitos, para evitar perda de integridade ao vincular malha e agregados.
4. Como analista, quero consultar atributos territoriais oficiais do setor junto da geometria, para cruzar variáveis sem depender de arquivos externos.
5. Como mantenedor da base, quero ingerir os agregados do IBGE por tema em tabelas largas, para preservar fidelidade ao formato oficial antes de qualquer remodelagem analítica.
6. Como analista, quero que todos os agregados do IBGE mantenham vínculo determinístico com o setor censitário, para cruzar indicadores com a malha espacial.
7. Como usuário técnico, quero um catálogo de variáveis do IBGE com `Tipo`, `Tema`, `Variável`, `Descrição` e categorias, para interpretar colunas como `V0001` sem consultar planilhas externas.
8. Como futuro consumidor por API, quero metadados consultáveis das variáveis do IBGE, para que a aplicação consiga descrever colunas e medidas.
9. Como operador da ingestão, quero registrar origem, arquivo, hash e snapshot de cada carga, para reprocessar sem duplicar dados.
10. Como operador da ingestão, quero que reprocessamentos substituam o snapshot anterior da mesma fonte, para manter o banco idempotente.
11. Como mantenedor da base, quero ingerir o cadastro bruto do TSE como ele vem nos arquivos, para garantir rastreabilidade e auditoria.
12. Como analista eleitoral, quero consolidar o TSE no nível de local de votação, para trabalhar no grão que faz sentido analítico.
13. Como analista eleitoral, quero que a identidade canônica do local use `SG_UF + CD_MUNICIPIO + NR_ZONA + NR_LOCAL_VOTACAO`, para evitar colisões e manter consistência entre arquivos.
14. Como operador da ingestão, quero deduplicar endereços antes da geocodificação, para evitar consultas repetidas e inconsistentes.
15. Como operador da ingestão, quero geocodificar com Nominatim self-hosted, para ter autonomia operacional e baixo custo recorrente.
16. Como operador da ingestão, quero começar o geocoder com extrato do Paraná, para validar o pipeline com custo e tempo menores.
17. Como mantenedor da solução, quero poder evoluir do extrato do Paraná para o Brasil inteiro, para escalar sem redesenhar a arquitetura.
18. Como operador da ingestão, quero usar Google Geocoding API como fallback, para resolver casos ambíguos ou malsucedidos no Nominatim.
19. Como responsável pela qualidade espacial, quero aceitar um ponto apenas se ele cair dentro do município esperado, para evitar georreferenciamento incorreto.
20. Como responsável pela qualidade espacial, quero marcar casos ambíguos, fora do município ou não resolvidos, para rastrear falhas de decisão.
21. Como auditor da ingestão, quero guardar histórico de tentativas de geocodificação, para entender por que um local foi aceito, rejeitado ou ficou pendente.
22. Como auditor da ingestão, quero guardar sempre um resumo estruturado da tentativa e payload bruto completo apenas para casos problemáticos, para equilibrar rastreabilidade e armazenamento.
23. Como mantenedor da base, quero um catálogo derivado do schema dos arquivos do TSE, para apoiar documentação e uso futuro por agentes de IA.
24. Como analista eleitoral, quero carregar os fatos eleitorais em grão canônico mínimo e derivar agregações depois, para não perder flexibilidade analítica.
25. Como consumidor da API, quero que os fatos eleitorais se vinculem ao local canônico georreferenciado, para permitir análises por município e local de votação.
26. Como operador da ingestão, quero separar claramente infraestrutura do geocoder, staging bruto, dados canônicos e enriquecimento, para simplificar manutenção e testes.
27. Como desenvolvedor, quero que credenciais e configuração do Google fiquem fora de arquivos versionados, para operar localmente com segurança.
28. Como desenvolvedor, quero testes automatizados dos fluxos críticos de parsing, vinculação, geocodificação e validação espacial, para evoluir o pipeline sem regressões silenciosas.

## Implementation Decisions

- O PRD cobre uma trilha guarda-chuva de ingestão real para IBGE e TSE; os slices subsequentes serão menores e verticais.
- As camadas operacionais serão separadas em `staging`, `canonical` e `enriched`.
- `5.6` e `5.7` permanecem como fundação técnica já entregue; a ingestão real começa a partir deste PRD.
- A malha municipal oficial do IBGE será fonte de verdade para validação espacial do TSE.
- A malha de setores censitários será carregada a partir do `GeoPackage` oficial com `CD_SETOR` como chave canônica textual de 15 dígitos.
- Os agregados do IBGE serão ingeridos inicialmente em tabelas largas por arquivo/pacote, preservando nomes originais das colunas sempre que tecnicamente possível.
- Os dicionários do IBGE serão ingeridos como catálogo semântico persistido no banco, incluindo `Tipo`, `Tema`, `Variável`, `Descrição` e categorias quando existirem.
- O catálogo do TSE será derivado do schema observado nos arquivos ingeridos, sem depender de parsing do PDF oficial nesta fase.
- No TSE, o grão canônico espacial será o local de votação, não a seção.
- A identidade canônica do local de votação usará `SG_UF + CD_MUNICIPIO + NR_ZONA + NR_LOCAL_VOTACAO`.
- `NR_SECAO` pode existir em staging, mas não é dimensão analítica principal nem parte da chave canônica do local.
- Os fatos eleitorais do TSE serão persistidos primeiro no menor grão útil do arquivo e agregados depois por local.
- A geocodificação usará duas queries principais: uma com nome do local + endereço + município + UF + Brasil e outra sem o nome do local.
- O provedor principal será Nominatim self-hosted e o fallback será Google Geocoding API.
- O extrato inicial do Nominatim será o Paraná; a arquitetura deve suportar expansão posterior para o Brasil inteiro.
- A validação final de qualquer ponto geocodificado exigirá pertencimento espacial ao município esperado.
- A deduplicação de endereços ocorrerá antes da consulta aos geocoders, em escopo nacional, preservando município e UF para validação.
- Cada carga deverá registrar metadados de origem, arquivo, hash e snapshot para garantir idempotência e reprocessamento seguro.
- O workflow de revisão humana não entra neste PRD; casos problemáticos serão classificados com status operacionais.
- Credenciais do Google e demais segredos operacionais devem ficar fora de arquivos versionados, via configuração local segura.

## Testing Decisions

- Bons testes devem validar comportamento observável: parsing correto, vinculação correta, persistência correta, decisão de geocodificação e validação espacial, sem acoplamento a detalhes internos da implementação.
- Devem existir testes de unidade para parsers de dicionários, normalização de chaves, deduplicação de endereços e decisão de aceitação/rejeição de candidatos de geocodificação.
- Devem existir testes de integração para cargas reais simplificadas do IBGE e TSE, usando PostgreSQL/PostGIS e verificando persistência, idempotência e vínculos entre camadas.
- Devem existir testes de integração do fluxo espacial do TSE verificando aceitação quando o ponto cai dentro do município e rejeição quando cai fora.
- Devem existir testes para fallback entre Nominatim e Google, cobrindo sucesso no primário, sucesso no fallback e falha total.
- Devem existir testes para o catálogo semântico do IBGE e catálogo derivado do TSE.
- O prior art deve seguir os testes já existentes no repositório para `Etl.Tests`, `Data.Tests` e testes com Testcontainers/PostGIS.

## Out of Scope

- Interface humana de revisão de geocodificação.
- Publicação dos novos dados e metadados via endpoints finais da API.
- Atualizações contínuas do Nominatim com diffs OSM.
- Parsing automático do PDF de dicionário do TSE.
- Remodelagem analítica final dos agregados largos do IBGE em views ou modelos dimensionais completos.
- Expansão imediata do geocoder de Paraná para Brasil inteiro na primeira entrega.

## Further Notes

- O valor deste trabalho não é apenas carregar dados, mas preservar semântica suficiente para consumo futuro por agentes de IA.
- A rastreabilidade entre arquivo bruto, dicionário, tabela canônica e resultado geoespacial é requisito de primeira classe.
- O pipeline deve permitir rodadas de ingestão esporádicas, como a geocodificação bienal, sem exigir operação manual frequente.
