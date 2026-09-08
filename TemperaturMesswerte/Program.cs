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
        PrintSection("Kategorisierte Messwerte", new[] { $"Messwerte unter 0: {unterNull}", $"Messwerte zwischen 0 und 10: {zwischenNullUndZehn}", $"Messwerte über 10: {ueberZehn}" });
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
        PrintSection("Neue Messreihe", new[] { "Möchten Sie eine neue Messreihe eingeben? (ja/nein)" });
        string antwort = ReadStyledInput("(ja/nein)").ToLower();
        if (antwort == "ja")
        {
            Main();
        }
    }

    private static void Hauptmenü()
    {
        // Ordentliche, hübsche Formatierung der Konsolenausgabe
        Console.Clear(); // Bildschirm vor der Anzeige des Hauptmenüs löschen

        string title = "Temperatur Messwerte Programm";
        string[] lines = new[]
        {
            "Die folgenden Schritte helfen Ihnen, Temperaturmesswerte einzugeben und auszuwerten.",
            "Bitte folgen Sie den Anweisungen.",
            "Es wird der Durchschnitt, das Minimum und Maximum der Messwerte berechnet.",
            "Anschließend können Sie entscheiden, ob Sie eine neue Messreihe eingeben möchten."
        };

        // Bestimme Breite der Box
        int maxLine = title.Length;
        foreach (var l in lines) if (l.Length > maxLine) maxLine = l.Length;
        int innerWidth = maxLine + 4; // Ränder

        // Zeichne Box mit Rahmen und zentriertem Titel
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔" + new string('═', innerWidth) + "╗");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("║" + CenterText(title, innerWidth) + "║");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╠" + new string('═', innerWidth) + "╣");

        Console.ForegroundColor = ConsoleColor.White;
        foreach (var l in lines)
        {
            Console.WriteLine("║" + PadText(l, innerWidth) + "║");
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╚" + new string('═', innerWidth) + "╝");
        Console.ResetColor();
        Console.WriteLine();

        // Lokale Hilfsfunktionen
        string CenterText(string s, int width)
        {
            int left = (width - s.Length) / 2;
            if (left < 0) left = 0;
            return new string(' ', left) + s + new string(' ', width - left - s.Length);
        }

        string PadText(string s, int width)
        {
            // kleines Padding links
            string padded = "  " + s;
            if (padded.Length > width) padded = padded.Substring(0, width);
            return padded + new string(' ', width - padded.Length);
        }
    }

    // Kleine Hilfsfunktion: Zeichnet eine hübsche Box wie im Hauptmenü
    private static void PrintBox(string title, string[] lines)
    {
        // Bestimme Breite der Box
        int maxLine = title.Length;
        foreach (var l in lines) if (l.Length > maxLine) maxLine = l.Length;
        int innerWidth = maxLine + 4; // Ränder

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔" + new string('═', innerWidth) + "╗");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("║" + CenterText(title, innerWidth) + "║");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╠" + new string('═', innerWidth) + "╣");

        Console.ForegroundColor = ConsoleColor.White;
        foreach (var l in lines)
        {
            Console.WriteLine("║" + PadText(l, innerWidth) + "║");
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╚" + new string('═', innerWidth) + "╝");
        Console.ResetColor();
        Console.WriteLine();

        // Lokale Hilfsfunktionen
        string CenterText(string s, int width)
        {
            int left = (width - s.Length) / 2;
            if (left < 0) left = 0;
            return new string(' ', left) + s + new string(' ', width - left - s.Length);
        }

        string PadText(string s, int width)
        {
            string padded = "  " + s;
            if (padded.Length > width) padded = padded.Substring(0, width);
            return padded + new string(' ', width - padded.Length);
        }
    }

    // Einheitliche einfache Abschnitts-Formatierung (kein kompletter Kasten)
    private static void PrintSection(string title, string[] lines)
    {
        // Header
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(new string('─', 60));
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("  " + title);
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(new string('─', 60));

        // Inhalt
        Console.ForegroundColor = ConsoleColor.White;
        foreach (var l in lines)
        {
            Console.WriteLine("  " + l);
        }
        Console.ResetColor();
        Console.WriteLine();
    }

    // Konsistente Prompt-Ausgabe (gleiche Farbe/Prefix für Eingaben)
    private static string ReadStyledInput(string prompt)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write("  → " + prompt + " ");
        Console.ResetColor();
        var input = Console.ReadLine();
        return input ?? string.Empty;
    }

     private static int AbfrageAnzahlMesswerte()
    {
        // Hübsche Ausgabe wie im Hauptmenü
        PrintBox("Anzahl Messwerte", new[] { "Wie viele Messwerte möchten Sie eingeben?", "Bitte eine ganze Zahl eingeben." });
        bool success = int.TryParse(ReadStyledInput(""), out int anzahl);
        if (!success)
        {
            PrintSection("Fehler", new[] { "Ungültige Eingabe. Bitte geben Sie eine Zahl ein." });
            return AbfrageAnzahlMesswerte();
        }
        return anzahl;
    }
    
    private static double[] EingabeMesswerte(int anzahl)
    {
        double[] messwerte = new double[anzahl];
        for (int i = 0; i < anzahl; i++)
        {
            string input = ReadStyledInput($"Geben Sie den {i + 1}. Messwert ein:");
            bool success = double.TryParse(input, out double wert);
            if (!success)
            {
                PrintSection("Fehler", new[] { "Ungültige Eingabe. Bitte geben Sie eine Zahl ein." });
                i--; // Wiederhole die Eingabe für diesen Index
                continue;
            }
            messwerte[i] = wert;
        }
        return messwerte;
    }

    private static void AnzeigeMesswerte(double[] messwerte)
    {
        var lines = new string[messwerte.Length];
        for (int i = 0; i < messwerte.Length; i++)
        {
            lines[i] = $"Messwert {i + 1}: {messwerte[i]}";
        }
        PrintSection("Eingegebene Messwerte", lines);
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
        PrintSection("Durchschnitt", new[] { $"Durchschnittlicher Messwert: {durchschnitt}" });
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
        PrintSection("Minimum und Maximum", new[] { $"Minimaler Messwert: {min}", $"Maximaler Messwert: {max}" });
    }
}
