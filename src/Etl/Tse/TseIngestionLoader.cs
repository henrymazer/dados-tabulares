using System.Text.Json;
using Data;
using DadosTabulares.Domain.Tse;
using Microsoft.EntityFrameworkCore;

namespace DadosTabulares.Etl.Tse;

public sealed class TseIngestionLoader(PublicDataDbContext context)
{
    public async Task<int> IngerirResultadosAsync(IReadOnlyList<ResultadoEleitoral> resultados, CancellationToken ct = default)
        => await IngerirAsync(
            context.ResultadosEleitorais,
            resultados.Select(ToRecord).ToList(),
            static record => new ResultadoKey(
                record.CandidatoCpf,
                record.AnoEleicao,
                record.Turno,
                record.NumeroZona,
                record.MunicipioCodigoIbge,
                record.NumeroSecao),
            ct);

    public async Task<int> IngerirEleitoradoAsync(IReadOnlyList<PerfilEleitorado> perfis, CancellationToken ct = default)
        => await IngerirAsync(
            context.PerfisEleitorado,
            perfis.Select(ToRecord).ToList(),
            static record => new PerfilKey(
                record.AnoEleicao,
                record.NumeroZona,
                record.MunicipioCodigoIbge,
                record.FaixaEtaria,
                record.Escolaridade,
                record.Genero),
            ct);

    public async Task<int> IngerirBensAsync(IReadOnlyList<BemDeclarado> bens, CancellationToken ct = default)
        => await IngerirAsync(
            context.BensDeclarados,
            bens.Select(ToRecord).ToList(),
            static record => new BemKey(
                record.CandidatoCpf,
                record.AnoEleicao,
                record.TipoBem,
                record.Descricao),
            ct);

    public async Task<int> IngerirPrestacoesContasAsync(IReadOnlyList<PrestacaoContas> prestacoes, CancellationToken ct = default)
        => await IngerirAsync(
            context.PrestacoesContas,
            prestacoes.Select(ToRecord).ToList(),
            static record => new PrestacaoKey(
                record.CandidatoCpf,
                record.AnoEleicao,
                record.TipoReceita,
                record.Descricao,
                record.TipoMovimentacao),
            ct);

    public async Task<int> IngerirColigacoesAsync(IReadOnlyList<Coligacao> coligacoes, CancellationToken ct = default)
        => await IngerirAsync(
            context.Set<ColigacaoRecord>(),
            coligacoes.Select(ToRecord).ToList(),
            static record => new ColigacaoKey(record.Nome, record.AnoEleicao),
            ct);

    private async Task<int> IngerirAsync<TRecord, TKey>(
        DbSet<TRecord> set,
        IReadOnlyList<TRecord> records,
        Func<TRecord, TKey> keySelector,
        CancellationToken ct)
        where TRecord : class
        where TKey : notnull
    {
        if (records.Count == 0)
            return 0;

        var existingKeys = (await set
                .AsNoTracking()
                .ToListAsync(ct))
            .Select(keySelector)
            .ToHashSet();

        var novos = new List<TRecord>();
        foreach (var record in records)
        {
            if (existingKeys.Add(keySelector(record)))
                novos.Add(record);
        }

        if (novos.Count == 0)
            return 0;

        await set.AddRangeAsync(novos, ct);
        await context.SaveChangesAsync(ct);
        return novos.Count;
    }

    private static ResultadoEleitoralRecord ToRecord(ResultadoEleitoral domain)
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

    private static PerfilEleitoradoRecord ToRecord(PerfilEleitorado domain)
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

    private static BemDeclaradoRecord ToRecord(BemDeclarado domain)
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

    private static PrestacaoContasRecord ToRecord(PrestacaoContas domain)
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

    private static ColigacaoRecord ToRecord(Coligacao domain)
        => new()
        {
            Nome = domain.Nome,
            AnoEleicao = domain.AnoEleicao.Ano,
            PartidosJson = JsonSerializer.Serialize(domain.Partidos.Select(partido => new PartidoDto(partido.Numero, partido.Sigla, partido.Nome)))
        };

    private readonly record struct ResultadoKey(string Cpf, int Ano, int Turno, int Zona, int Municipio, int Secao);

    private readonly record struct PerfilKey(int Ano, int Zona, int Municipio, string FaixaEtaria, string Escolaridade, string Genero);

    private readonly record struct BemKey(string Cpf, int Ano, string TipoBem, string Descricao);

    private readonly record struct PrestacaoKey(string Cpf, int Ano, string TipoReceita, string Descricao, string TipoMovimentacao);

    private readonly record struct ColigacaoKey(string Nome, int Ano);

    private sealed record PartidoDto(int Numero, string Sigla, string Nome);
}
