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
