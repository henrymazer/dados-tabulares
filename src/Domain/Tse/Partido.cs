namespace DadosTabulares.Domain.Tse;

public class Partido
{
    public int Numero { get; }
    public string Sigla { get; }
    public string Nome { get; }

    public Partido(int numero, string sigla, string nome)
    {
        if (numero <= 0)
            throw new ArgumentException("Número do partido deve ser maior que zero.", nameof(numero));

        if (string.IsNullOrWhiteSpace(sigla))
            throw new ArgumentException("Sigla do partido não pode ser vazia.", nameof(sigla));

        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome do partido não pode ser vazio.", nameof(nome));

        Numero = numero;
        Sigla = sigla;
        Nome = nome;
    }
}
