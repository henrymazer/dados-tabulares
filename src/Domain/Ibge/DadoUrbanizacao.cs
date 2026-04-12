using DadosTabulares.Domain.Tse;

namespace DadosTabulares.Domain.Ibge;

public class DadoUrbanizacao
{
    public Municipio Municipio { get; }
    public string TipoArea { get; }
    public int Populacao { get; }

    public DadoUrbanizacao(
        Municipio municipio,
        string tipoArea,
        int populacao)
    {
        if (string.IsNullOrWhiteSpace(tipoArea))
            throw new ArgumentException("Tipo de área não pode ser vazio.", nameof(tipoArea));

        if (populacao < 0)
            throw new ArgumentException("População não pode ser negativa.", nameof(populacao));

        Municipio = municipio ?? throw new ArgumentNullException(nameof(municipio));
        TipoArea = tipoArea;
        Populacao = populacao;
    }
}
