using System.Text;
using DadosTabulares.Domain.Ibge;
using DadosTabulares.Domain.Tse;
using Etl.Ibge;

namespace Etl.Tests.Ibge;

public sealed class IbgeCsvParserTests
{
    [Fact]
    public void ParsePopulacional_ConverteLinhaCsvEmEntidadeDeDominio()
    {
        var csv = """
                  codigo_ibge,municipio,uf,faixa_etaria,raca,quantidade
                  3550308,Sao Paulo,SP,0 a 4 anos,Branca,150000
                  """;

        using var csvStream = new MemoryStream(Encoding.UTF8.GetBytes(csv));
        var parser = new IbgeCsvParser();

        var dados = parser.ParsePopulacional(csvStream);

        var dado = Assert.Single(dados);
        Assert.Equal(new Municipio(3550308, "Sao Paulo", new UF("SP")), dado.Municipio);
        Assert.Equal("0 a 4 anos", dado.FaixaEtaria);
        Assert.Equal("Branca", dado.Raca);
        Assert.Equal(150000, dado.Quantidade);
    }

    [Fact]
    public void ParseEscolaridade_ConverteLinhaCsvEmEntidadeDeDominio()
    {
        var csv = """
                  codigo_ibge;municipio;uf;nivel_escolaridade;quantidade
                  3550308;Sao Paulo;SP;Ensino Medio Completo;200000
                  """;

        var parser = new IbgeCsvParser();

        var dados = parser.ParseEscolaridade(ToStream(csv));

        var dado = Assert.Single(dados);
        Assert.Equal(new Municipio(3550308, "Sao Paulo", new UF("SP")), dado.Municipio);
        Assert.Equal("Ensino Medio Completo", dado.NivelEscolaridade);
        Assert.Equal(200000, dado.Quantidade);
    }

    [Fact]
    public void ParseRenda_ConverteLinhaCsvEmEntidadeDeDominio()
    {
        var csv = """
                  codigo_ibge,municipio,uf,faixa_renda,quantidade
                  3550308,Sao Paulo,SP,1 a 2 salarios minimos,85000
                  """;

        var parser = new IbgeCsvParser();

        var dados = parser.ParseRenda(ToStream(csv));

        var dado = Assert.Single(dados);
        Assert.Equal(new Municipio(3550308, "Sao Paulo", new UF("SP")), dado.Municipio);
        Assert.Equal("1 a 2 salarios minimos", dado.FaixaRenda);
        Assert.Equal(85000, dado.Quantidade);
    }

    [Fact]
    public void ParseSaneamento_ConverteLinhaCsvEmEntidadeDeDominio()
    {
        var csv = """
                  codigo_ibge,municipio,uf,tipo_saneamento,domicilios_atendidos
                  3550308,Sao Paulo,SP,Rede geral de esgoto,320000
                  """;

        var parser = new IbgeCsvParser();

        var dados = parser.ParseSaneamento(ToStream(csv));

        var dado = Assert.Single(dados);
        Assert.Equal(new Municipio(3550308, "Sao Paulo", new UF("SP")), dado.Municipio);
        Assert.Equal("Rede geral de esgoto", dado.TipoSaneamento);
        Assert.Equal(320000, dado.DomiciliosAtendidos);
    }

    [Fact]
    public void ParseUrbanizacao_ConverteLinhaCsvEmEntidadeDeDominio()
    {
        var csv = """
                  codigo_ibge,municipio,uf,tipo_area,populacao
                  3550308,Sao Paulo,SP,Urbana,11200000
                  """;

        var parser = new IbgeCsvParser();

        var dados = parser.ParseUrbanizacao(ToStream(csv));

        var dado = Assert.Single(dados);
        Assert.Equal(new Municipio(3550308, "Sao Paulo", new UF("SP")), dado.Municipio);
        Assert.Equal("Urbana", dado.TipoArea);
        Assert.Equal(11200000, dado.Populacao);
    }

    [Fact]
    public void ParseInfraestrutura_ConverteLinhaCsvEmEntidadeDeDominio()
    {
        var csv = """
                  codigo_ibge,municipio,uf,tipo_infraestrutura,domicilios_atendidos
                  3550308,Sao Paulo,SP,Energia eletrica,450000
                  """;

        var parser = new IbgeCsvParser();

        var dados = parser.ParseInfraestrutura(ToStream(csv));

        var dado = Assert.Single(dados);
        Assert.Equal(new Municipio(3550308, "Sao Paulo", new UF("SP")), dado.Municipio);
        Assert.Equal("Energia eletrica", dado.TipoInfraestrutura);
        Assert.Equal(450000, dado.DomiciliosAtendidos);
    }

    private static MemoryStream ToStream(string csv)
        => new(Encoding.UTF8.GetBytes(csv));
}
