internal class Ellipse(double halbachseA, double halbachseB) : GeometrischeForm
{
    public double HalbachseA { get; set; } = halbachseA;
    public double HalbachseB { get; set; } = halbachseB;

    public override void BerechneFläche()
    {
        Fläche = Math.PI * HalbachseA * HalbachseB;
    }

    public override void BerechneUmfang()
    {
        // Näherung nach Ramanujan
        Umfang = Math.PI * (3 * (HalbachseA + HalbachseB) - Math.Sqrt((3 * HalbachseA + HalbachseB) * (HalbachseA + 3 * HalbachseB)));
    }
}
