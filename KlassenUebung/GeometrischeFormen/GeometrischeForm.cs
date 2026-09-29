public abstract class GeometrischeForm
{
    public double Fläche { get; set; }
    public double Umfang { get; set; }

    public abstract void BerechneFläche();
    public abstract void BerechneUmfang();
}
