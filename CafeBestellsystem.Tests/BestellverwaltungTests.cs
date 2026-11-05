using CafeBestellsystem.Core.Bestellungen;
using CafeBestellsystem.Core.Produkte;
using Xunit;

namespace CafeBestellsystem.Tests;

public class BestellverwaltungTests
{
    [Fact]
    public void BestellungErstellen_VergibtFortlaufendeNummern()
    {
        var verwaltung = new Bestellverwaltung();

        Assert.Equal(1, verwaltung.BestellungErstellen().Nummer);
        Assert.Equal(2, verwaltung.BestellungErstellen().Nummer);
    }

    [Fact]
    public void BestellungSuchen_UnbekannteNummer_GibtNullZurueck()
    {
        var verwaltung = new Bestellverwaltung();

        Assert.Null(verwaltung.BestellungSuchen(42));
    }

    [Fact]
    public void Bestelllisten_Leer_GebenLeereListenUndNullEuroUmsatzZurueck()
    {
        var verwaltung = new Bestellverwaltung();

        Assert.Empty(verwaltung.OffeneBestellungenAnzeigen());
        Assert.Empty(verwaltung.AbgeschlosseneBestellungenAnzeigen());
        Assert.Empty(verwaltung.NachGesamtpreisSortieren());
        Assert.Equal(0m, verwaltung.TagesumsatzBerechnen(new DateOnly(2026, 10, 31)));
    }

    [Fact]
    public void Bestellungen_FilterUndSortierung_BeruecksichtigenStatusUndPreis()
    {
        var verwaltung = new Bestellverwaltung();
        var teuer = verwaltung.BestellungErstellen();
        teuer.PositionHinzufuegen(new Speise("Kuchen", 12m, true), 1);
        var guenstig = verwaltung.BestellungErstellen();
        guenstig.PositionHinzufuegen(new Getraenk("Tee", 3m, true, 200), 1);
        var abgeschlossen = verwaltung.BestellungErstellen();
        abgeschlossen.Abschliessen(new DateOnly(2026, 10, 31));

        Assert.Same(guenstig, verwaltung.BestellungSuchen(guenstig.Nummer));
        Assert.Equal([teuer, guenstig], verwaltung.OffeneBestellungenAnzeigen());
        Assert.Equal([abgeschlossen], verwaltung.AbgeschlosseneBestellungenAnzeigen());
        Assert.Equal([abgeschlossen, guenstig, teuer], verwaltung.NachGesamtpreisSortieren());
    }

    [Fact]
    public void TagesumsatzBerechnen_NurAbgeschlosseneBestellungenDesAngegebenenTages()
    {
        var verwaltung = new Bestellverwaltung();
        var heute = new DateOnly(2026, 10, 31);
        var gestern = heute.AddDays(-1);
        var heuteBestellung = verwaltung.BestellungErstellen();
        heuteBestellung.PositionHinzufuegen(new Speise("Kuchen", 5m, true), 1);
        heuteBestellung.Abschliessen(heute);
        var gesternBestellung = verwaltung.BestellungErstellen();
        gesternBestellung.PositionHinzufuegen(new Speise("Suppe", 7m, false), 1);
        gesternBestellung.Abschliessen(gestern);
        var offen = verwaltung.BestellungErstellen();
        offen.PositionHinzufuegen(new Speise("Torte", 9m, true), 1);

        Assert.Equal(5m, verwaltung.TagesumsatzBerechnen(heute));
    }

    [Fact]
    public void ProduktumsatzBerechnen_GruppiertProdukteUndVerteiltRabattCentgenau()
    {
        var verwaltung = new Bestellverwaltung();
        var heute = new DateOnly(2026, 10, 31);
        var gestern = heute.AddDays(-1);
        var rabattBestellung = verwaltung.BestellungErstellen();
        rabattBestellung.PositionHinzufuegen(new Speise("Alpha", 6.67m, true), 1);
        rabattBestellung.PositionHinzufuegen(new Speise("Beta", 6.67m, true), 1);
        rabattBestellung.PositionHinzufuegen(new Speise("Gamma", 6.67m, true), 1);
        rabattBestellung.Abschliessen(heute);
        var weitereBestellung = verwaltung.BestellungErstellen();
        weitereBestellung.PositionHinzufuegen(new Speise("Alpha", 3m, true), 1);
        weitereBestellung.Abschliessen(heute);
        var andereTagesbestellung = verwaltung.BestellungErstellen();
        andereTagesbestellung.PositionHinzufuegen(new Speise("Gestern", 50m, true), 1);
        andereTagesbestellung.Abschliessen(gestern);
        var offen = verwaltung.BestellungErstellen();
        offen.PositionHinzufuegen(new Speise("Offen", 40m, true), 1);
        var storniert = verwaltung.BestellungErstellen();
        storniert.PositionHinzufuegen(new Speise("Storniert", 30m, true), 1);
        storniert.Stornieren();

        var produktumsatz = verwaltung.ProduktumsatzBerechnen(heute);

        Assert.Equal(3, produktumsatz.Count);
        Assert.Equal(9.01m, produktumsatz[new Speise("Alpha", 1m, true).Beschreibung()]);
        Assert.Equal(6m, produktumsatz[new Speise("Beta", 1m, true).Beschreibung()]);
        Assert.Equal(6m, produktumsatz[new Speise("Gamma", 1m, true).Beschreibung()]);
        Assert.Equal(verwaltung.TagesumsatzBerechnen(heute), produktumsatz.Values.Sum());
        Assert.Equal(21.01m, produktumsatz.Values.Sum());
    }

    [Fact]
    public void ProduktumsatzBerechnen_OhnePassendeAbschluesse_GibtLeeresErgebnis()
    {
        var verwaltung = new Bestellverwaltung();

        Assert.Empty(verwaltung.ProduktumsatzBerechnen(new DateOnly(2026, 10, 31)));
    }
}
