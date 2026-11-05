using CafeBestellsystem.Core.Bestellungen;
using CafeBestellsystem.Core.Produkte;
using CafeBestellsystem.Konsole;
using Xunit;

namespace CafeBestellsystem.Tests;

public class KonsolenmenueTests
{
    [Fact]
    public void Starten_UngueltigeAuswahl_GibtMeldungUndKannBeendetWerden()
    {
        var ausgabe = new StringWriter();
        var menue = NeuesMenue("abc\n0\n", ausgabe);

        menue.Starten();

        Assert.Contains("Ungültige Auswahl", ausgabe.ToString());
        Assert.Contains("Auf Wiedersehen", ausgabe.ToString());
    }

    [Fact]
    public void Starten_LeereAuswahl_GibtMeldungUndKannBeendetWerden()
    {
        var ausgabe = new StringWriter();
        var menue = NeuesMenue("\n0\n", ausgabe);

        menue.Starten();

        Assert.Contains("Ungültige Auswahl", ausgabe.ToString());
        Assert.Contains("Auf Wiedersehen", ausgabe.ToString());
    }

    [Fact]
    public void Starten_AuswahlAusserhalbBereichs_GibtMeldungUndKannBeendetWerden()
    {
        var ausgabe = new StringWriter();
        var menue = NeuesMenue("-1\n8\n0\n", ausgabe);

        menue.Starten();

        Assert.Equal(2, ausgabe.ToString().Split("Ungültige Auswahl").Length - 1);
        Assert.Contains("Auf Wiedersehen", ausgabe.ToString());
    }

    [Fact]
    public void Starten_UngueltigeBestellnummern_GibtMeldungUndKannMitGueltigerNummerFortfahren()
    {
        var ausgabe = new StringWriter();
        var menue = NeuesMenue("4\n\n4\n2147483648\n2\n4\n1\n0\n", ausgabe);

        menue.Starten();

        Assert.Equal(2, ausgabe.ToString().Split("Ungültige Bestellnummer").Length - 1);
        Assert.Contains("Gesamtpreis: 0,00 €", ausgabe.ToString());
    }

    [Fact]
    public void Starten_UngueltigeProduktnummernUndMengen_FuegenKeinePositionHinzu()
    {
        var ausgabe = new StringWriter();
        var menue = NeuesMenue(
            "2\n"
            + "3\n1\n\n"
            + "3\n1\n2147483648\n"
            + "3\n1\n1\n0\n"
            + "3\n1\n1\n2147483648\n"
            + "3\n1\n1\n2\n"
            + "4\n1\n0\n",
            ausgabe);

        menue.Starten();

        Assert.Equal(2, ausgabe.ToString().Split("Ungültige Produktnummer").Length - 1);
        Assert.Equal(2, ausgabe.ToString().Split("Die Menge muss eine positive Zahl sein").Length - 1);
        Assert.Contains("Gesamtpreis: 7,00 €", ausgabe.ToString());
    }

    [Fact]
    public void Starten_UnbekannteBestellnummer_GibtMeldungUndLaeuftWeiter()
    {
        var ausgabe = new StringWriter();
        var menue = NeuesMenue("4\n999\n0\n", ausgabe);

        menue.Starten();

        Assert.Contains("Bestellung nicht gefunden", ausgabe.ToString());
        Assert.Contains("Auf Wiedersehen", ausgabe.ToString());
    }

    [Fact]
    public void Starten_VollstaendigerAblauf_BerechnetUndZeigtPreisUndTagesumsatz()
    {
        var ausgabe = new StringWriter();
        var menue = NeuesMenue("2\n3\n1\n1\n2\n4\n1\n5\n1\n6\n0\n", ausgabe);

        menue.Starten();

        Assert.Contains("Gesamtpreis: 7,00 €", ausgabe.ToString());
        Assert.Contains("Tagesumsatz: 7,00 €", ausgabe.ToString());
        Assert.Contains("Auf Wiedersehen", ausgabe.ToString());
    }

