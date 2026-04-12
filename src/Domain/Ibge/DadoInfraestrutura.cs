using DadosTabulares.Domain.Tse;

namespace DadosTabulares.Domain.Ibge;

public class DadoInfraestrutura
{
    public Municipio Municipio { get; }
    public string TipoInfraestrutura { get; }
    public int DomiciliosAtendidos { get; }

    public DadoInfraestrutura(
        Municipio municipio,
        string tipoInfraestrutura,
        int domiciliosAtendidos)
    {
        if (string.IsNullOrWhiteSpace(tipoInfraestrutura))
            throw new ArgumentException("Tipo de infraestrutura não pode ser vazio.", nameof(tipoInfraestrutura));

        if (domiciliosAtendidos < 0)
            throw new ArgumentException("Domicílios atendidos não pode ser negativo.", nameof(domiciliosAtendidos));

        Municipio = municipio ?? throw new ArgumentNullException(nameof(municipio));
        TipoInfraestrutura = tipoInfraestrutura;
        DomiciliosAtendidos = domiciliosAtendidos;
    }
}
