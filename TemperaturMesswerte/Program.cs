namespace TemperaturMesswerte;

class Program
{
    static void Main()
    {
        Hauptmenü();
        
        int anzahl = AbfrageAnzahlMesswerte();
        
        double[] messwerte = EingabeMesswerte(anzahl);
        AnzeigeMesswerte(messwerte);

        double summe = BerechneSumme(messwerte);
        double durchschnitt = BerechneDurchschnitt(summe, anzahl);
        AnzeigeDurchschnitt(durchschnitt);

        (double min, double max) = BerechneMinimumUndMaximum(messwerte);
        AnzeigeMinimumUndMaximum(min, max);

        (int unterNull, int zwischenNullUndZehn, int ueberZehn) = KategorisiereMesswerte(messwerte);
        AnzeigeKategorisierteMesswerte(unterNull, zwischenNullUndZehn, ueberZehn);

        NeueMessreiheEingeben();
    }

    private static void AnzeigeKategorisierteMesswerte(int unterNull, int zwischenNullUndZehn, int ueberZehn)
    {
        Console.WriteLine($"Messwerte unter 0: {unterNull}");
        Console.WriteLine($"Messwerte zwischen 0 und 10: {zwischenNullUndZehn}");
        Console.WriteLine($"Messwerte über 10: {ueberZehn}");
    }
    
    private static (int, int, int) KategorisiereMesswerte(double[] messwerte)
    {
        int unterNull = 0;
        int zwischenNullUndZehn = 0;
        int ueberZehn = 0;
        for (int i = 0; i < messwerte.Length; i++)
        {
            if (messwerte[i] < 0) unterNull++;
            else if (messwerte[i] <= 10) zwischenNullUndZehn++;
            else ueberZehn++;
        }
        return (unterNull, zwischenNullUndZehn, ueberZehn);
    }

    
    private static void NeueMessreiheEingeben()
    {
        Console.WriteLine("Möchten Sie eine neue Messreihe eingeben? (ja/nein)");
        string antwort = Console.ReadLine().ToLower();
        if (antwort == "ja")
        {
            Main();
        }
    }

    private static void Hauptmenü()
    {
        Console.WriteLine("Temperatur Messwerte Programm");
        Console.WriteLine("Die folgenden Schritte helfen Ihnen, Temperaturmesswerte einzugeben und auszuwerten.");
        Console.WriteLine("Bitte folgen Sie den Anweisungen.");
        Console.WriteLine("Es wird der Durchschnitt, das Minimum und Maximum der Messwerte berechnet.");
        Console.WriteLine("Anschließend können Sie entscheiden, ob Sie eine neue Messreihe eingeben möchten.");
    }

     private static int AbfrageAnzahlMesswerte()
    {
        Console.WriteLine("Wie viele Messwerte möchten Sie eingeben?");
        bool success = int.TryParse(Console.ReadLine(), out int anzahl);
        if (!success)
        {
            Console.WriteLine("Ungültige Eingabe. Bitte geben Sie eine Zahl ein.");
            return AbfrageAnzahlMesswerte();
        }
        return anzahl;
    }
    
    private static double[] EingabeMesswerte(int anzahl)
    {
        double[] messwerte = new double[anzahl];
        for (int i = 0; i < anzahl; i++)
        {
            Console.WriteLine($"Geben Sie den {i + 1}. Messwert ein:");
            bool success = double.TryParse(Console.ReadLine(), out double wert);
            if (!success)
            {
                Console.WriteLine("Ungültige Eingabe. Bitte geben Sie eine Zahl ein.");
                i--; // Wiederhole die Eingabe für diesen Index
                continue;
            }
            messwerte[i] = wert;
        }
        return messwerte;
    }

    private static void AnzeigeMesswerte(double[] messwerte)
    {
        Console.WriteLine("Eingegebene Messwerte:");
        for (int i = 0; i < messwerte.Length; i++)
        {
            Console.WriteLine($"Messwert {i + 1}: {messwerte[i]}");
        }
    }
    
    private static double BerechneSumme(double[] messwerte)
    {
        double summe = 0;
        for (int i = 0; i < messwerte.Length; i++)
        {
            summe += messwerte[i];
        }
        return summe;
    }

    private static double BerechneDurchschnitt(double summe, int anzahl)
    {
        return summe / anzahl;
    }

    private static void AnzeigeDurchschnitt(double durchschnitt)
    {
        Console.WriteLine($"Durchschnittlicher Messwert: {durchschnitt}");
    }
    
    private static (double, double) BerechneMinimumUndMaximum(double[] messwerte)
    {
        double min = messwerte[0];
        double max = messwerte[0];
        for (int i = 1; i < messwerte.Length; i++)
        {
            if (messwerte[i] < min) min = messwerte[i];
            if (messwerte[i] > max) max = messwerte[i];
        }
        return (min, max);
    }
    
    private static void AnzeigeMinimumUndMaximum(double min, double max)
    {
        Console.WriteLine($"Minimaler Messwert: {min}");
        Console.WriteLine($"Maximaler Messwert: {max}");
    }
}
