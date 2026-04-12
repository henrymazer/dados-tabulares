using DadosTabulares.Domain.Tse;

namespace DadosTabulares.Domain.Ibge;

public class DadoSaneamento
{
    public Municipio Municipio { get; }
    public string TipoSaneamento { get; }
    public int DomiciliosAtendidos { get; }

    public DadoSaneamento(
        Municipio municipio,
        string tipoSaneamento,
        int domiciliosAtendidos)
    {
        if (string.IsNullOrWhiteSpace(tipoSaneamento))
            throw new ArgumentException("Tipo de saneamento não pode ser vazio.", nameof(tipoSaneamento));

        if (domiciliosAtendidos < 0)
            throw new ArgumentException("Domicílios atendidos não pode ser negativo.", nameof(domiciliosAtendidos));

        Municipio = municipio ?? throw new ArgumentNullException(nameof(municipio));
        TipoSaneamento = tipoSaneamento;
        DomiciliosAtendidos = domiciliosAtendidos;
    }
}
