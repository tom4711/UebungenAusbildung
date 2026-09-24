internal class Program
{
    private static void Main(string[] args)
    {
        var auto = new Auto();
        var motorrad = new Motorrad();
        var lkw = new LKW();
        var flugzeug = new Flugzeug();

        var fahrzeuge = new List<Fahrzeug>
        {
            auto,
            motorrad,
            lkw,
            flugzeug
        };
    }
}

internal abstract class Fahrzeug
{
    public int Baujahr { get; set; }
    public string Farbe { get; set; }
    public int Geschwindigkeit { get; set; }
    public int MaxGeschwindigkeit { get; set; }
    public double Tankinhalt { get; set; }
    public double Kraftstoffverbrauch { get; set; }

    public void Beschleunigen()
    {
        Console.WriteLine("Das Fahrzeug beschleunigt.");
    }
    public void Bremsen()
    {
        Console.WriteLine("Das Fahrzeug bremst.");
    }

}

internal class Auto : Fahrzeug
{
    public int AnzahlTueren { get; set; }
    public string Marke { get; set; }
    public string Modell { get; set; }
    public double Kofferraumvolumen { get; set; }
    public bool Schiebedach { get; set; }
    public void Hupe()
    {
        Console.WriteLine("Das Auto hupt.");
    }
}
internal class Motorrad : Fahrzeug
{
    public string Marke { get; set; }
    public string Modell { get; set; }
    public void Wheelie()
    {
        Console.WriteLine("Das Motorrad macht einen Wheelie.");
    }
}
internal class LKW : Fahrzeug
{
    public string Marke { get; set; }
    public string Modell { get; set; }
    public double Ladevolumen { get; set; }
    public void LadegutAufnehmen()
    {
        Console.WriteLine("Der LKW nimmt Ladegut auf.");
    }
}
internal class Flugzeug : Fahrzeug
{
    public string Marke { get; set; }
    public string Modell { get; set; }
    public double Spannweite { get; set; }
    public double Reichweite { get; set; }
    public double Flughöhe { get; set; }
    public void Starten()
    {
        Console.WriteLine("Das Flugzeug startet.");
    }
}
