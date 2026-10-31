namespace CafeBestellsystem.Core.Produkte;

public abstract class Produkt
{
    protected Produkt(string name, decimal preis)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Der Produktname darf nicht leer sein.", nameof(name));
        }

        if (preis < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(preis), "Der Preis darf nicht negativ sein.");
        }

        Name = name.Trim();
        Preis = preis;
    }

    public string Name { get; }

    public decimal Preis { get; }

    public abstract string Beschreibung();
}
