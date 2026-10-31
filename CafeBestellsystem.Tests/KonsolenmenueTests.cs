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
        var menue = NeuesMenue("-1\n7\n0\n", ausgabe);

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
}
