internal class Rechteck(double länge, double breite) : GeometrischeForm
{
    public double Länge { get; set; } = länge;
    public double Breite { get; set; } = breite;

    public override void BerechneFläche()
    {
        Fläche = Länge * Breite;
    }

    public override void BerechneUmfang()
    {
        Umfang = 2 * (Länge + Breite);
    }
}
