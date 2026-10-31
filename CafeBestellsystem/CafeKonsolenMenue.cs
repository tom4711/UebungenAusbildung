using System.Globalization;
using CafeBestellsystem.Core.Bestellungen;
using CafeBestellsystem.Core.Produkte;

namespace CafeBestellsystem.Konsole;

public class CafeKonsolenMenue
{
    private static readonly CultureInfo DeutscheKultur = CultureInfo.GetCultureInfo("de-DE");
    private readonly TextReader _eingabe;
    private readonly TextWriter _ausgabe;
    private readonly Bestellverwaltung _verwaltung;
    private readonly IReadOnlyList<Produkt> _produkte;

    public CafeKonsolenMenue(
        TextReader input,
        TextWriter output,
        Bestellverwaltung verwaltung,
        IReadOnlyList<Produkt> produkte)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(output);
        ArgumentNullException.ThrowIfNull(verwaltung);
        ArgumentNullException.ThrowIfNull(produkte);

        _eingabe = input;
        _ausgabe = output;
        _verwaltung = verwaltung;
        _produkte = produkte;
    }

    public void Starten()
    {
        var beenden = false;
        while (!beenden)
        {
            MenueAnzeigen();
            var eingabe = _eingabe.ReadLine();
            if (eingabe is null)
            {
                beenden = true;
                continue;
            }

            if (!int.TryParse(eingabe, out var auswahl) || auswahl < 0 || auswahl > 6)
            {
                _ausgabe.WriteLine("Ungültige Auswahl. Bitte wählen Sie eine Zahl von 0 bis 6.");
                continue;
            }

            switch (auswahl)
            {
                case 1:
                    ProdukteAnzeigen();
                    break;
                case 2:
                    BestellungStarten();
                    break;
                case 3:
                    ProduktHinzufuegen();
                    break;
                case 4:
                    GesamtpreisAnzeigen();
                    break;
                case 5:
                    BestellungAbschliessen();
                    break;
                case 6:
                    TagesumsatzAnzeigen();
                    break;
                case 0:
                    beenden = true;
                    break;
            }
        }

        _ausgabe.WriteLine("Auf Wiedersehen!");
    }

    private void MenueAnzeigen()
    {
        _ausgabe.WriteLine();
        _ausgabe.WriteLine("Café-Bestellsystem");
        _ausgabe.WriteLine("1. Produkte anzeigen");
        _ausgabe.WriteLine("2. Bestellung starten");
        _ausgabe.WriteLine("3. Produkt zu Bestellung hinzufügen");
        _ausgabe.WriteLine("4. Gesamtpreis anzeigen");
        _ausgabe.WriteLine("5. Bestellung abschließen");
        _ausgabe.WriteLine("6. Tagesumsatz anzeigen");
        _ausgabe.WriteLine("0. Beenden");
        _ausgabe.Write("Auswahl: ");
    }

    private void ProdukteAnzeigen()
    {
        _ausgabe.WriteLine("Produkte:");
        for (var index = 0; index < _produkte.Count; index++)
        {
            var produkt = _produkte[index];
            _ausgabe.WriteLine($"{index + 1}. {produkt.Beschreibung()} - {PreisFormatieren(produkt.Preis)}");
        }
    }

    private void BestellungStarten()
    {
        var bestellung = _verwaltung.BestellungErstellen();
        _ausgabe.WriteLine($"Bestellung {bestellung.Nummer} wurde gestartet.");
    }

    private void ProduktHinzufuegen()
    {
        var bestellung = BestellungAbfragen();
        if (bestellung is null)
        {
            return;
        }

        ProdukteAnzeigen();
        var produktNummer = ZahlAbfragen("Produktnummer: ");
        if (produktNummer is null || produktNummer < 1 || produktNummer > _produkte.Count)
        {
            _ausgabe.WriteLine("Ungültige Produktnummer.");
            return;
        }

        var menge = ZahlAbfragen("Menge: ");
        if (menge is null || menge <= 0)
        {
            _ausgabe.WriteLine("Die Menge muss eine positive Zahl sein.");
            return;
        }

        if (!bestellung.PositionHinzufuegen(_produkte[produktNummer.Value - 1], menge.Value))
        {
            _ausgabe.WriteLine("Die Bestellung ist nicht mehr offen.");
            return;
        }

        _ausgabe.WriteLine("Produkt wurde hinzugefügt.");
    }

    private void GesamtpreisAnzeigen()
    {
        var bestellung = BestellungAbfragen();
        if (bestellung is null)
        {
            return;
        }

        _ausgabe.WriteLine($"Gesamtpreis: {PreisFormatieren(bestellung.GesamtpreisBerechnen())}");
    }

    private void BestellungAbschliessen()
    {
        var bestellung = BestellungAbfragen();
        if (bestellung is null)
        {
            return;
        }

        var heute = DateOnly.FromDateTime(DateTime.Today);
        if (bestellung.Abschliessen(heute))
        {
            _ausgabe.WriteLine($"Bestellung {bestellung.Nummer} wurde abgeschlossen.");
        }
        else
        {
            _ausgabe.WriteLine("Die Bestellung ist nicht mehr offen.");
        }
    }

    private void TagesumsatzAnzeigen()
    {
        var heute = DateOnly.FromDateTime(DateTime.Today);
        _ausgabe.WriteLine($"Tagesumsatz: {PreisFormatieren(_verwaltung.TagesumsatzBerechnen(heute))}");
    }

    private Bestellung? BestellungAbfragen()
    {
        var nummer = ZahlAbfragen("Bestellnummer: ");
        if (nummer is null)
        {
            _ausgabe.WriteLine("Ungültige Bestellnummer.");
            return null;
        }

        var bestellung = _verwaltung.BestellungSuchen(nummer.Value);
        if (bestellung is null)
        {
            _ausgabe.WriteLine("Bestellung nicht gefunden.");
        }

        return bestellung;
    }

    private int? ZahlAbfragen(string aufforderung)
    {
        _ausgabe.Write(aufforderung);
        var eingabe = _eingabe.ReadLine();
        return int.TryParse(eingabe, out var zahl) ? zahl : null;
    }

    private static string PreisFormatieren(decimal preis)
    {
        return $"{preis.ToString("F2", DeutscheKultur)} €";
    }
}
