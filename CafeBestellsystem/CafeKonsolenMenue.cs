using System.Globalization;
using CafeBestellsystem.Core.Bestellungen;
using CafeBestellsystem.Core.Produkte;
using Spectre.Console;

namespace CafeBestellsystem.Konsole;

public class CafeKonsolenMenue
{
    private static readonly CultureInfo DeutscheKultur = CultureInfo.GetCultureInfo("de-DE");
    private readonly TextReader _eingabe;
    private readonly Bestellverwaltung _verwaltung;
    private readonly IReadOnlyList<Produkt> _produkte;
    private readonly IAnsiConsole _konsole;
    private readonly bool _interaktiveKonsole;

    public CafeKonsolenMenue(
        TextReader input,
        TextWriter output,
        Bestellverwaltung verwaltung,
        IReadOnlyList<Produkt> produkte,
        bool? interaktiv = null)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(output);
        ArgumentNullException.ThrowIfNull(verwaltung);
        ArgumentNullException.ThrowIfNull(produkte);

        _eingabe = input;
        _verwaltung = verwaltung;
        _produkte = produkte;
        _interaktiveKonsole = interaktiv ?? (ReferenceEquals(output, Console.Out) && !Console.IsOutputRedirected);
        _konsole = AnsiConsole.Create(new AnsiConsoleSettings
        {
            Out = new AnsiConsoleOutput(output),
            Ansi = _interaktiveKonsole ? AnsiSupport.Yes : AnsiSupport.No,
            ColorSystem = _interaktiveKonsole ? ColorSystemSupport.Standard : ColorSystemSupport.NoColors
        });
    }

    public void Starten()
    {
        var beenden = false;
        while (!beenden)
        {
            if (_interaktiveKonsole)
            {
                _konsole.Clear();
            }

            MenueAnzeigen();
            var eingabe = _eingabe.ReadLine();
            if (eingabe is null)
            {
                beenden = true;
                continue;
            }

            if (!int.TryParse(eingabe, out var auswahl) || auswahl < 0 || auswahl > 7)
            {
                FehlerAnzeigen("Ungültige Auswahl. Bitte wählen Sie eine Zahl von 0 bis 7.");
                AufraeumenAbwarten();
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
                case 7:
                    OffeneBestellungenAnzeigen();
                    break;
                case 0:
                    beenden = true;
                    break;
            }

            if (!beenden)
            {
                AufraeumenAbwarten();
            }
        }

        _konsole.MarkupLine("[green]Auf Wiedersehen![/]");
    }

    private void MenueAnzeigen()
    {
        _konsole.MarkupLine("[bold cyan]Café-Bestellsystem[/]");
        var tabelle = new Table().Border(TableBorder.Rounded).BorderColor(Color.Grey)
            .AddColumn(new TableColumn("[bold]Nr.[/]"))
            .AddColumn(new TableColumn("[bold]Aktion[/]"));
        tabelle.AddRow("[yellow]1[/]", "Produkte anzeigen");
        tabelle.AddRow("[yellow]2[/]", "Bestellung starten");
        tabelle.AddRow("[yellow]3[/]", "Produkt hinzufügen");
        tabelle.AddRow("[yellow]4[/]", "Gesamtpreis anzeigen");
        tabelle.AddRow("[yellow]5[/]", "Bestellung abschließen");
        tabelle.AddRow("[yellow]6[/]", "Tagesumsatz anzeigen");
        tabelle.AddRow("[yellow]7[/]", "Offene Bestellungen anzeigen");
        tabelle.AddRow("[yellow]0[/]", "Beenden");
        _konsole.Write(tabelle);
        _konsole.Markup("[bold blue]Auswahl: [/]");
    }

    private void ProdukteAnzeigen()
    {
        var tabelle = new Table().Border(TableBorder.Rounded).BorderColor(Color.Grey)
            .AddColumn(new TableColumn("[bold]Nr.[/]"))
            .AddColumn(new TableColumn("[bold]Produkt[/]"))
            .AddColumn(new TableColumn("[bold]Preis[/]"));
        for (var index = 0; index < _produkte.Count; index++)
        {
            var produkt = _produkte[index];
            tabelle.AddRow((index + 1).ToString(), Markup.Escape(produkt.Beschreibung()), PreisFormatieren(produkt.Preis));
        }

        _konsole.Write(tabelle);
    }

    private void BestellungStarten()
    {
        var bestellung = _verwaltung.BestellungErstellen();
        ErfolgAnzeigen($"Bestellung {bestellung.Nummer} wurde gestartet.");
    }

    private void ProduktHinzufuegen()
    {
        var offeneBestellungen = _verwaltung.OffeneBestellungenAnzeigen();
        if (offeneBestellungen.Count == 0)
        {
            FehlerAnzeigen("Keine offenen Bestellungen vorhanden.");
            return;
        }

        _konsole.MarkupLine("[bold cyan]Offene Bestellungen zur Auswahl[/]");
        OffeneBestellungenTabelleAnzeigen(offeneBestellungen);
        var nummer = ZahlAbfragen("Bestellnummer aus der Übersicht: ");
        if (nummer is null)
        {
            FehlerAnzeigen("Ungültige Bestellnummer.");
            return;
        }

        var bestellung = offeneBestellungen.FirstOrDefault(offene => offene.Nummer == nummer.Value);
        if (bestellung is null)
        {
            FehlerAnzeigen("Diese Nummer gehört zu keiner offenen Bestellung.");
            return;
        }

        BestellungsinhaltAnzeigen(bestellung);
        ProdukteAnzeigen();
        var produktNummer = ZahlAbfragen("Produktnummer: ");
        if (produktNummer is null || produktNummer < 1 || produktNummer > _produkte.Count)
        {
            FehlerAnzeigen("Ungültige Produktnummer.");
            return;
        }

        var menge = ZahlAbfragen("Menge: ");
        if (menge is null || menge <= 0)
        {
            FehlerAnzeigen("Die Menge muss eine positive Zahl sein.");
            return;
        }

        if (!bestellung.PositionHinzufuegen(_produkte[produktNummer.Value - 1], menge.Value))
        {
            FehlerAnzeigen("Die Bestellung ist nicht mehr offen.");
            return;
        }

        ErfolgAnzeigen("Produkt wurde hinzugefügt.");
    }

    private void GesamtpreisAnzeigen()
    {
        var bestellung = BestellungAbfragen();
        if (bestellung is null)
        {
            return;
        }

        _konsole.MarkupLine($"[bold]Gesamtpreis:[/] [green]{PreisFormatieren(bestellung.GesamtpreisBerechnen())}[/]");
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
            ErfolgAnzeigen($"Bestellung {bestellung.Nummer} wurde abgeschlossen.");
        }
        else
        {
            FehlerAnzeigen("Die Bestellung ist nicht mehr offen.");
        }
    }

    private void TagesumsatzAnzeigen()
    {
        var heute = DateOnly.FromDateTime(DateTime.Today);
        var tagesumsatz = _verwaltung.TagesumsatzBerechnen(heute);
        var produktumsatz = _verwaltung.ProduktumsatzBerechnen(heute);

        _konsole.MarkupLine($"[bold]Tagesumsatz:[/] [green]{PreisFormatieren(tagesumsatz)}[/]");
        if (tagesumsatz == 0m || produktumsatz.Count == 0)
        {
            _konsole.MarkupLine("[grey]Keine Produktumsätze für heute vorhanden.[/]");
            return;
        }

        _konsole.MarkupLine("[bold cyan]Umsatz nach Produkt[/]");
        var sortierteProduktumsaetze = produktumsatz
            .OrderByDescending(produkt => produkt.Value)
            .ThenBy(produkt => produkt.Key, StringComparer.Ordinal)
            .ToList();
        var tabelle = new Table().Border(TableBorder.Rounded).BorderColor(Color.Grey)
            .AddColumn(new TableColumn("[bold]Produkt[/]"))
            .AddColumn(new TableColumn("[bold]Umsatz[/]"))
            .AddColumn(new TableColumn("[bold]Anteil[/]"));
        var farben = new[]
        {
            Color.Blue,
            Color.Green,
            Color.Yellow,
            Color.Purple,
            Color.Orange1,
            Color.Cyan1,
            Color.Red,
            Color.Grey
        };
        var diagramm = new BreakdownChart().ShowPercentage();

        for (var index = 0; index < sortierteProduktumsaetze.Count; index++)
        {
            var produkt = sortierteProduktumsaetze[index];
            var anteil = produkt.Value / tagesumsatz;
            tabelle.AddRow(
                Markup.Escape(produkt.Key),
                PreisFormatieren(produkt.Value),
                anteil.ToString("P1", DeutscheKultur));
            diagramm.AddItem(
                Markup.Escape(produkt.Key),
                (double)(anteil * 100m),
                farben[index % farben.Length]);
        }

        _konsole.Write(tabelle);
        _konsole.Write(diagramm);
    }

    private void OffeneBestellungenAnzeigen()
    {
        var offeneBestellungen = _verwaltung.OffeneBestellungenAnzeigen();
        if (offeneBestellungen.Count == 0)
        {
            _konsole.MarkupLine("[grey]Keine offenen Bestellungen vorhanden.[/]");
            return;
        }

        _konsole.MarkupLine("[bold cyan]Offene Bestellungen[/]");
        OffeneBestellungenTabelleAnzeigen(offeneBestellungen);
    }

    private void OffeneBestellungenTabelleAnzeigen(IReadOnlyList<Bestellung> bestellungen)
    {
        var tabelle = new Table().Border(TableBorder.Rounded).BorderColor(Color.Grey)
            .AddColumn(new TableColumn("[bold]Bestellnr.[/]"))
            .AddColumn(new TableColumn("[bold]Enthaltene Produkte[/]"))
            .AddColumn(new TableColumn("[bold]Gesamtpreis[/]"));

        foreach (var bestellung in bestellungen)
        {
            var inhalt = bestellung.Positionen.Count == 0
                ? "Noch keine Produkte"
                : string.Join(", ", bestellung.Positionen.Select(position =>
                    $"{position.Menge} × {position.Produkt.Beschreibung()}"));
            tabelle.AddRow(
                bestellung.Nummer.ToString(),
                Markup.Escape(inhalt),
                PreisFormatieren(bestellung.GesamtpreisBerechnen()));
        }

        _konsole.Write(tabelle);
    }

    private void BestellungsinhaltAnzeigen(Bestellung bestellung)
    {
        _konsole.MarkupLine($"[bold cyan]Aktueller Inhalt von Bestellung {bestellung.Nummer}[/]");
        if (bestellung.Positionen.Count == 0)
        {
            _konsole.MarkupLine("[grey]Noch keine Produkte in dieser Bestellung.[/]");
        }
        else
        {
            var tabelle = new Table().Border(TableBorder.Rounded).BorderColor(Color.Grey)
                .AddColumn(new TableColumn("[bold]Menge[/]"))
                .AddColumn(new TableColumn("[bold]Produkt[/]"))
                .AddColumn(new TableColumn("[bold]Positionspreis[/]"));
            foreach (var position in bestellung.Positionen)
            {
                tabelle.AddRow(
                    position.Menge.ToString(),
                    Markup.Escape(position.Produkt.Beschreibung()),
                    PreisFormatieren(position.Positionspreis));
            }

            _konsole.Write(tabelle);
        }

        _konsole.MarkupLine($"[bold]Aktueller Gesamtpreis:[/] [green]{PreisFormatieren(bestellung.GesamtpreisBerechnen())}[/]");
    }

    private Bestellung? BestellungAbfragen()
    {
        var nummer = ZahlAbfragen("Bestellnummer: ");
        if (nummer is null)
        {
            FehlerAnzeigen("Ungültige Bestellnummer.");
            return null;
        }

        var bestellung = _verwaltung.BestellungSuchen(nummer.Value);
        if (bestellung is null)
        {
            FehlerAnzeigen("Bestellung nicht gefunden.");
        }

        return bestellung;
    }

    private int? ZahlAbfragen(string aufforderung)
    {
        _konsole.Markup($"[blue]{Markup.Escape(aufforderung)}[/]");
        var eingabe = _eingabe.ReadLine();
        return int.TryParse(eingabe, out var zahl) ? zahl : null;
    }

    private static string PreisFormatieren(decimal preis)
    {
        return $"{preis.ToString("F2", DeutscheKultur)} €";
    }

    private void ErfolgAnzeigen(string text)
    {
        _konsole.MarkupLine($"[green]{Markup.Escape(text)}[/]");
    }

    private void FehlerAnzeigen(string text)
    {
        _konsole.MarkupLine($"[red]{Markup.Escape(text)}[/]");
    }

    private void AufraeumenAbwarten()
    {
        if (!_interaktiveKonsole)
        {
            return;
        }

        _konsole.MarkupLine("[grey]Weiter mit Enter ...[/]");
        _eingabe.ReadLine();
    }
}
