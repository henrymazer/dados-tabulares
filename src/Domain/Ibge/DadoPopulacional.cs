using DadosTabulares.Domain.Tse;

namespace DadosTabulares.Domain.Ibge;

public class DadoPopulacional
{
    public Municipio Municipio { get; }
    public string FaixaEtaria { get; }
    public string Raca { get; }
    public int Quantidade { get; }

    public DadoPopulacional(
        Municipio municipio,
        string faixaEtaria,
        string raca,
        int quantidade)
    {
        if (string.IsNullOrWhiteSpace(faixaEtaria))
            throw new ArgumentException("Faixa etária não pode ser vazia.", nameof(faixaEtaria));

        if (string.IsNullOrWhiteSpace(raca))
            throw new ArgumentException("Raça não pode ser vazia.", nameof(raca));

        if (quantidade < 0)
            throw new ArgumentException("Quantidade não pode ser negativa.", nameof(quantidade));

        Municipio = municipio ?? throw new ArgumentNullException(nameof(municipio));
        FaixaEtaria = faixaEtaria;
        Raca = raca;
        Quantidade = quantidade;
    }
}
