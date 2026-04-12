namespace Domain.Tests.Tse;

using DadosTabulares.Domain.Tse;

public class CandidatoTests
{
    private static Partido CriarPartido() => new(13, "PT", "Partido dos Trabalhadores");

    [Fact]
    public void Deve_criar_candidato_valido()
    {
        var partido = CriarPartido();
        var candidato = new Candidato(
            cpf: "12345678901",
            nome: "João Silva",
            nomeUrna: "João",
            numero: 1313,
            cargo: "Vereador",
            partido: partido,
            anoEleicao: new AnoEleicao(2022),
            uf: new UF("SP"));

        Assert.Equal("12345678901", candidato.Cpf);
        Assert.Equal("João Silva", candidato.Nome);
        Assert.Equal("João", candidato.NomeUrna);
        Assert.Equal(1313, candidato.Numero);
        Assert.Equal("Vereador", candidato.Cargo);
        Assert.Equal(partido, candidato.Partido);
        Assert.Equal(2022, candidato.AnoEleicao.Ano);
        Assert.Equal("SP", candidato.UF.Sigla);
    }

    [Fact]
    public void Deve_rejeitar_nome_vazio()
    {
        Assert.Throws<ArgumentException>(() => new Candidato(
            cpf: "12345678901", nome: "", nomeUrna: "João",
            numero: 1313, cargo: "Vereador", partido: CriarPartido(),
            anoEleicao: new AnoEleicao(2022), uf: new UF("SP")));
    }

    [Fact]
    public void Deve_rejeitar_cpf_vazio()
    {
        Assert.Throws<ArgumentException>(() => new Candidato(
            cpf: "", nome: "João", nomeUrna: "João",
            numero: 1313, cargo: "Vereador", partido: CriarPartido(),
            anoEleicao: new AnoEleicao(2022), uf: new UF("SP")));
    }

    [Fact]
    public void Deve_rejeitar_numero_invalido()
    {
        Assert.Throws<ArgumentException>(() => new Candidato(
            cpf: "12345678901", nome: "João", nomeUrna: "João",
            numero: 0, cargo: "Vereador", partido: CriarPartido(),
            anoEleicao: new AnoEleicao(2022), uf: new UF("SP")));
    }

    [Fact]
    public void Deve_rejeitar_cargo_vazio()
    {
        Assert.Throws<ArgumentException>(() => new Candidato(
            cpf: "12345678901", nome: "João", nomeUrna: "João",
            numero: 1313, cargo: "", partido: CriarPartido(),
            anoEleicao: new AnoEleicao(2022), uf: new UF("SP")));
    }

    [Fact]
    public void Deve_permitir_nome_urna_vazio()
    {
        var candidato = new Candidato(
            cpf: "12345678901", nome: "João Silva", nomeUrna: "",
            numero: 1313, cargo: "Vereador", partido: CriarPartido(),
            anoEleicao: new AnoEleicao(2022), uf: new UF("SP"));
        Assert.Equal("", candidato.NomeUrna);
    }
}
