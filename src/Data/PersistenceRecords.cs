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
