using CafeBestellsystem.Core.Bestellungen;
using CafeBestellsystem.Core.Produkte;
using Xunit;

namespace CafeBestellsystem.Tests;

public class BestellablaufTests
{
    [Fact]
    public void BestellungMitRabattAbschliessen_ErscheintImTagesumsatzUndBleibtUnveraenderlich()
    {
        var verwaltung = new Bestellverwaltung();
        var datum = new DateOnly(2026, 10, 31);
        var bestellung = verwaltung.BestellungErstellen();
        var kaffee = new Kaffee("Filterkaffee", 4m, true, 250, "Arabica");
        var kuchen = new Speise("Kuchen", 8m, true);

        bestellung.PositionHinzufuegen(kaffee, 2);
        bestellung.PositionHinzufuegen(kuchen, 2);

        Assert.Equal(24m, bestellung.WarenwertBerechnen());
        Assert.Equal(21.60m, bestellung.GesamtpreisBerechnen());
        Assert.True(bestellung.Abschliessen(datum));

        Assert.False(bestellung.PositionHinzufuegen(kuchen, 1));
        Assert.Equal(21.60m, verwaltung.TagesumsatzBerechnen(datum));
    }
}
