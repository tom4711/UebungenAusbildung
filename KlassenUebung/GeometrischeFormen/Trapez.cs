internal class Trapez(double obereBasis, double untereBasis, double höhe) : GeometrischeForm
{
    public double ObereBasis { get; set; } = obereBasis;
    public double UntereBasis { get; set; } = untereBasis;
    public double Höhe { get; set; } = höhe;

    public override void BerechneFläche()
    {
        Fläche = (ObereBasis + UntereBasis) / 2 * Höhe;
    }

    public override void BerechneUmfang()
    {
        Umfang = ObereBasis + UntereBasis + 2 * Math.Sqrt(Math.Pow((UntereBasis - ObereBasis) / 2, 2) + Höhe * Höhe );
    }
}
