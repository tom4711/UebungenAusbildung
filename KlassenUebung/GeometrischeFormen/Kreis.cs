internal class Kreis(double radius) : GeometrischeForm
{
    public double Radius { get; set; } = radius;

    public override void BerechneFläche()
    {
        Fläche = Math.PI * Radius * Radius;
    }

    public override void BerechneUmfang()
    {
        Umfang = 2 * Math.PI * Radius;
    }
}
