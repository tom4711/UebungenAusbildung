using CafeBestellsystem.Core.Bestellungen;
using CafeBestellsystem.Core.Produkte;
using Xunit;

namespace CafeBestellsystem.Tests;

public class BestellpositionTests
{
    [Fact]
    public void Positionspreis_MengeEins_EntsprichtProduktpreis()
    {
        var position = new Bestellposition(new Speise("Kuchen", 3.50m, true), 1);

        Assert.Equal(3.50m, position.Positionspreis);
    }

    [Fact]
    public void Positionspreis_GroessereMenge_MultipliziertDenProduktpreis()
    {
        var position = new Bestellposition(new Speise("Kuchen", 3.50m, true), 3);

        Assert.Equal(10.50m, position.Positionspreis);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Bestellposition_NichtPositiveMenge_WirftArgumentOutOfRangeException(int menge)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Bestellposition(new Speise("Kuchen", 3m, true), menge));
    }

    [Fact]
    public void Bestellposition_ProduktFehlt_WirftArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new Bestellposition(null!, 1));
    }
}
