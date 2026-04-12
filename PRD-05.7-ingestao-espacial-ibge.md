# PRD-05.7 - Ingestao Espacial IBGE

## Objetivo
Popular `ibge.setores_censitarios` a partir de uma fonte espacial do IBGE, preservando o modelo definido no `5.6` e garantindo geometria `MultiPolygon` em SRID `4674`.

## Escopo
- adicionar um dataset espacial explicito para setores censitarios no fluxo ETL do IBGE;
- ler uma fonte GeoJSON de setores censitarios com metadados minimos do setor;
- converter `Polygon` para `MultiPolygon` quando necessario;
- validar geometria, nulidade e SRID antes da persistencia;
- substituir a carga espacial inteira de forma transacional, sem afetar os datasets tabulares.

## Nao Escopo
- nao modelar outros objetos geograficos do IBGE;
- nao implementar suporte espacial do TSE;
- nao expor endpoint novo;
- nao alterar o schema espacial alem do necessario para a ingestao.

## Criterios de Aceite
- o ETL aceita o dataset `setores-censitarios`;
- a ingestao grava `CodigoSetor`, `MunicipioCodigoIbge`, `MunicipioNome`, `UfSigla` e `Geometria`;
- geometrias sao persistidas como `MultiPolygon` com SRID `4674`;
- geometria invalida ou tipo espacial nao suportado falha com erro explicito;
- existe teste cobrindo parser e ingestao integrada contra PostGIS.
