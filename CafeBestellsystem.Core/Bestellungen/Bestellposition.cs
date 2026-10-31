using CafeBestellsystem.Core.Produkte;

namespace CafeBestellsystem.Core.Bestellungen;

public class Bestellposition
{
    public Bestellposition(Produkt produkt, int menge)
    {
        ArgumentNullException.ThrowIfNull(produkt);

        if (menge <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(menge), "Die Menge muss größer als 0 sein.");
        }

        Produkt = produkt;
        Menge = menge;
    }

    public Produkt Produkt { get; }

    public int Menge { get; }

    public decimal Positionspreis => Produkt.Preis * Menge;
}
