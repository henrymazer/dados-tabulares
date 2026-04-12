using System.Text.Json;
using DadosTabulares.Domain.Ibge;
using DadosTabulares.Domain.Pnad;
using DadosTabulares.Domain.Tse;

namespace Data.Repositories;

internal static class RepositoryMapping
{
    public static Municipio ToMunicipio(int codigoIbge, string nome, string ufSigla)
        => new(codigoIbge, nome, new UF(ufSigla));

    public static Partido ToPartido(int numero, string sigla, string nome)
        => new(numero, sigla, nome);

    public static Candidato ToCandidato(
        string cpf,
        string nome,
        string nomeUrna,
        int numero,
        string cargo,
        int partidoNumero,
        string partidoSigla,
        string partidoNome,
        int anoEleicao,
        string ufSigla)
        => new(cpf, nome, nomeUrna, numero, cargo, ToPartido(partidoNumero, partidoSigla, partidoNome), new AnoEleicao(anoEleicao), new UF(ufSigla));

    public static ZonaEleitoral ToZona(int numeroZona, int codigoIbge, string nomeMunicipio, string ufSigla)
        => new(numeroZona, ToMunicipio(codigoIbge, nomeMunicipio, ufSigla));

    public static SecaoEleitoral ToSecao(int numeroSecao, int numeroZona, int codigoIbge, string nomeMunicipio, string ufSigla)
        => new(numeroSecao, ToZona(numeroZona, codigoIbge, nomeMunicipio, ufSigla));

    public static ResultadoEleitoral ToDomain(this ResultadoEleitoralRecord record)
        => new(
            ToCandidato(
                record.CandidatoCpf,
                record.CandidatoNome,
                record.CandidatoNomeUrna,
                record.CandidatoNumero,
                record.Cargo,
                record.PartidoNumero,
                record.PartidoSigla,
                record.PartidoNome,
                record.AnoEleicao,
                record.UfSigla),
            new AnoEleicao(record.AnoEleicao),
            record.Turno,
            ToZona(record.NumeroZona, record.MunicipioCodigoIbge, record.MunicipioNome, record.UfSigla),
            ToSecao(record.NumeroSecao, record.NumeroZona, record.MunicipioCodigoIbge, record.MunicipioNome, record.UfSigla),
            record.QuantidadeVotos);

    public static ResultadoEleitoralRecord ToRecord(this ResultadoEleitoral domain)
        => new()
        {
            CandidatoCpf = domain.Candidato.Cpf,
            AnoEleicao = domain.AnoEleicao.Ano,
            CandidatoNome = domain.Candidato.Nome,
            CandidatoNomeUrna = domain.Candidato.NomeUrna,
            CandidatoNumero = domain.Candidato.Numero,
            Cargo = domain.Candidato.Cargo,
            PartidoNumero = domain.Candidato.Partido.Numero,
            PartidoSigla = domain.Candidato.Partido.Sigla,
            PartidoNome = domain.Candidato.Partido.Nome,
            UfSigla = domain.Candidato.UF.Sigla,
            Turno = domain.Turno,
            NumeroZona = domain.ZonaEleitoral.NumeroZona,
            MunicipioCodigoIbge = domain.ZonaEleitoral.Municipio.CodigoIbge,
            MunicipioNome = domain.ZonaEleitoral.Municipio.Nome,
            NumeroSecao = domain.SecaoEleitoral.NumeroSecao,
            QuantidadeVotos = domain.QuantidadeVotos
        };

    public static PerfilEleitorado ToDomain(this PerfilEleitoradoRecord record)
        => new(
            new AnoEleicao(record.AnoEleicao),
            ToZona(record.NumeroZona, record.MunicipioCodigoIbge, record.MunicipioNome, record.UfSigla),
            record.FaixaEtaria,
            record.Escolaridade,
            record.Genero,
            record.QuantidadeEleitores);

    public static PerfilEleitoradoRecord ToRecord(this PerfilEleitorado domain)
        => new()
        {
            AnoEleicao = domain.AnoEleicao.Ano,
            NumeroZona = domain.ZonaEleitoral.NumeroZona,
            MunicipioCodigoIbge = domain.ZonaEleitoral.Municipio.CodigoIbge,
            MunicipioNome = domain.ZonaEleitoral.Municipio.Nome,
            UfSigla = domain.ZonaEleitoral.Municipio.UF.Sigla,
            FaixaEtaria = domain.FaixaEtaria,
            Escolaridade = domain.Escolaridade,
            Genero = domain.Genero,
            QuantidadeEleitores = domain.QuantidadeEleitores
        };

