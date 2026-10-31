namespace CafeBestellsystem.Core.Produkte;

public class Kaffee : Getraenk
{
    public Kaffee(string name, decimal preis, bool istHeiss, int groesseInMl, string bohnenSorte)
        : base(name, preis, istHeiss, groesseInMl)
    {
        if (string.IsNullOrWhiteSpace(bohnenSorte))
        {
            throw new ArgumentException("Die Bohnensorte darf nicht leer sein.", nameof(bohnenSorte));
        }

        BohnenSorte = bohnenSorte.Trim();
    }

    public string BohnenSorte { get; }

    public override string Beschreibung()
    {
        return $"{base.Beschreibung()}, Bohnen: {BohnenSorte}";
    }
}
