`scripts/load_ibge_alfabetizacao.py` faz a carga inicial do conjunto de alfabetização do IBGE no schema `ibge`.

O que ele faz:

1. Cria `postgis` e o schema `ibge`.
2. Cria `ibge.dicionario_variaveis`.
3. Cria `ibge.alfabetizacao_setores`.
4. Converte o CSV do ZIP de alfabetização para um CSV normalizado.
5. Converte `X` do arquivo do IBGE em `NULL`.
6. Importa a malha `BR_setores_CD2022.gpkg` para `ibge.setores_censitarios_raw`.
7. Consolida uma geometria única por `cd_setor` em `ibge.setores_censitarios`.
8. Cria a view `ibge.alfabetizacao_setores_com_geometria`.
9. Aplica comentários detalhados nas tabelas e colunas.

Uso:

```bash
python3 scripts/load_ibge_alfabetizacao.py \
  --connection-string 'postgresql://USER:PASSWORD@HOST:PORT/DB?sslmode=require'
```
