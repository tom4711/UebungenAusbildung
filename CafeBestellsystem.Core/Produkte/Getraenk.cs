namespace CafeBestellsystem.Core.Produkte;

public class Getraenk : Produkt
{
    public Getraenk(string name, decimal preis, bool istHeiss, int groesseInMl)
        : base(name, preis)
    {
        if (groesseInMl <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(groesseInMl), "Die Größe muss größer als 0 ml sein.");
        }

        IstHeiss = istHeiss;
        GroesseInMl = groesseInMl;
    }

    public bool IstHeiss { get; }

    public int GroesseInMl { get; }

    public override string Beschreibung()
    {
        var temperatur = IstHeiss ? "heiß" : "kalt";
        return $"{Name} ({temperatur}, {GroesseInMl} ml)";
    }
}
