using System.Globalization;
using CafeBestellsystem.Core.Bestellungen;
using CafeBestellsystem.Core.Produkte;
using Spectre.Console;
using Spectre.Console.Rendering;

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
        bool? interaktiv = null,
        IAnsiConsole? ansiConsole = null)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(output);
        ArgumentNullException.ThrowIfNull(verwaltung);
        ArgumentNullException.ThrowIfNull(produkte);

        _eingabe = input;
        _verwaltung = verwaltung;
        _produkte = produkte;
        _interaktiveKonsole = interaktiv ?? ansiConsole?.Profile.Capabilities.Interactive
            ?? (ReferenceEquals(output, Console.Out) && !Console.IsOutputRedirected);
        _konsole = ansiConsole ?? AnsiConsole.Create(new AnsiConsoleSettings
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
            var auswahl = MenueAuswahlAbfragen();
            if (auswahl is null)
            {
                beenden = true;
                continue;
            }

            switch (auswahl.Value)
            {
                case MenueAktion.ProdukteAnzeigen:
                    ProdukteAnzeigen();
                    break;
                case MenueAktion.BestellungStarten:
                    BestellungStarten();
                    break;
                case MenueAktion.ProduktHinzufuegen:
                    ProduktHinzufuegen();
                    break;
                case MenueAktion.GesamtpreisAnzeigen:
                    GesamtpreisAnzeigen();
                    break;
                case MenueAktion.BestellungAbschliessen:
                    BestellungAbschliessen();
                    break;
                case MenueAktion.TagesumsatzAnzeigen:
                    TagesumsatzAnzeigen();
                    break;
                case MenueAktion.OffeneBestellungenAnzeigen:
                    OffeneBestellungenAnzeigen();
                    break;
                case MenueAktion.Beenden:
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
        PanelAnzeigen(
            "Café-Bestellsystem",
            new Markup("[grey]Bestellungen übersichtlich verwalten[/]"));
        if (_interaktiveKonsole)
        {
            return;
        }

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
        PanelAnzeigen("Aktionen", tabelle);
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

        PanelAnzeigen("Produktkatalog", tabelle);
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
            PanelAnzeigen("Keine offenen Bestellungen", new Markup("[grey]Es gibt aktuell keine offenen Bestellungen.[/]"));
            return;
        }

        OffeneBestellungenTabelleAnzeigen(offeneBestellungen, "Offene Bestellungen zur Auswahl");
        var bestellung = OffeneBestellungAuswaehlen(offeneBestellungen, "Welche Bestellung soll ergänzt werden?");
        if (bestellung is null) return;

        BestellungsinhaltAnzeigen(bestellung);
        ProdukteAnzeigen();
        var produkte = ProdukteAuswaehlen();
        if (produkte is null || produkte.Count == 0)
        {
            _konsole.MarkupLine("[grey]Keine Produkte ausgewählt. Die Bestellung bleibt unverändert.[/]");
            return;
        }

        var warenkorb = new List<(Produkt Produkt, int Menge)>();
        foreach (var produkt in produkte)
        {
            _konsole.MarkupLine($"[bold cyan]Menge für {Markup.Escape(produkt.Beschreibung())}[/]");
            var menge = ZahlAbfragen(
                "Menge (0 = Abbrechen): ",
                zahl => zahl > 0,
                "Die Menge muss eine positive Zahl sein.");
            if (menge is null)
            {
                _konsole.MarkupLine("[yellow]Vorgang abgebrochen. Die Bestellung bleibt unverändert.[/]");
                return;
            }

            warenkorb.Add((produkt, menge.Value));
        }

        WarenkorbVorschauAnzeigen(bestellung, warenkorb);
        if (!BestellungBestaetigen())
        {
            _konsole.MarkupLine("[yellow]Warenkorb verworfen. Die Bestellung bleibt unverändert.[/]");
            return;
        }

        foreach (var position in warenkorb)
        {
            if (!bestellung.PositionHinzufuegen(position.Produkt, position.Menge))
            {
                FehlerAnzeigen("Die Bestellung ist nicht mehr offen.");
                return;
            }
        }

        ErfolgAnzeigen($"{warenkorb.Count} Produkt{(warenkorb.Count == 1 ? "" : "e")} wurde{(warenkorb.Count == 1 ? "" : "n")} zur Bestellung hinzugefügt.");
    }

    private IReadOnlyList<Produkt>? ProdukteAuswaehlen()
    {
        if (_interaktiveKonsole)
        {
            var auswahl = _konsole.Prompt(
                new MultiSelectionPrompt<ProduktPromptAuswahl>()
                    .Title("[bold]Welche Produkte möchten Sie hinzufügen?[/]")
                    .HighlightStyle(new Style(Color.Cyan1, decoration: Decoration.Bold))
                    .NotRequired()
                    .InstructionsText("[grey](Mit [blue]<Leertaste>[/] auswählen, mit [green]<Enter>[/] bestätigen)[/]")
                    .UseConverter(auswahl => auswahl.Produkt is null
                        ? "[grey]Zurück zum Hauptmenü[/]"
                        : $"{Markup.Escape(auswahl.Produkt.Beschreibung())} · [green]{PreisFormatieren(auswahl.Produkt.Preis)}[/]")
                    .AddChoices(_produkte.Select(produkt => new ProduktPromptAuswahl(produkt))
                        .Append(new ProduktPromptAuswahl(null))));

            return auswahl.Any(auswahl => auswahl.Produkt is null)
                ? null
                : auswahl.Select(auswahl => auswahl.Produkt!).ToList();
        }

        while (true)
        {
            _konsole.Markup("[blue]Produktnummern (z. B. 1, 3; 0 = Abbrechen): [/]");
            var eingabe = _eingabe.ReadLine();
            if (eingabe is null)
            {
                return null;
            }

            if (eingabe.Trim() == "0")
            {
                return null;
            }

            var nummern = eingabe.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (nummern.Length > 0
                && nummern.All(nummer => int.TryParse(nummer, out var index) && index >= 1 && index <= _produkte.Count))
            {
                return nummern.Select(int.Parse).Distinct().Select(index => _produkte[index - 1]).ToList();
            }

            FehlerAnzeigen($"Ungültige Auswahl. Bitte Produktnummern von 1 bis {_produkte.Count} kommasepariert eingeben.");
        }
    }

    private void WarenkorbVorschauAnzeigen(
        Bestellung bestellung,
        IReadOnlyList<(Produkt Produkt, int Menge)> warenkorb)
    {
        var tabelle = new Table().Border(TableBorder.Rounded).BorderColor(Color.Grey)
            .AddColumn(new TableColumn("[bold]Produkt[/]"))
            .AddColumn(new TableColumn("[bold]Menge[/]"))
            .AddColumn(new TableColumn("[bold]Einzelpreis[/]"))
            .AddColumn(new TableColumn("[bold]Positionspreis[/]"));

        foreach (var position in warenkorb)
        {
            tabelle.AddRow(
                Markup.Escape(position.Produkt.Beschreibung()),
                position.Menge.ToString(),
                PreisFormatieren(position.Produkt.Preis),
                PreisFormatieren(position.Produkt.Preis * position.Menge));
        }

        var bestellvorschau = new Bestellung(bestellung.Nummer);
        foreach (var position in bestellung.Positionen)
        {
            bestellvorschau.PositionHinzufuegen(position.Produkt, position.Menge);
        }

        foreach (var position in warenkorb)
        {
            bestellvorschau.PositionHinzufuegen(position.Produkt, position.Menge);
        }

        PanelAnzeigen(
            $"Warenkorb · Bestellung {bestellung.Nummer}",
            new Rows(
                tabelle,
                new Markup($"[bold]Neuer Bestellgesamtpreis:[/] [green]{PreisFormatieren(bestellvorschau.GesamtpreisBerechnen())}[/]")));
    }

    private bool BestellungBestaetigen()
    {
        if (_interaktiveKonsole)
        {
            return _konsole.Confirm("Produkte zur Bestellung hinzufügen?");
        }

        while (true)
        {
            _konsole.Markup("[blue]Warenkorb hinzufügen? (j/n): [/]");
            var eingabe = _eingabe.ReadLine();
            if (eingabe is null)
            {
                return false;
            }

            if (eingabe.Trim().Equals("j", StringComparison.OrdinalIgnoreCase)
                || eingabe.Trim().Equals("ja", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (eingabe.Trim().Equals("n", StringComparison.OrdinalIgnoreCase)
                || eingabe.Trim().Equals("nein", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            FehlerAnzeigen("Bitte mit j oder n antworten.");
        }
    }

    private void GesamtpreisAnzeigen()
    {
        var bestellung = BestellungAbfragen();
        if (bestellung is null)
        {
            return;
        }

        PanelAnzeigen(
            $"Bestellung {bestellung.Nummer}",
            new Markup($"[bold]Gesamtpreis[/]\n[bold green]{PreisFormatieren(bestellung.GesamtpreisBerechnen())}[/]"));
    }

    private void BestellungAbschliessen()
    {
        var offeneBestellungen = _verwaltung.OffeneBestellungenAnzeigen();
        if (offeneBestellungen.Count == 0)
        {
            PanelAnzeigen("Keine offenen Bestellungen", new Markup("Es gibt aktuell keine Bestellungen zum Abschließen."));
            return;
        }

        OffeneBestellungenTabelleAnzeigen(offeneBestellungen, "Offene Bestellungen zum Abschließen");
        var bestellung = OffeneBestellungAuswaehlen(offeneBestellungen, "Welche Bestellung soll abgeschlossen werden?");
        if (bestellung is null) return;
        BestellungsinhaltAnzeigen(bestellung);
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

        if (tagesumsatz == 0m || produktumsatz.Count == 0)
        {
            PanelAnzeigen(
                "Tagesumsatz",
                new Rows(
                    new Markup($"[bold]Gesamtumsatz[/]  [green]{PreisFormatieren(tagesumsatz)}[/]"),
                    new Markup("[grey]Keine Produktumsätze für heute vorhanden.[/]")));
            return;
        }

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

        PanelAnzeigen(
            "Tagesumsatz",
            new Rows(
                new Markup($"[bold]Gesamtumsatz[/]  [green]{PreisFormatieren(tagesumsatz)}[/]"),
                new Markup("[bold cyan]Umsatz nach Produkt[/]"),
                tabelle,
                diagramm));
    }

    private void OffeneBestellungenAnzeigen()
    {
        var offeneBestellungen = _verwaltung.OffeneBestellungenAnzeigen();
        if (offeneBestellungen.Count == 0)
        {
            PanelAnzeigen("Offene Bestellungen", new Markup("[grey]Keine offenen Bestellungen vorhanden.[/]"));
            return;
        }

        OffeneBestellungenTabelleAnzeigen(offeneBestellungen);
    }

    private void OffeneBestellungenTabelleAnzeigen(
        IReadOnlyList<Bestellung> bestellungen,
        string titel = "Offene Bestellungen")
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

        PanelAnzeigen(titel, tabelle);
    }

    private void BestellungsinhaltAnzeigen(Bestellung bestellung)
    {
        IRenderable inhalt;
        if (bestellung.Positionen.Count == 0)
        {
            inhalt = new Markup("[grey]Noch keine Produkte in dieser Bestellung.[/]");
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

            inhalt = tabelle;
        }

        PanelAnzeigen(
            $"Aktueller Inhalt von Bestellung {bestellung.Nummer}",
            new Rows(
                inhalt,
                new Markup($"[bold]Aktueller Gesamtpreis:[/] [green]{PreisFormatieren(bestellung.GesamtpreisBerechnen())}[/]")));
    }

    private Bestellung? BestellungAbfragen()
    {
        while (true)
        {
            var nummer = ZahlAbfragen("Bestellnummer (0 = Abbrechen): ", zahl => zahl > 0, "Ungültige Bestellnummer.");
            if (nummer is null)
            {
                return null;
            }

            var bestellung = _verwaltung.BestellungSuchen(nummer.Value);
            if (bestellung is not null)
            {
                return bestellung;
            }

            FehlerAnzeigen("Bestellung nicht gefunden.");
        }
    }

    private MenueAktion? MenueAuswahlAbfragen()
    {
        if (_interaktiveKonsole)
        {
            return _konsole.Prompt(
                new SelectionPrompt<MenueAktion>()
                    .Title("[bold]Was möchten Sie tun?[/]")
                    .HighlightStyle(new Style(Color.Cyan1, decoration: Decoration.Bold))
                    .UseConverter(MenueAktionBeschreiben)
                    .AddChoices(Enum.GetValues<MenueAktion>()));
        }

        while (true)
        {
            _konsole.Markup("[bold blue]Auswahl: [/]");
            var eingabe = _eingabe.ReadLine();
            if (eingabe is null)
            {
                return null;
            }

            if (int.TryParse(eingabe, out var auswahl) && auswahl >= 0 && auswahl <= 7)
            {
                return auswahl switch
                {
                    1 => MenueAktion.ProdukteAnzeigen,
                    2 => MenueAktion.BestellungStarten,
                    3 => MenueAktion.ProduktHinzufuegen,
                    4 => MenueAktion.GesamtpreisAnzeigen,
                    5 => MenueAktion.BestellungAbschliessen,
                    6 => MenueAktion.TagesumsatzAnzeigen,
                    7 => MenueAktion.OffeneBestellungenAnzeigen,
                    _ => MenueAktion.Beenden
                };
            }

            FehlerAnzeigen("Ungültige Auswahl. Bitte wählen Sie eine Zahl von 0 bis 7.");
        }
    }

    private Bestellung? OffeneBestellungAuswaehlen(IReadOnlyList<Bestellung> bestellungen, string titel)
    {
        if (_interaktiveKonsole)
        {
            var auswahl = bestellungen.Select(bestellung => new BestellungPromptAuswahl(bestellung))
                .Append(new BestellungPromptAuswahl(null));
            return _konsole.Prompt(
                new SelectionPrompt<BestellungPromptAuswahl>()
                    .Title($"[bold]{Markup.Escape(titel)}[/]")
                    .HighlightStyle(new Style(Color.Cyan1, decoration: Decoration.Bold))
                    .UseConverter(auswahl => auswahl.Bestellung is null
                        ? "[grey]Zurück zum Hauptmenü[/]"
                        : $"[yellow]Bestellung {auswahl.Bestellung.Nummer}[/] · {Markup.Escape(BestellpositionenKurzAnzeigen(auswahl.Bestellung))} · {PreisFormatieren(auswahl.Bestellung.GesamtpreisBerechnen())}")
                    .AddChoices(auswahl)).Bestellung;
        }

        while (true)
        {
            var nummer = ZahlAbfragen(
                "Bestellnummer aus der Übersicht (0 = Abbrechen): ",
                zahl => bestellungen.Any(bestellung => bestellung.Nummer == zahl),
                "Diese Nummer gehört zu keiner offenen Bestellung.");
            if (nummer is null) return null;
            var bestellung = bestellungen.FirstOrDefault(offen => offen.Nummer == nummer.Value);
            if (bestellung is not null) return bestellung;
        }
    }

    private static string BestellpositionenKurzAnzeigen(Bestellung bestellung)
    {
        return bestellung.Positionen.Count == 0
            ? "Noch keine Produkte"
            : string.Join(", ", bestellung.Positionen.Select(position =>
                $"{position.Menge} × {position.Produkt.Beschreibung()}"));
    }

    private static string MenueAktionBeschreiben(MenueAktion aktion)
    {
        return aktion switch
        {
            MenueAktion.ProdukteAnzeigen => "Produkte anzeigen",
            MenueAktion.BestellungStarten => "Bestellung starten",
            MenueAktion.ProduktHinzufuegen => "Produkt hinzufügen",
            MenueAktion.GesamtpreisAnzeigen => "Gesamtpreis anzeigen",
            MenueAktion.BestellungAbschliessen => "Bestellung abschließen",
            MenueAktion.TagesumsatzAnzeigen => "Tagesumsatz anzeigen",
            MenueAktion.OffeneBestellungenAnzeigen => "Offene Bestellungen anzeigen",
            _ => "Beenden"
        };
    }

    private enum MenueAktion
    {
        ProdukteAnzeigen,
        BestellungStarten,
        ProduktHinzufuegen,
        GesamtpreisAnzeigen,
        BestellungAbschliessen,
        TagesumsatzAnzeigen,
        OffeneBestellungenAnzeigen,
        Beenden
    }

    private sealed record BestellungPromptAuswahl(Bestellung? Bestellung);

    private sealed record ProduktPromptAuswahl(Produkt? Produkt);

    private int? ZahlAbfragen(string aufforderung, Func<int, bool> istGueltig, string fehlermeldung)
    {
        while (true)
        {
            _konsole.Markup($"[blue]{Markup.Escape(aufforderung)}[/]");
            var eingabe = _eingabe.ReadLine();
            if (eingabe is null)
            {
                return null;
            }

            if (int.TryParse(eingabe, out var zahl) && zahl == 0)
            {
                return null;
            }

            if (int.TryParse(eingabe, out zahl) && istGueltig(zahl))
            {
                return zahl;
            }

            FehlerAnzeigen(fehlermeldung);
        }
    }

    private static string PreisFormatieren(decimal preis)
    {
        return $"{preis.ToString("F2", DeutscheKultur)} €";
    }

    private void PanelAnzeigen(string titel, IRenderable inhalt)
    {
        _konsole.Write(
            new Panel(inhalt)
                .Header($"[bold yellow]{Markup.Escape(titel)}[/]")
                .Border(BoxBorder.Rounded)
                .BorderColor(Color.Cyan1)
                .Padding(1, 0));
    }

    private void ErfolgAnzeigen(string text)
    {
        PanelAnzeigen("Erfolgreich", new Markup($"[bold green]{Markup.Escape(text)}[/]"));
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
