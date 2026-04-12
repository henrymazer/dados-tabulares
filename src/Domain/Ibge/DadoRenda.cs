using DadosTabulares.Domain.Tse;

namespace DadosTabulares.Domain.Ibge;

public class DadoRenda
{
    public Municipio Municipio { get; }
    public string FaixaRenda { get; }
    public int Quantidade { get; }

    public DadoRenda(
        Municipio municipio,
        string faixaRenda,
        int quantidade)
    {
        if (string.IsNullOrWhiteSpace(faixaRenda))
            throw new ArgumentException("Faixa de renda não pode ser vazia.", nameof(faixaRenda));

        if (quantidade < 0)
            throw new ArgumentException("Quantidade não pode ser negativa.", nameof(quantidade));

        Municipio = municipio ?? throw new ArgumentNullException(nameof(municipio));
        FaixaRenda = faixaRenda;
        Quantidade = quantidade;
    }
}
