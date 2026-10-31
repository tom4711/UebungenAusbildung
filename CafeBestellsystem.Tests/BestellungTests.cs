using CafeBestellsystem.Core.Bestellungen;
using CafeBestellsystem.Core.Produkte;
using Xunit;

namespace CafeBestellsystem.Tests;

public class BestellungTests
{
    [Fact]
    public void GesamtpreisBerechnen_LeereBestellung_GibtNullEuroZurueck()
    {
        var bestellung = new Bestellung(1);

        Assert.Equal(0m, bestellung.GesamtpreisBerechnen());
        Assert.Equal(0, bestellung.AnzahlPositionen());
    }

    [Fact]
    public void PositionHinzufuegen_MehrerePositionen_BerechnetWarenwertUndRabatt()
    {
        var bestellung = new Bestellung(1);

        bestellung.PositionHinzufuegen(new Speise("Kuchen", 5m, true), 2);
        bestellung.PositionHinzufuegen(new Getraenk("Tee", 10m, true, 250), 1);

        Assert.Equal(20m, bestellung.WarenwertBerechnen());
        Assert.Equal(18m, bestellung.GesamtpreisBerechnen());
    }

    [Fact]
    public void PositionHinzufuegen_GleichesProduktZweimal_BehaltGetrenntePositionen()
    {
        var bestellung = new Bestellung(1);
        var tee = new Getraenk("Tee", 2m, true, 250);

        bestellung.PositionHinzufuegen(tee, 1);
        bestellung.PositionHinzufuegen(tee, 2);

        Assert.Equal(2, bestellung.AnzahlPositionen());
        Assert.Collection(
            bestellung.Positionen,
            position => Assert.Equal(1, position.Menge),
            position => Assert.Equal(2, position.Menge));
    }

    [Fact]
    public void GesamtpreisBerechnen_UnterRabattgrenze_GibtWarenwertZurueck()
    {
        var bestellung = BestellungMitWarenwert(19.99m);

        Assert.Equal(19.99m, bestellung.GesamtpreisBerechnen());
    }

    [Fact]
    public void GesamtpreisBerechnen_AnDerRabattgrenze_GibtZehnProzentRabatt()
    {
        var bestellung = BestellungMitWarenwert(20.00m);

        Assert.Equal(18.00m, bestellung.GesamtpreisBerechnen());
    }

    [Fact]
    public void GesamtpreisBerechnen_HalbcentErgebnis_RundetVomKundenWeg()
    {
        var bestellung = BestellungMitWarenwert(20.05m);

        Assert.Equal(18.05m, bestellung.GesamtpreisBerechnen());
    }

    [Fact]
    public void Bestellung_Abgeschlossen_SpeichertDatumUndAkzeptiertKeineAenderungen()
    {
        var bestellung = new Bestellung(1);
        var datum = new DateOnly(2026, 10, 31);

        Assert.True(bestellung.Abschliessen(datum));

        Assert.Equal(Bestellstatus.Abgeschlossen, bestellung.Status);
        Assert.Equal(datum, bestellung.Abschlussdatum);
        Assert.False(bestellung.Abschliessen(datum));
        Assert.False(bestellung.PositionHinzufuegen(new Speise("Kuchen", 3m, true), 1));
        Assert.False(bestellung.Stornieren());
    }

    [Fact]
    public void Bestellung_Storniert_KannNichtAbgeschlossenOderGeaendertWerden()
    {
        var bestellung = new Bestellung(1);

        Assert.True(bestellung.Stornieren());

        Assert.Equal(Bestellstatus.Storniert, bestellung.Status);
        Assert.False(bestellung.Abschliessen(new DateOnly(2026, 10, 31)));
        Assert.False(bestellung.PositionHinzufuegen(new Speise("Kuchen", 3m, true), 1));
    }

    [Fact]
    public void Bestellung_NummerMussPositivSein()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Bestellung(0));
    }

    private static Bestellung BestellungMitWarenwert(decimal warenwert)
    {
        var bestellung = new Bestellung(1);
        bestellung.PositionHinzufuegen(new Speise("Tagesgericht", warenwert, false), 1);
        return bestellung;
    }
}
