namespace Domain.Tests.Tse;

using DadosTabulares.Domain.Tse;

public class PrestacaoContasTests
{
    private static Candidato CriarCandidato() => new(
        cpf: "12345678901", nome: "João", nomeUrna: "João",
        numero: 1313, cargo: "Vereador",
        partido: new Partido(13, "PT", "Partido dos Trabalhadores"),
        anoEleicao: new AnoEleicao(2022), uf: new UF("SP"));

    [Fact]
    public void Deve_criar_prestacao_contas_valida()
    {
        var prestacao = new PrestacaoContas(
            candidato: CriarCandidato(),
            anoEleicao: new AnoEleicao(2022),
            tipoReceita: "Doação pessoa física",
            descricao: "Doação campanha",
            valor: 10_000.00m,
            tipoMovimentacao: "Receita");

        Assert.Equal("Doação pessoa física", prestacao.TipoReceita);
        Assert.Equal("Doação campanha", prestacao.Descricao);
        Assert.Equal(10_000.00m, prestacao.Valor);
        Assert.Equal("Receita", prestacao.TipoMovimentacao);
    }

    [Fact]
    public void Deve_rejeitar_tipo_receita_vazio()
    {
        Assert.Throws<ArgumentException>(() => new PrestacaoContas(
            candidato: CriarCandidato(), anoEleicao: new AnoEleicao(2022),
            tipoReceita: "", descricao: "Doação", valor: 100m,
            tipoMovimentacao: "Receita"));
    }

    [Fact]
    public void Deve_rejeitar_descricao_vazia()
    {
        Assert.Throws<ArgumentException>(() => new PrestacaoContas(
            candidato: CriarCandidato(), anoEleicao: new AnoEleicao(2022),
            tipoReceita: "Doação", descricao: "", valor: 100m,
            tipoMovimentacao: "Receita"));
    }

    [Fact]
    public void Deve_rejeitar_valor_negativo()
    {
        Assert.Throws<ArgumentException>(() => new PrestacaoContas(
            candidato: CriarCandidato(), anoEleicao: new AnoEleicao(2022),
            tipoReceita: "Doação", descricao: "Doação", valor: -1m,
            tipoMovimentacao: "Receita"));
    }

    [Fact]
    public void Deve_rejeitar_tipo_movimentacao_vazio()
    {
        Assert.Throws<ArgumentException>(() => new PrestacaoContas(
            candidato: CriarCandidato(), anoEleicao: new AnoEleicao(2022),
            tipoReceita: "Doação", descricao: "Doação", valor: 100m,
            tipoMovimentacao: ""));
    }
}
