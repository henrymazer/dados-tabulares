using NetTopologySuite.Geometries;

namespace Data;

public sealed class UfRecord
{
    public string Sigla { get; set; } = string.Empty;
}

public sealed class AnoEleicaoRecord
{
    public int Ano { get; set; }
}

public sealed class MunicipioRecord
{
    public int CodigoIbge { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string UfSigla { get; set; } = string.Empty;
}

public sealed class PartidoRecord
{
    public int Numero { get; set; }
    public string Sigla { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
}

public sealed class CandidatoRecord
{
    public string Cpf { get; set; } = string.Empty;
    public int AnoEleicao { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string NomeUrna { get; set; } = string.Empty;
    public int Numero { get; set; }
    public string Cargo { get; set; } = string.Empty;
    public int PartidoNumero { get; set; }
    public string PartidoSigla { get; set; } = string.Empty;
    public string PartidoNome { get; set; } = string.Empty;
    public string UfSigla { get; set; } = string.Empty;
}

public sealed class ColigacaoRecord
{
    public string Nome { get; set; } = string.Empty;
    public int AnoEleicao { get; set; }
    public string PartidosJson { get; set; } = "[]";
}

public sealed class ZonaEleitoralRecord
{
    public int NumeroZona { get; set; }
    public int MunicipioCodigoIbge { get; set; }
    public string MunicipioNome { get; set; } = string.Empty;
    public string UfSigla { get; set; } = string.Empty;
}

public sealed class SecaoEleitoralRecord
{
    public int NumeroSecao { get; set; }
    public int NumeroZona { get; set; }
    public int MunicipioCodigoIbge { get; set; }
    public string MunicipioNome { get; set; } = string.Empty;
    public string UfSigla { get; set; } = string.Empty;
}

public sealed class ResultadoEleitoralRecord
{
    public string CandidatoCpf { get; set; } = string.Empty;
    public int AnoEleicao { get; set; }
    public string CandidatoNome { get; set; } = string.Empty;
    public string CandidatoNomeUrna { get; set; } = string.Empty;
    public int CandidatoNumero { get; set; }
    public string Cargo { get; set; } = string.Empty;
    public int PartidoNumero { get; set; }
    public string PartidoSigla { get; set; } = string.Empty;
    public string PartidoNome { get; set; } = string.Empty;
    public string UfSigla { get; set; } = string.Empty;
    public int Turno { get; set; }
    public int NumeroZona { get; set; }
    public int MunicipioCodigoIbge { get; set; }
    public string MunicipioNome { get; set; } = string.Empty;
    public int NumeroSecao { get; set; }
    public int QuantidadeVotos { get; set; }
}

public sealed class PerfilEleitoradoRecord
{
    public int AnoEleicao { get; set; }
    public int NumeroZona { get; set; }
    public int MunicipioCodigoIbge { get; set; }
    public string MunicipioNome { get; set; } = string.Empty;
    public string UfSigla { get; set; } = string.Empty;
    public string FaixaEtaria { get; set; } = string.Empty;
    public string Escolaridade { get; set; } = string.Empty;
    public string Genero { get; set; } = string.Empty;
    public int QuantidadeEleitores { get; set; }
}

public sealed class BemDeclaradoRecord
{
    public string CandidatoCpf { get; set; } = string.Empty;
    public int AnoEleicao { get; set; }
    public string CandidatoNome { get; set; } = string.Empty;
    public string CandidatoNomeUrna { get; set; } = string.Empty;
    public int CandidatoNumero { get; set; }
    public string Cargo { get; set; } = string.Empty;
    public int PartidoNumero { get; set; }
    public string PartidoSigla { get; set; } = string.Empty;
    public string PartidoNome { get; set; } = string.Empty;
    public string UfSigla { get; set; } = string.Empty;
    public string TipoBem { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
}

public sealed class PrestacaoContasRecord
{
    public string CandidatoCpf { get; set; } = string.Empty;
    public int AnoEleicao { get; set; }
    public string CandidatoNome { get; set; } = string.Empty;
    public string CandidatoNomeUrna { get; set; } = string.Empty;
    public int CandidatoNumero { get; set; }
    public string Cargo { get; set; } = string.Empty;
    public int PartidoNumero { get; set; }
    public string PartidoSigla { get; set; } = string.Empty;
    public string PartidoNome { get; set; } = string.Empty;
    public string UfSigla { get; set; } = string.Empty;
    public string TipoReceita { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public string TipoMovimentacao { get; set; } = string.Empty;
}

public sealed class DadoPopulacionalRecord
{
    public int MunicipioCodigoIbge { get; set; }
    public string MunicipioNome { get; set; } = string.Empty;
    public string UfSigla { get; set; } = string.Empty;
    public string FaixaEtaria { get; set; } = string.Empty;
    public string Raca { get; set; } = string.Empty;
    public int Quantidade { get; set; }
}

public sealed class DadoEscolaridadeRecord
{
    public int MunicipioCodigoIbge { get; set; }
    public string MunicipioNome { get; set; } = string.Empty;
    public string UfSigla { get; set; } = string.Empty;
    public string NivelEscolaridade { get; set; } = string.Empty;
    public int Quantidade { get; set; }
}

public sealed class DadoRendaRecord
{
    public int MunicipioCodigoIbge { get; set; }
    public string MunicipioNome { get; set; } = string.Empty;
    public string UfSigla { get; set; } = string.Empty;
    public string FaixaRenda { get; set; } = string.Empty;
    public int Quantidade { get; set; }
}

public sealed class DadoSaneamentoRecord
{
    public int MunicipioCodigoIbge { get; set; }
    public string MunicipioNome { get; set; } = string.Empty;
    public string UfSigla { get; set; } = string.Empty;
    public string TipoSaneamento { get; set; } = string.Empty;
    public int DomiciliosAtendidos { get; set; }
}

public sealed class DadoUrbanizacaoRecord
{
    public int MunicipioCodigoIbge { get; set; }
    public string MunicipioNome { get; set; } = string.Empty;
    public string UfSigla { get; set; } = string.Empty;
    public string TipoArea { get; set; } = string.Empty;
    public int Populacao { get; set; }
}

public sealed class DadoInfraestruturaRecord
{
    public int MunicipioCodigoIbge { get; set; }
    public string MunicipioNome { get; set; } = string.Empty;
    public string UfSigla { get; set; } = string.Empty;
    public string TipoInfraestrutura { get; set; } = string.Empty;
    public int DomiciliosAtendidos { get; set; }
}

public sealed class CargaBrutaSnapshotRecord
{
    public long Id { get; set; }
    public string Fonte { get; set; } = string.Empty;
    public string Dataset { get; set; } = string.Empty;
    public string Escopo { get; set; } = string.Empty;
    public string ChaveSnapshot { get; set; } = string.Empty;
    public string HashSnapshot { get; set; } = string.Empty;
    public string NomeArquivoOriginal { get; set; } = string.Empty;
    public string CaminhoArquivoOriginal { get; set; } = string.Empty;
    public long TamanhoBytes { get; set; }
    public int RegistrosImportados { get; set; }
    public bool IsCurrent { get; set; }
    public DateTimeOffset RegistradoEmUtc { get; set; }
    public DateTimeOffset? SubstituidoEmUtc { get; set; }
}

public sealed class CargaBrutaAuditoriaRecord
{
    public long Id { get; set; }
    public long SnapshotId { get; set; }
    public string Fonte { get; set; } = string.Empty;
    public string Dataset { get; set; } = string.Empty;
    public string Escopo { get; set; } = string.Empty;
    public string ChaveSnapshot { get; set; } = string.Empty;
    public string HashSnapshot { get; set; } = string.Empty;
    public string NomeArquivoOriginal { get; set; } = string.Empty;
    public string CaminhoArquivoOriginal { get; set; } = string.Empty;
    public long TamanhoBytes { get; set; }
    public string Status { get; set; } = string.Empty;
    public int RegistrosImportados { get; set; }
    public DateTimeOffset RegistradoEmUtc { get; set; }
}

public sealed class SetorCensitarioRecord
{
    public string CodigoSetor { get; set; } = string.Empty;
    public string Situacao { get; set; } = string.Empty;
    public string CodigoSituacao { get; set; } = string.Empty;
    public string CodigoTipo { get; set; } = string.Empty;
    public double AreaKm2 { get; set; }
    public string CodigoRegiao { get; set; } = string.Empty;
    public string NomeRegiao { get; set; } = string.Empty;
    public string CodigoUf { get; set; } = string.Empty;
    public string NomeUf { get; set; } = string.Empty;
    public string MunicipioCodigoIbge { get; set; } = string.Empty;
    public string MunicipioNome { get; set; } = string.Empty;
    public string CodigoDistrito { get; set; } = string.Empty;
    public string NomeDistrito { get; set; } = string.Empty;
    public string CodigoSubdistrito { get; set; } = string.Empty;
    public string NomeSubdistrito { get; set; } = string.Empty;
    public string CodigoBairro { get; set; } = string.Empty;
    public string NomeBairro { get; set; } = string.Empty;
    public string CodigoNucleoUrbano { get; set; } = string.Empty;
    public string NomeNucleoUrbano { get; set; } = string.Empty;
    public string CodigoFcu { get; set; } = string.Empty;
    public string NomeFcu { get; set; } = string.Empty;
    public string CodigoAglomerado { get; set; } = string.Empty;
    public string NomeAglomerado { get; set; } = string.Empty;
    public string CodigoRegiaoIntermediaria { get; set; } = string.Empty;
    public string NomeRegiaoIntermediaria { get; set; } = string.Empty;
    public string CodigoRegiaoImediata { get; set; } = string.Empty;
    public string NomeRegiaoImediata { get; set; } = string.Empty;
    public string CodigoConcentracaoUrbana { get; set; } = string.Empty;
    public string NomeConcentracaoUrbana { get; set; } = string.Empty;
    public string UfSigla { get; set; } = string.Empty;
    public MultiPolygon Geometria { get; set; } = default!;
}

public sealed class MunicipioMalhaRecord
{
    public string CodigoMunicipio { get; set; } = string.Empty;
    public string NomeMunicipio { get; set; } = string.Empty;
    public string CodigoRegiaoImediata { get; set; } = string.Empty;
    public string NomeRegiaoImediata { get; set; } = string.Empty;
    public string CodigoRegiaoIntermediaria { get; set; } = string.Empty;
    public string NomeRegiaoIntermediaria { get; set; } = string.Empty;
    public string CodigoUf { get; set; } = string.Empty;
    public string NomeUf { get; set; } = string.Empty;
    public string UfSigla { get; set; } = string.Empty;
    public string CodigoRegiao { get; set; } = string.Empty;
    public string NomeRegiao { get; set; } = string.Empty;
    public string SiglaRegiao { get; set; } = string.Empty;
    public string CodigoConcentracaoUrbana { get; set; } = string.Empty;
    public string NomeConcentracaoUrbana { get; set; } = string.Empty;
    public double AreaKm2 { get; set; }
    public MultiPolygon Geometria { get; set; } = default!;
}

public sealed class IbgeCatalogoVariavelRecord
{
    public long Id { get; set; }
    public string FonteDicionario { get; set; } = string.Empty;
    public string Pacote { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string Tema { get; set; } = string.Empty;
    public string Variavel { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
}

public sealed class IbgeCatalogoCategoriaRecord
{
    public long Id { get; set; }
    public string FonteDicionario { get; set; } = string.Empty;
    public string Pacote { get; set; } = string.Empty;
    public string Variavel { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
}

public sealed class IbgeAgregadoStagingRecord
{
    public long Id { get; set; }
    public string Pacote { get; set; } = string.Empty;
    public string NomeArquivoInterno { get; set; } = string.Empty;
    public string CodigoSetor { get; set; } = string.Empty;
    public string PayloadJson { get; set; } = "{}";
}

public sealed class TseLocalVotacaoBrutoStagingRecord
{
    public long Id { get; set; }
    public int AnoEleicao { get; set; }
    public string UfSigla { get; set; } = string.Empty;
    public string CodigoUnidadeEleitoral { get; set; } = string.Empty;
    public string NomeUnidadeEleitoral { get; set; } = string.Empty;
    public string MunicipioCodigoIbge { get; set; } = string.Empty;
    public string MunicipioNome { get; set; } = string.Empty;
    public int NumeroZona { get; set; }
    public int NumeroSecao { get; set; }
    public int NumeroLocalVotacao { get; set; }
    public string NomeLocalVotacao { get; set; } = string.Empty;
    public string EnderecoLocalVotacao { get; set; } = string.Empty;
    public string PayloadJson { get; set; } = "{}";
}

public sealed class TrimestreRecord
{
    public int Ano { get; set; }
    public int Numero { get; set; }
}

public sealed class DadoDesempregoRecord
{
    public string UfSigla { get; set; } = string.Empty;
    public int TrimestreAno { get; set; }
    public int TrimestreNumero { get; set; }
    public decimal TaxaDesemprego { get; set; }
}

public sealed class DadoInformalidadeRecord
{
    public string UfSigla { get; set; } = string.Empty;
    public int TrimestreAno { get; set; }
    public int TrimestreNumero { get; set; }
    public decimal TaxaInformalidade { get; set; }
}

public sealed class DadoRendaMediaRecord
{
    public string UfSigla { get; set; } = string.Empty;
    public int TrimestreAno { get; set; }
    public int TrimestreNumero { get; set; }
    public decimal RendaMedia { get; set; }
}
