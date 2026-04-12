namespace DadosTabulares.Domain.Tse;

public class ZonaEleitoral
{
    public int NumeroZona { get; }
    public Municipio Municipio { get; }

    public ZonaEleitoral(int numeroZona, Municipio municipio)
    {
        if (numeroZona <= 0)
            throw new ArgumentException("Número da zona eleitoral deve ser maior que zero.", nameof(numeroZona));

        Municipio = municipio ?? throw new ArgumentNullException(nameof(municipio));
        NumeroZona = numeroZona;
    }
}
