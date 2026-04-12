namespace DadosTabulares.Domain.Tse;

public class SecaoEleitoral
{
    public int NumeroSecao { get; }
    public ZonaEleitoral ZonaEleitoral { get; }

    public SecaoEleitoral(int numeroSecao, ZonaEleitoral zonaEleitoral)
    {
        if (numeroSecao <= 0)
            throw new ArgumentException("Número da seção eleitoral deve ser maior que zero.", nameof(numeroSecao));

        ZonaEleitoral = zonaEleitoral ?? throw new ArgumentNullException(nameof(zonaEleitoral));
        NumeroSecao = numeroSecao;
    }
}
