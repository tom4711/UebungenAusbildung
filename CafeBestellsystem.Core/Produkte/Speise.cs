namespace CafeBestellsystem.Core.Produkte;

public class Speise : Produkt
{
    public Speise(string name, decimal preis, bool istVegetarisch)
        : base(name, preis)
    {
        IstVegetarisch = istVegetarisch;
    }

    public bool IstVegetarisch { get; }

    public override string Beschreibung()
    {
        var kennzeichnung = IstVegetarisch ? "vegetarisch" : "nicht vegetarisch";
        return $"{Name} ({kennzeichnung})";
    }
}