    [Fact]
    public void Starten_GeschlosseneBestellung_LehntProduktVorProduktabfrageAb()
    {
        var ausgabe = new StringWriter();
        var verwaltung = new Bestellverwaltung();
        var bestellung = verwaltung.BestellungErstellen();
        bestellung.Abschliessen(DateOnly.FromDateTime(DateTime.Today));
        var menue = NeuesMenueMitVerwaltung("3\n1\n0\n", ausgabe, verwaltung);

        menue.Starten();

        var text = ausgabe.ToString();
        Assert.Contains("Keine offenen Bestellungen", text);
        Assert.DoesNotContain("Produktnummer:", text);
    }

    [Fact]
    public void Starten_Produktseite_WartetVorDemNaechstenMenueAufEnter()
    {
        var ausgabe = new StringWriter();
        var verwaltung = new Bestellverwaltung();
        var menue = NeuesMenueMitVerwaltung("1\n\n0\n", ausgabe, verwaltung, interaktiv: true);

        menue.Starten();

        var text = ausgabe.ToString();
        var produktIndex = text.IndexOf("Tee", StringComparison.Ordinal);
        var weiterIndex = text.IndexOf("Weiter mit Enter", StringComparison.Ordinal);
        var naechstesMenueIndex = text.IndexOf("Café-Bestellsystem", produktIndex + 1, StringComparison.Ordinal);

        Assert.True(produktIndex >= 0);
        Assert.True(weiterIndex > produktIndex);
        Assert.True(naechstesMenueIndex > weiterIndex);
    }

    [Fact]
    public void Starten_OffeneBestellungenAnzeigen_ZeigtPositionenMengenUndSumme()
    {
        var ausgabe = new StringWriter();
        var verwaltung = new Bestellverwaltung();
        var offen = verwaltung.BestellungErstellen();
        offen.PositionHinzufuegen(new Getraenk("Tee", 3.50m, true, 250), 2);
        verwaltung.BestellungErstellen();
        var abgeschlossen = verwaltung.BestellungErstellen();
        abgeschlossen.PositionHinzufuegen(new Speise("Geschlossener Kuchen", 4m, true), 1);
        abgeschlossen.Abschliessen(DateOnly.FromDateTime(DateTime.Today));
        var menue = NeuesMenueMitVerwaltung("7\n0\n", ausgabe, verwaltung);

        menue.Starten();

        var text = ausgabe.ToString();
        Assert.Contains("Tee", text);
        Assert.Contains("2 × Tee", text);
        Assert.Contains("Noch keine Produkte", text);
        Assert.Contains("7,00 €", text);
        Assert.DoesNotContain("Geschlossener Kuchen", text);
    }

    [Fact]
    public void Starten_ProduktHinzufuegen_ZeigtOffeneBestellungUndDerenInhaltVorProduktabfrage()
    {
        var ausgabe = new StringWriter();
        var verwaltung = new Bestellverwaltung();
        var bestellung = verwaltung.BestellungErstellen();
        bestellung.PositionHinzufuegen(new Getraenk("Tee", 3.50m, true, 250), 1);
        var menue = NeuesMenueMitVerwaltung("3\n1\n1\n2\n0\n", ausgabe, verwaltung);

        menue.Starten();

        var text = ausgabe.ToString();
        var inhaltIndex = text.IndexOf("Tee", StringComparison.Ordinal);
        var produktabfrageIndex = text.IndexOf("Produktnummer:", StringComparison.Ordinal);
        Assert.True(inhaltIndex >= 0);
        Assert.True(produktabfrageIndex > inhaltIndex);
        Assert.Equal(2, bestellung.AnzahlPositionen());
        Assert.Equal(10.50m, bestellung.GesamtpreisBerechnen());
    }

