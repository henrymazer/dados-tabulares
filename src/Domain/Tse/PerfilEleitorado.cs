namespace DadosTabulares.Domain.Tse;

public class PerfilEleitorado
{
    public AnoEleicao AnoEleicao { get; }
    public ZonaEleitoral ZonaEleitoral { get; }
    public string FaixaEtaria { get; }
    public string Escolaridade { get; }
    public string Genero { get; }
    public int QuantidadeEleitores { get; }

    public PerfilEleitorado(
        AnoEleicao anoEleicao,
        ZonaEleitoral zonaEleitoral,
        string faixaEtaria,
        string escolaridade,
        string genero,
        int quantidadeEleitores)
    {
        if (string.IsNullOrWhiteSpace(faixaEtaria))
            throw new ArgumentException("Faixa etária não pode ser vazia.", nameof(faixaEtaria));

        if (string.IsNullOrWhiteSpace(escolaridade))
            throw new ArgumentException("Escolaridade não pode ser vazia.", nameof(escolaridade));

        if (string.IsNullOrWhiteSpace(genero))
            throw new ArgumentException("Gênero não pode ser vazio.", nameof(genero));

        if (quantidadeEleitores < 0)
            throw new ArgumentException("Quantidade de eleitores não pode ser negativa.", nameof(quantidadeEleitores));

        AnoEleicao = anoEleicao ?? throw new ArgumentNullException(nameof(anoEleicao));
        ZonaEleitoral = zonaEleitoral ?? throw new ArgumentNullException(nameof(zonaEleitoral));
        FaixaEtaria = faixaEtaria;
        Escolaridade = escolaridade;
        Genero = genero;
        QuantidadeEleitores = quantidadeEleitores;
    }
}
