internal class Program
{
    private static void Main(string[] args)
    {
        var auto = new Auto();
        var motorrad = new Motorrad();
        var lkw = new LKW();
        var flugzeug = new Flugzeug();

        var fahrzeuge = new List<Fahrzeug>
        {
            auto,
            motorrad,
            lkw,
            flugzeug
        };

        List<GeometrischeForm> geometrischeFormen =
        [
            new Kreis(5),
            new Rechteck(4, 6),
            new Dreieck(3, 4, 5),
            new Quadrat(5),
            new Trapez(3, 5, 4),
            new Ellipse(3, 5),
        ];

        foreach (var form in geometrischeFormen)
        {
            form.BerechneFläche();
            form.BerechneUmfang();
       
            switch (form)
            {
                case Kreis kreis:
                    Console.WriteLine("Kreis:");
                    Console.WriteLine($"Fläche: {kreis.Fläche}");
                    Console.WriteLine($"Umfang: {kreis.Umfang}");
                    Console.WriteLine($"Radius: {kreis.Radius}");
                    Console.WriteLine();
                    break;
                case Quadrat quadrat:
                Console.WriteLine("Quadrat:");
                    Console.WriteLine($"Fläche: {quadrat.Fläche}");
                    Console.WriteLine($"Umfang: {quadrat.Umfang}");
                    Console.WriteLine($"Seitenlänge: {quadrat.Seite}");
                    Console.WriteLine();
                    break;
                case Rechteck rechteck:
                Console.WriteLine("Rechteck:");
                    Console.WriteLine($"Fläche: {rechteck.Fläche}");
                    Console.WriteLine($"Umfang: {rechteck.Umfang}");
                    Console.WriteLine($"Breite: {rechteck.Breite}, Höhe: {rechteck.Länge}");
                    Console.WriteLine();
                    break;
                case Dreieck dreieck:
                Console.WriteLine("Dreieck:");
                    Console.WriteLine($"Fläche: {dreieck.Fläche}");
                    Console.WriteLine($"Umfang: {dreieck.Umfang}");
                    Console.WriteLine($"SeiteA: {dreieck.SeiteA}, SeiteB: {dreieck.SeiteB}, SeiteC: {dreieck.SeiteC}");
                    Console.WriteLine();
                    break;             
                case Trapez trapez:
                Console.WriteLine("Trapez:");
                    Console.WriteLine($"Fläche: {trapez.Fläche}");
                    Console.WriteLine($"Umfang: {trapez.Umfang}");
                    Console.WriteLine($"ObereBasis: {trapez.ObereBasis}, UntereBasis: {trapez.UntereBasis}, Höhe: {trapez.Höhe}");
                    Console.WriteLine();
                    break;
                case Ellipse ellipse:
                Console.WriteLine("Ellipse:");
                    Console.WriteLine($"Fläche: {ellipse.Fläche}");
                    Console.WriteLine($"Umfang: {ellipse.Umfang}");
                    Console.WriteLine($"HalbachseA: {ellipse.HalbachseA}, HalbachseB: {ellipse.HalbachseB}");
                    Console.WriteLine();
                    break;
            }
        }

        

    }
}
