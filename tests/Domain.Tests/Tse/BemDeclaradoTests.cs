namespace Domain.Tests.Tse;

using DadosTabulares.Domain.Tse;

public class BemDeclaradoTests
{
    private static Candidato CriarCandidato() => new(
        cpf: "12345678901", nome: "João", nomeUrna: "João",
        numero: 1313, cargo: "Vereador",
        partido: new Partido(13, "PT", "Partido dos Trabalhadores"),
        anoEleicao: new AnoEleicao(2022), uf: new UF("SP"));

    [Fact]
    public void Deve_criar_bem_declarado_valido()
    {
        var bem = new BemDeclarado(
            candidato: CriarCandidato(),
            anoEleicao: new AnoEleicao(2022),
            tipoBem: "Imóvel",
            descricao: "Apartamento em São Paulo",
            valor: 500_000.00m);

        Assert.Equal("Imóvel", bem.TipoBem);
        Assert.Equal("Apartamento em São Paulo", bem.Descricao);
        Assert.Equal(500_000.00m, bem.Valor);
    }

    [Fact]
    public void Deve_rejeitar_tipo_bem_vazio()
    {
        Assert.Throws<ArgumentException>(() => new BemDeclarado(
            candidato: CriarCandidato(), anoEleicao: new AnoEleicao(2022),
            tipoBem: "", descricao: "Apto", valor: 100m));
    }

    [Fact]
    public void Deve_rejeitar_descricao_vazia()
    {
        Assert.Throws<ArgumentException>(() => new BemDeclarado(
            candidato: CriarCandidato(), anoEleicao: new AnoEleicao(2022),
            tipoBem: "Imóvel", descricao: "", valor: 100m));
    }

    [Fact]
    public void Deve_rejeitar_valor_negativo()
    {
        Assert.Throws<ArgumentException>(() => new BemDeclarado(
            candidato: CriarCandidato(), anoEleicao: new AnoEleicao(2022),
            tipoBem: "Imóvel", descricao: "Apto", valor: -1m));
    }

    [Fact]
    public void Deve_aceitar_valor_zero()
    {
        var bem = new BemDeclarado(
            candidato: CriarCandidato(), anoEleicao: new AnoEleicao(2022),
            tipoBem: "Outros", descricao: "Sem valor", valor: 0m);

        Assert.Equal(0m, bem.Valor);
    }
}