    public static BemDeclarado ToDomain(this BemDeclaradoRecord record)
        => new(
            ToCandidato(
                record.CandidatoCpf,
                record.CandidatoNome,
                record.CandidatoNomeUrna,
                record.CandidatoNumero,
                record.Cargo,
                record.PartidoNumero,
                record.PartidoSigla,
                record.PartidoNome,
                record.AnoEleicao,
                record.UfSigla),
            new AnoEleicao(record.AnoEleicao),
            record.TipoBem,
            record.Descricao,
            record.Valor);

    public static BemDeclaradoRecord ToRecord(this BemDeclarado domain)
        => new()
        {
            CandidatoCpf = domain.Candidato.Cpf,
            AnoEleicao = domain.AnoEleicao.Ano,
            CandidatoNome = domain.Candidato.Nome,
            CandidatoNomeUrna = domain.Candidato.NomeUrna,
            CandidatoNumero = domain.Candidato.Numero,
            Cargo = domain.Candidato.Cargo,
            PartidoNumero = domain.Candidato.Partido.Numero,
            PartidoSigla = domain.Candidato.Partido.Sigla,
            PartidoNome = domain.Candidato.Partido.Nome,
            UfSigla = domain.Candidato.UF.Sigla,
            TipoBem = domain.TipoBem,
            Descricao = domain.Descricao,
            Valor = domain.Valor
        };

    public static PrestacaoContas ToDomain(this PrestacaoContasRecord record)
        => new(
            ToCandidato(
                record.CandidatoCpf,
                record.CandidatoNome,
                record.CandidatoNomeUrna,
                record.CandidatoNumero,
                record.Cargo,
                record.PartidoNumero,
                record.PartidoSigla,
                record.PartidoNome,
                record.AnoEleicao,
                record.UfSigla),
            new AnoEleicao(record.AnoEleicao),
            record.TipoReceita,
            record.Descricao,
            record.Valor,
            record.TipoMovimentacao);

    public static PrestacaoContasRecord ToRecord(this PrestacaoContas domain)
        => new()
        {
            CandidatoCpf = domain.Candidato.Cpf,
            AnoEleicao = domain.AnoEleicao.Ano,
            CandidatoNome = domain.Candidato.Nome,
            CandidatoNomeUrna = domain.Candidato.NomeUrna,
            CandidatoNumero = domain.Candidato.Numero,
            Cargo = domain.Candidato.Cargo,
            PartidoNumero = domain.Candidato.Partido.Numero,
            PartidoSigla = domain.Candidato.Partido.Sigla,
            PartidoNome = domain.Candidato.Partido.Nome,
            UfSigla = domain.Candidato.UF.Sigla,
            TipoReceita = domain.TipoReceita,
            Descricao = domain.Descricao,
            Valor = domain.Valor,
            TipoMovimentacao = domain.TipoMovimentacao
        };

    public static DadoPopulacional ToDomain(this DadoPopulacionalRecord record)
        => new(ToMunicipio(record.MunicipioCodigoIbge, record.MunicipioNome, record.UfSigla), record.FaixaEtaria, record.Raca, record.Quantidade);

    public static DadoPopulacionalRecord ToRecord(this DadoPopulacional domain)
        => new()
        {
            MunicipioCodigoIbge = domain.Municipio.CodigoIbge,
            MunicipioNome = domain.Municipio.Nome,
            UfSigla = domain.Municipio.UF.Sigla,
            FaixaEtaria = domain.FaixaEtaria,
            Raca = domain.Raca,
            Quantidade = domain.Quantidade
        };

    public static DadoEscolaridade ToDomain(this DadoEscolaridadeRecord record)
        => new(ToMunicipio(record.MunicipioCodigoIbge, record.MunicipioNome, record.UfSigla), record.NivelEscolaridade, record.Quantidade);

    public static DadoEscolaridadeRecord ToRecord(this DadoEscolaridade domain)
        => new()
        {
            MunicipioCodigoIbge = domain.Municipio.CodigoIbge,
            MunicipioNome = domain.Municipio.Nome,
            UfSigla = domain.Municipio.UF.Sigla,
            NivelEscolaridade = domain.NivelEscolaridade,
            Quantidade = domain.Quantidade
        };

    public static DadoRenda ToDomain(this DadoRendaRecord record)
        => new(ToMunicipio(record.MunicipioCodigoIbge, record.MunicipioNome, record.UfSigla), record.FaixaRenda, record.Quantidade);

