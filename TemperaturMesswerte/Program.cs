namespace TemperaturMesswerte;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Temperatur Messwerte Programm");
        Console.WriteLine("Wie viele Messwerte möchten Sie eingeben?");
        int anzahl = int.Parse(Console.ReadLine());
        double[] messwerte = new double[anzahl];
        for (int i = 0; i < anzahl; i++)
        {
            Console.WriteLine($"Geben Sie den {i + 1}. Messwert ein:");
            messwerte[i] = double.Parse(Console.ReadLine());
        }
        Console.WriteLine("Eingegebene Messwerte:");
        for (int i = 0; i < anzahl; i++)
        {
            Console.WriteLine($"Messwert {i + 1}: {messwerte[i]}");
        }
        
        //Durschnitt berechnen
        double summe = 0;
        for (int i = 0; i < anzahl; i++)
        {
            summe += messwerte[i];
        }
        double durchschnitt = summe / anzahl;
        Console.WriteLine($"Durchschnittlicher Messwert: {durchschnitt}");

        // Minimum und Maximum berechnen
        double min = messwerte[0];
        double max = messwerte[0];
        for (int i = 1; i < anzahl; i++)
        {
            if (messwerte[i] < min) min = messwerte[i];
            if (messwerte[i] > max) max = messwerte[i];
        }
        Console.WriteLine($"Minimaler Messwert: {min}");
        Console.WriteLine($"Maximaler Messwert: {max}");

        // Kategorisieren der Messwerte
        int unterNull = 0;
        int zwischenNullUndZehn = 0;
        int ueberZehn = 0;
        for (int i = 0; i < anzahl; i++)
        {
            if (messwerte[i] < 0) unterNull++;
            else if (messwerte[i] <= 10) zwischenNullUndZehn++;
            else ueberZehn++;
        }
        Console.WriteLine($"Messwerte unter 0: {unterNull}");
        Console.WriteLine($"Messwerte zwischen 0 und 10: {zwischenNullUndZehn}");
        Console.WriteLine($"Messwerte über 10: {ueberZehn}");

        // Neue Messreihe eingeben
        Console.WriteLine("Möchten Sie eine neue Messreihe eingeben? (ja/nein)");
        string antwort = Console.ReadLine().ToLower();
        if (antwort == "ja")
        {
            Main(args);
        }
    }
}
