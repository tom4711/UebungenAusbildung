internal class Dreieck(double seiteA, double seiteB, double seiteC) : GeometrischeForm
{
    public double SeiteA { get; set; } = seiteA;
    public double SeiteB { get; set; } = seiteB;
    public double SeiteC { get; set; } = seiteC;

    public override void BerechneFläche()
    {
        double s = (SeiteA + SeiteB + SeiteC) / 2;
        Fläche = Math.Sqrt(s * (s - SeiteA) * (s - SeiteB) * (s - SeiteC));
    }

    public override void BerechneUmfang()
    {
        Umfang = SeiteA + SeiteB + SeiteC;
    }
}