    [Fact]
    public void Starten_ProduktHinzufuegen_OhneOffeneBestellungFragtKeinProduktAb()
    {
        var ausgabe = new StringWriter();
        var verwaltung = new Bestellverwaltung();
        var menue = NeuesMenueMitVerwaltung("3\n0\n", ausgabe, verwaltung);

        menue.Starten();

        Assert.Contains("Keine offenen Bestellungen", ausgabe.ToString());
        Assert.DoesNotContain("Produktnummer:", ausgabe.ToString());
    }

    [Fact]
    public void Starten_ProduktHinzufuegen_UngueltigeBestellnummerFragtKeinProduktAb()
    {
        var ausgabe = new StringWriter();
        var verwaltung = new Bestellverwaltung();
        verwaltung.BestellungErstellen();
        var menue = NeuesMenueMitVerwaltung("3\n999\n0\n", ausgabe, verwaltung);

        menue.Starten();

        Assert.Contains("keiner offenen Bestellung", ausgabe.ToString());
        Assert.DoesNotContain("Produktnummer:", ausgabe.ToString());
    }

    [Fact]
    public void Starten_TagesumsatzMitUmsatz_ZeigtProduktanteile()
    {
        var ausgabe = new StringWriter();
        var verwaltung = new Bestellverwaltung();
        var bestellung = verwaltung.BestellungErstellen();
        bestellung.PositionHinzufuegen(new Getraenk("Tee", 3.50m, true, 250), 2);
        bestellung.PositionHinzufuegen(new Speise("Kuchen", 4.20m, true), 1);
        bestellung.Abschliessen(DateOnly.FromDateTime(DateTime.Today));
        var menue = NeuesMenueMitVerwaltung("6\n0\n", ausgabe, verwaltung);

        menue.Starten();

        var text = ausgabe.ToString();
        Assert.Contains("Umsatz nach Produkt", text);
        Assert.Contains("Tee (heiß, 250 ml)", text);
        Assert.Contains("Kuchen (vegetarisch)", text);
        Assert.Contains("7,00 €", text);
        Assert.Contains("4,20 €", text);
        Assert.Contains("62.5%", text);
    }

    [Fact]
    public void Starten_TagesumsatzOhneAbschluesse_ZeigtHinweisOhneDiagramm()
    {
        var ausgabe = new StringWriter();
        var verwaltung = new Bestellverwaltung();
        var menue = NeuesMenueMitVerwaltung("6\n0\n", ausgabe, verwaltung);

        menue.Starten();

        Assert.Contains("Keine Produktumsätze", ausgabe.ToString());
        Assert.DoesNotContain("Umsatz nach Produkt", ausgabe.ToString());
    }

    [Fact]
    public void Starten_EingabeEndet_BeendetMenueOhneEndlosschleife()
    {
        var ausgabe = new StringWriter();
        var menue = NeuesMenue("", ausgabe);

        menue.Starten();

        Assert.Contains("Auf Wiedersehen", ausgabe.ToString());
    }

    private static CafeKonsolenMenue NeuesMenue(string eingabe, StringWriter ausgabe)
    {
        var verwaltung = new Bestellverwaltung();
        IReadOnlyList<Produkt> produkte = new List<Produkt>
        {
            new Getraenk("Tee", 3.50m, true, 250)
        };
        return new CafeKonsolenMenue(new StringReader(eingabe), ausgabe, verwaltung, produkte);
    }

    private static CafeKonsolenMenue NeuesMenueMitVerwaltung(
        string eingabe,
        StringWriter ausgabe,
        Bestellverwaltung verwaltung,
        bool interaktiv = false)
    {
        IReadOnlyList<Produkt> produkte = new List<Produkt>
        {
            new Getraenk("Tee", 3.50m, true, 250)
        };
        return new CafeKonsolenMenue(new StringReader(eingabe), ausgabe, verwaltung, produkte, interaktiv);
    }
}