    public static DadoRendaRecord ToRecord(this DadoRenda domain)
        => new()
        {
            MunicipioCodigoIbge = domain.Municipio.CodigoIbge,
            MunicipioNome = domain.Municipio.Nome,
            UfSigla = domain.Municipio.UF.Sigla,
            FaixaRenda = domain.FaixaRenda,
            Quantidade = domain.Quantidade
        };

    public static DadoSaneamento ToDomain(this DadoSaneamentoRecord record)
        => new(ToMunicipio(record.MunicipioCodigoIbge, record.MunicipioNome, record.UfSigla), record.TipoSaneamento, record.DomiciliosAtendidos);

    public static DadoSaneamentoRecord ToRecord(this DadoSaneamento domain)
        => new()
        {
            MunicipioCodigoIbge = domain.Municipio.CodigoIbge,
            MunicipioNome = domain.Municipio.Nome,
            UfSigla = domain.Municipio.UF.Sigla,
            TipoSaneamento = domain.TipoSaneamento,
            DomiciliosAtendidos = domain.DomiciliosAtendidos
        };

    public static DadoUrbanizacao ToDomain(this DadoUrbanizacaoRecord record)
        => new(ToMunicipio(record.MunicipioCodigoIbge, record.MunicipioNome, record.UfSigla), record.TipoArea, record.Populacao);

    public static DadoUrbanizacaoRecord ToRecord(this DadoUrbanizacao domain)
        => new()
        {
            MunicipioCodigoIbge = domain.Municipio.CodigoIbge,
            MunicipioNome = domain.Municipio.Nome,
            UfSigla = domain.Municipio.UF.Sigla,
            TipoArea = domain.TipoArea,
            Populacao = domain.Populacao
        };

    public static DadoInfraestrutura ToDomain(this DadoInfraestruturaRecord record)
        => new(ToMunicipio(record.MunicipioCodigoIbge, record.MunicipioNome, record.UfSigla), record.TipoInfraestrutura, record.DomiciliosAtendidos);

    public static DadoInfraestruturaRecord ToRecord(this DadoInfraestrutura domain)
        => new()
        {
            MunicipioCodigoIbge = domain.Municipio.CodigoIbge,
            MunicipioNome = domain.Municipio.Nome,
            UfSigla = domain.Municipio.UF.Sigla,
            TipoInfraestrutura = domain.TipoInfraestrutura,
            DomiciliosAtendidos = domain.DomiciliosAtendidos
        };

    public static DadoDesemprego ToDomain(this DadoDesempregoRecord record)
        => new(new UF(record.UfSigla), new Trimestre(record.TrimestreAno, record.TrimestreNumero), record.TaxaDesemprego);

    public static DadoDesempregoRecord ToRecord(this DadoDesemprego domain)
        => new()
        {
            UfSigla = domain.UF.Sigla,
            TrimestreAno = domain.Trimestre.Ano,
            TrimestreNumero = domain.Trimestre.Numero,
            TaxaDesemprego = domain.TaxaDesemprego
        };

    public static DadoInformalidade ToDomain(this DadoInformalidadeRecord record)
        => new(new UF(record.UfSigla), new Trimestre(record.TrimestreAno, record.TrimestreNumero), record.TaxaInformalidade);

    public static DadoInformalidadeRecord ToRecord(this DadoInformalidade domain)
        => new()
        {
            UfSigla = domain.UF.Sigla,
            TrimestreAno = domain.Trimestre.Ano,
            TrimestreNumero = domain.Trimestre.Numero,
            TaxaInformalidade = domain.TaxaInformalidade
        };

    public static DadoRendaMedia ToDomain(this DadoRendaMediaRecord record)
        => new(new UF(record.UfSigla), new Trimestre(record.TrimestreAno, record.TrimestreNumero), record.RendaMedia);

    public static DadoRendaMediaRecord ToRecord(this DadoRendaMedia domain)
        => new()
        {
            UfSigla = domain.UF.Sigla,
            TrimestreAno = domain.Trimestre.Ano,
            TrimestreNumero = domain.Trimestre.Numero,
            RendaMedia = domain.RendaMedia
        };

    public static string SerializePartidos(IReadOnlyList<Partido> partidos)
        => JsonSerializer.Serialize(partidos.Select(x => new PartidoDto(x.Numero, x.Sigla, x.Nome)));

    public static IReadOnlyList<Partido> DeserializePartidos(string json)
        => JsonSerializer.Deserialize<List<PartidoDto>>(json)?.Select(x => new Partido(x.Numero, x.Sigla, x.Nome)).ToList()
           ?? [];

    private sealed record PartidoDto(int Numero, string Sigla, string Nome);
}
