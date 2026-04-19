`scripts/load_ibge_dataset.py` carrega um ou mais datasets agregados do IBGE no schema `ibge`, reaproveitando a tabela geográfica compartilhada `ibge.setores_censitarios`.

Uso:

```bash
python3 scripts/load_ibge_dataset.py \
  --connection-string 'postgresql://USER:PASSWORD@HOST:PORT/DB?sslmode=require' \
  --dataset basico \
  --dataset demografia
```

Datasets suportados:

- `basico`
- `caracteristicas_domicilio1`
- `caracteristicas_domicilio2`
- `caracteristicas_domicilio3`
- `cor_ou_raca`
- `demografia`
- `domicilios_indigenas`
- `domicilios_quilombolas`
- `entorno_moradores`
- `obitos`
- `parentesco`
- `pessoas_indigenas`
- `pessoas_quilombolas`
- `renda_responsavel`

Observações:

- O carregador exige que `ibge.setores_censitarios` já exista.
- Cada dataset vira uma tabela `<slug>_setores` e uma view `<slug>_setores_com_geometria`.
- Os valores `X` do IBGE são convertidos para `NULL`.
- As colunas são carregadas como `numeric` para acomodar contagens, médias e percentuais sem depender de tipagem específica por arquivo.
- O dataset `basico` ignora as colunas administrativas duplicadas do CSV e mantém apenas `cd_setor` + variáveis `V0001..V0007`, porque a dimensão geográfica compartilhada já carrega esses atributos.
