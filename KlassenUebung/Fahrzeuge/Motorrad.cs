internal class Motorrad : Fahrzeug
{
    public string Marke { get; set; }
    public string Modell { get; set; }
    public void Wheelie()
    {
        Console.WriteLine("Das Motorrad macht einen Wheelie.");
    }
}
