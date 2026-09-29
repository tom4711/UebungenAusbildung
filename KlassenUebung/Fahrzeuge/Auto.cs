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
