using CafeBestellsystem.Core.Produkte;
using Xunit;

namespace CafeBestellsystem.Tests;

public class ProdukteTests
{
    [Fact]
    public void Produkt_GueltigeDaten_BehaltNameUndPreis()
    {
        var produkt = new TestProdukt(" Espresso ", 2.50m);

        Assert.Equal("Espresso", produkt.Name);
        Assert.Equal(2.50m, produkt.Preis);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Produkt_LeererName_WirftArgumentException(string name)
    {
        Assert.Throws<ArgumentException>(() => new TestProdukt(name, 2m));
    }

    [Fact]
    public void Produkt_NegativerPreis_WirftArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new TestProdukt("Tee", -0.01m));
    }

    [Fact]
    public void Getraenk_NichtPositiveGroesse_WirftArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Getraenk("Tee", 2m, true, 0));
    }

    [Fact]
    public void Kaffee_LeereBohnensorte_WirftArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Kaffee("Kaffee", 3m, true, 200, " "));
    }

    [Fact]
    public void Produkte_BeschreibungenWerdenPolymorphAufgerufen()
    {
        var produkte = new List<Produkt>
        {
            new Getraenk("Tee", 2m, true, 250),
            new Speise("Kuchen", 3.5m, true),
            new Kaffee("Espresso", 2.5m, false, 60, "Arabica")
        };

        var beschreibungen = produkte.Select(produkt => produkt.Beschreibung()).ToList();

        Assert.Contains("250", beschreibungen[0]);
        Assert.Contains("vegetarisch", beschreibungen[1], StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Arabica", beschreibungen[2]);
    }

    private sealed class TestProdukt(string name, decimal preis) : Produkt(name, preis)
    {
        public override string Beschreibung() => Name;
    }
}
