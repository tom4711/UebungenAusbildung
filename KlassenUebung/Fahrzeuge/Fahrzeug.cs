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
