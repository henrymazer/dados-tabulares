using NetTopologySuite.Geometries;
using Microsoft.EntityFrameworkCore;

namespace Data;

public sealed class PublicDataDbContext(DbContextOptions<PublicDataDbContext> options) : DbContext(options)
{
    public DbSet<UfRecord> Ufs => Set<UfRecord>();
    public DbSet<AnoEleicaoRecord> AnosEleicao => Set<AnoEleicaoRecord>();
    public DbSet<MunicipioRecord> Municipios => Set<MunicipioRecord>();
    public DbSet<PartidoRecord> Partidos => Set<PartidoRecord>();
    public DbSet<CandidatoRecord> Candidatos => Set<CandidatoRecord>();
    public DbSet<ColigacaoRecord> Coligacoes => Set<ColigacaoRecord>();
    public DbSet<ZonaEleitoralRecord> ZonasEleitorais => Set<ZonaEleitoralRecord>();
    public DbSet<SecaoEleitoralRecord> SecoesEleitorais => Set<SecaoEleitoralRecord>();
    public DbSet<ResultadoEleitoralRecord> ResultadosEleitorais => Set<ResultadoEleitoralRecord>();
    public DbSet<PerfilEleitoradoRecord> PerfisEleitorado => Set<PerfilEleitoradoRecord>();
    public DbSet<BemDeclaradoRecord> BensDeclarados => Set<BemDeclaradoRecord>();
    public DbSet<PrestacaoContasRecord> PrestacoesContas => Set<PrestacaoContasRecord>();
    public DbSet<DadoPopulacionalRecord> DadosPopulacionais => Set<DadoPopulacionalRecord>();
    public DbSet<DadoEscolaridadeRecord> DadosEscolaridade => Set<DadoEscolaridadeRecord>();
    public DbSet<DadoRendaRecord> DadosRenda => Set<DadoRendaRecord>();
    public DbSet<DadoSaneamentoRecord> DadosSaneamento => Set<DadoSaneamentoRecord>();
    public DbSet<DadoUrbanizacaoRecord> DadosUrbanizacao => Set<DadoUrbanizacaoRecord>();
    public DbSet<DadoInfraestruturaRecord> DadosInfraestrutura => Set<DadoInfraestruturaRecord>();
    public DbSet<SetorCensitarioRecord> SetoresCensitarios => Set<SetorCensitarioRecord>();
    public DbSet<TrimestreRecord> Trimestres => Set<TrimestreRecord>();
    public DbSet<DadoDesempregoRecord> DadosDesemprego => Set<DadoDesempregoRecord>();
    public DbSet<DadoInformalidadeRecord> DadosInformalidade => Set<DadoInformalidadeRecord>();
    public DbSet<DadoRendaMediaRecord> DadosRendaMedia => Set<DadoRendaMediaRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureTse(modelBuilder);
        ConfigureIbge(modelBuilder);
        ConfigurePnad(modelBuilder);
    }

    private static void ConfigureTse(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("public");

        modelBuilder.Entity<UfRecord>(entity =>
        {
            entity.ToTable("ufs", "tse");
            entity.HasKey(x => x.Sigla);
            entity.Property(x => x.Sigla).HasMaxLength(2).IsRequired();
        });

        modelBuilder.Entity<AnoEleicaoRecord>(entity =>
        {
            entity.ToTable("anos_eleicao", "tse");
            entity.HasKey(x => x.Ano);
        });

        modelBuilder.Entity<MunicipioRecord>(entity =>
        {
            entity.ToTable("municipios", "tse");
            entity.HasKey(x => x.CodigoIbge);
            entity.Property(x => x.Nome).HasMaxLength(200).IsRequired();
            entity.Property(x => x.UfSigla).HasMaxLength(2).IsRequired();
            entity.HasIndex(x => new { x.UfSigla, x.Nome });
        });

        modelBuilder.Entity<PartidoRecord>(entity =>
        {
            entity.ToTable("partidos", "tse");
            entity.HasKey(x => x.Numero);
            entity.Property(x => x.Sigla).HasMaxLength(20).IsRequired();
            entity.Property(x => x.Nome).HasMaxLength(200).IsRequired();
            entity.HasIndex(x => x.Sigla).IsUnique();
        });

        modelBuilder.Entity<CandidatoRecord>(entity =>
        {
            entity.ToTable("candidatos", "tse");
            entity.HasKey(x => new { x.Cpf, x.AnoEleicao });
            entity.Property(x => x.Cpf).HasMaxLength(11).IsRequired();
            entity.Property(x => x.Nome).HasMaxLength(200).IsRequired();
            entity.Property(x => x.NomeUrna).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Cargo).HasMaxLength(100).IsRequired();
            entity.Property(x => x.PartidoSigla).HasMaxLength(20).IsRequired();
            entity.Property(x => x.PartidoNome).HasMaxLength(200).IsRequired();
            entity.Property(x => x.UfSigla).HasMaxLength(2).IsRequired();
        });

        modelBuilder.Entity<ColigacaoRecord>(entity =>
        {
            entity.ToTable("coligacoes", "tse");
            entity.HasKey(x => new { x.Nome, x.AnoEleicao });
            entity.Property(x => x.Nome).HasMaxLength(200).IsRequired();
            entity.Property(x => x.PartidosJson).HasColumnType("jsonb").IsRequired();
        });

        modelBuilder.Entity<ZonaEleitoralRecord>(entity =>
        {
            entity.ToTable("zonas_eleitorais", "tse");
            entity.HasKey(x => new { x.NumeroZona, x.MunicipioCodigoIbge });
            entity.Property(x => x.MunicipioNome).HasMaxLength(200).IsRequired();
            entity.Property(x => x.UfSigla).HasMaxLength(2).IsRequired();
        });

        modelBuilder.Entity<SecaoEleitoralRecord>(entity =>
        {
            entity.ToTable("secoes_eleitorais", "tse");
            entity.HasKey(x => new { x.NumeroSecao, x.NumeroZona, x.MunicipioCodigoIbge });
            entity.Property(x => x.MunicipioNome).HasMaxLength(200).IsRequired();
            entity.Property(x => x.UfSigla).HasMaxLength(2).IsRequired();
        });

        modelBuilder.Entity<ResultadoEleitoralRecord>(entity =>
        {
            entity.ToTable("resultados_eleitorais", "tse");
            entity.HasKey(x => new { x.CandidatoCpf, x.AnoEleicao, x.Turno, x.NumeroZona, x.MunicipioCodigoIbge, x.NumeroSecao });
            entity.Property(x => x.CandidatoCpf).HasMaxLength(11).IsRequired();
            entity.Property(x => x.CandidatoNome).HasMaxLength(200).IsRequired();
            entity.Property(x => x.CandidatoNomeUrna).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Cargo).HasMaxLength(100).IsRequired();
            entity.Property(x => x.PartidoSigla).HasMaxLength(20).IsRequired();
            entity.Property(x => x.PartidoNome).HasMaxLength(200).IsRequired();
            entity.Property(x => x.UfSigla).HasMaxLength(2).IsRequired();
            entity.Property(x => x.MunicipioNome).HasMaxLength(200).IsRequired();
            entity.HasIndex(x => new { x.MunicipioCodigoIbge, x.AnoEleicao });
            entity.HasIndex(x => new { x.NumeroZona, x.MunicipioCodigoIbge, x.AnoEleicao });
            entity.HasIndex(x => new { x.PartidoNumero, x.AnoEleicao });
        });

        modelBuilder.Entity<PerfilEleitoradoRecord>(entity =>
        {
            entity.ToTable("perfis_eleitorado", "tse");
            entity.HasKey(x => new { x.AnoEleicao, x.NumeroZona, x.MunicipioCodigoIbge, x.FaixaEtaria, x.Escolaridade, x.Genero });
            entity.Property(x => x.MunicipioNome).HasMaxLength(200).IsRequired();
            entity.Property(x => x.UfSigla).HasMaxLength(2).IsRequired();
            entity.Property(x => x.FaixaEtaria).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Escolaridade).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Genero).HasMaxLength(50).IsRequired();
            entity.HasIndex(x => new { x.MunicipioCodigoIbge, x.AnoEleicao });
        });

        modelBuilder.Entity<BemDeclaradoRecord>(entity =>
        {
            entity.ToTable("bens_declarados", "tse");
            entity.HasKey(x => new { x.CandidatoCpf, x.AnoEleicao, x.TipoBem, x.Descricao });
            entity.Property(x => x.CandidatoCpf).HasMaxLength(11).IsRequired();
            entity.Property(x => x.CandidatoNome).HasMaxLength(200).IsRequired();
            entity.Property(x => x.CandidatoNomeUrna).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Cargo).HasMaxLength(100).IsRequired();
            entity.Property(x => x.PartidoSigla).HasMaxLength(20).IsRequired();
            entity.Property(x => x.PartidoNome).HasMaxLength(200).IsRequired();
            entity.Property(x => x.UfSigla).HasMaxLength(2).IsRequired();
            entity.Property(x => x.TipoBem).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Descricao).HasMaxLength(500).IsRequired();
            entity.Property(x => x.Valor).HasPrecision(18, 2);
        });

        modelBuilder.Entity<PrestacaoContasRecord>(entity =>
        {
            entity.ToTable("prestacoes_contas", "tse");
            entity.HasKey(x => new { x.CandidatoCpf, x.AnoEleicao, x.TipoReceita, x.Descricao, x.TipoMovimentacao });
            entity.Property(x => x.CandidatoCpf).HasMaxLength(11).IsRequired();
            entity.Property(x => x.CandidatoNome).HasMaxLength(200).IsRequired();
            entity.Property(x => x.CandidatoNomeUrna).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Cargo).HasMaxLength(100).IsRequired();
            entity.Property(x => x.PartidoSigla).HasMaxLength(20).IsRequired();
            entity.Property(x => x.PartidoNome).HasMaxLength(200).IsRequired();
            entity.Property(x => x.UfSigla).HasMaxLength(2).IsRequired();
            entity.Property(x => x.TipoReceita).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Descricao).HasMaxLength(500).IsRequired();
            entity.Property(x => x.TipoMovimentacao).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Valor).HasPrecision(18, 2);
        });
    }

    private static void ConfigureIbge(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DadoPopulacionalRecord>(entity =>
        {
            entity.ToTable("dados_populacionais", "ibge");
            entity.HasKey(x => new { x.MunicipioCodigoIbge, x.FaixaEtaria, x.Raca });
            entity.Property(x => x.MunicipioNome).HasMaxLength(200).IsRequired();
            entity.Property(x => x.UfSigla).HasMaxLength(2).IsRequired();
            entity.Property(x => x.FaixaEtaria).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Raca).HasMaxLength(100).IsRequired();
            entity.HasIndex(x => x.UfSigla);
        });

        modelBuilder.Entity<DadoEscolaridadeRecord>(entity =>
        {
            entity.ToTable("dados_escolaridade", "ibge");
            entity.HasKey(x => new { x.MunicipioCodigoIbge, x.NivelEscolaridade });
            entity.Property(x => x.MunicipioNome).HasMaxLength(200).IsRequired();
            entity.Property(x => x.UfSigla).HasMaxLength(2).IsRequired();
            entity.Property(x => x.NivelEscolaridade).HasMaxLength(100).IsRequired();
            entity.HasIndex(x => x.UfSigla);
        });

        modelBuilder.Entity<DadoRendaRecord>(entity =>
        {
            entity.ToTable("dados_renda", "ibge");
            entity.HasKey(x => new { x.MunicipioCodigoIbge, x.FaixaRenda });
            entity.Property(x => x.MunicipioNome).HasMaxLength(200).IsRequired();
            entity.Property(x => x.UfSigla).HasMaxLength(2).IsRequired();
            entity.Property(x => x.FaixaRenda).HasMaxLength(100).IsRequired();
            entity.HasIndex(x => x.UfSigla);
        });

        modelBuilder.Entity<DadoSaneamentoRecord>(entity =>
        {
            entity.ToTable("dados_saneamento", "ibge");
            entity.HasKey(x => new { x.MunicipioCodigoIbge, x.TipoSaneamento });
            entity.Property(x => x.MunicipioNome).HasMaxLength(200).IsRequired();
            entity.Property(x => x.UfSigla).HasMaxLength(2).IsRequired();
            entity.Property(x => x.TipoSaneamento).HasMaxLength(100).IsRequired();
            entity.HasIndex(x => x.UfSigla);
        });

        modelBuilder.Entity<DadoUrbanizacaoRecord>(entity =>
        {
            entity.ToTable("dados_urbanizacao", "ibge");
            entity.HasKey(x => new { x.MunicipioCodigoIbge, x.TipoArea });
            entity.Property(x => x.MunicipioNome).HasMaxLength(200).IsRequired();
            entity.Property(x => x.UfSigla).HasMaxLength(2).IsRequired();
            entity.Property(x => x.TipoArea).HasMaxLength(100).IsRequired();
            entity.HasIndex(x => x.UfSigla);
        });

        modelBuilder.Entity<DadoInfraestruturaRecord>(entity =>
        {
            entity.ToTable("dados_infraestrutura", "ibge");
            entity.HasKey(x => new { x.MunicipioCodigoIbge, x.TipoInfraestrutura });
            entity.Property(x => x.MunicipioNome).HasMaxLength(200).IsRequired();
            entity.Property(x => x.UfSigla).HasMaxLength(2).IsRequired();
            entity.Property(x => x.TipoInfraestrutura).HasMaxLength(100).IsRequired();
            entity.HasIndex(x => x.UfSigla);
        });

        modelBuilder.Entity<SetorCensitarioRecord>(entity =>
        {
            entity.ToTable("setores_censitarios", "ibge");
            entity.HasKey(x => x.CodigoSetor);
            entity.Property(x => x.CodigoSetor).HasMaxLength(15).IsRequired();
            entity.Property(x => x.MunicipioNome).HasMaxLength(200).IsRequired();
            entity.Property(x => x.UfSigla).HasMaxLength(2).IsRequired();
            entity.Property(x => x.Geometria)
                .HasColumnType("geometry(MultiPolygon,4674)")
                .IsRequired();
            entity.HasIndex(x => x.MunicipioCodigoIbge);
            entity.HasIndex(x => x.UfSigla);
            entity.HasIndex(x => x.Geometria).HasMethod("gist");
        });
    }

    private static void ConfigurePnad(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TrimestreRecord>(entity =>
        {
            entity.ToTable("trimestres", "pnad");
            entity.HasKey(x => new { x.Ano, x.Numero });
        });

        modelBuilder.Entity<DadoDesempregoRecord>(entity =>
        {
            entity.ToTable("dados_desemprego", "pnad");
            entity.HasKey(x => new { x.UfSigla, x.TrimestreAno, x.TrimestreNumero });
            entity.Property(x => x.UfSigla).HasMaxLength(2).IsRequired();
            entity.Property(x => x.TaxaDesemprego).HasPrecision(8, 2);
            entity.HasIndex(x => new { x.TrimestreAno, x.TrimestreNumero });
        });

        modelBuilder.Entity<DadoInformalidadeRecord>(entity =>
        {
            entity.ToTable("dados_informalidade", "pnad");
            entity.HasKey(x => new { x.UfSigla, x.TrimestreAno, x.TrimestreNumero });
            entity.Property(x => x.UfSigla).HasMaxLength(2).IsRequired();
            entity.Property(x => x.TaxaInformalidade).HasPrecision(8, 2);
            entity.HasIndex(x => new { x.TrimestreAno, x.TrimestreNumero });
        });

        modelBuilder.Entity<DadoRendaMediaRecord>(entity =>
        {
            entity.ToTable("dados_renda_media", "pnad");
            entity.HasKey(x => new { x.UfSigla, x.TrimestreAno, x.TrimestreNumero });
            entity.Property(x => x.UfSigla).HasMaxLength(2).IsRequired();
            entity.Property(x => x.RendaMedia).HasPrecision(18, 2);
            entity.HasIndex(x => new { x.TrimestreAno, x.TrimestreNumero });
        });
    }
}
