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
