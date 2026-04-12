using DadosTabulares.Domain.Tse;

namespace DadosTabulares.Domain.Ibge;

public class DadoEscolaridade
{
    public Municipio Municipio { get; }
    public string NivelEscolaridade { get; }
    public int Quantidade { get; }

    public DadoEscolaridade(
        Municipio municipio,
        string nivelEscolaridade,
        int quantidade)
    {
        if (string.IsNullOrWhiteSpace(nivelEscolaridade))
            throw new ArgumentException("Nível de escolaridade não pode ser vazio.", nameof(nivelEscolaridade));

        if (quantidade < 0)
            throw new ArgumentException("Quantidade não pode ser negativa.", nameof(quantidade));

        Municipio = municipio ?? throw new ArgumentNullException(nameof(municipio));
        NivelEscolaridade = nivelEscolaridade;
        Quantidade = quantidade;
    }
}
