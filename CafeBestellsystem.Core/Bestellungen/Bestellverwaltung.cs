namespace CafeBestellsystem.Core.Bestellungen;

public class Bestellverwaltung
{
    private readonly List<Bestellung> _bestellungen = new();
    private int _naechsteNummer = 1;

    public Bestellung BestellungErstellen()
    {
        var bestellung = new Bestellung(_naechsteNummer);
        _naechsteNummer++;
        _bestellungen.Add(bestellung);
        return bestellung;
    }

    public Bestellung? BestellungSuchen(int nummer)
    {
        return _bestellungen.FirstOrDefault(bestellung => bestellung.Nummer == nummer);
    }

    public IReadOnlyList<Bestellung> OffeneBestellungenAnzeigen()
    {
        return _bestellungen
            .Where(bestellung => bestellung.Status == Bestellstatus.Offen)
            .ToList();
    }

    public IReadOnlyList<Bestellung> AbgeschlosseneBestellungenAnzeigen()
    {
        return _bestellungen
            .Where(bestellung => bestellung.Status == Bestellstatus.Abgeschlossen)
            .ToList();
    }

    public IReadOnlyList<Bestellung> NachGesamtpreisSortieren()
    {
        return _bestellungen
            .OrderBy(bestellung => bestellung.GesamtpreisBerechnen())
            .ToList();
    }

    public decimal TagesumsatzBerechnen(DateOnly datum)
    {
        return _bestellungen
            .Where(bestellung => bestellung.Status == Bestellstatus.Abgeschlossen
                && bestellung.Abschlussdatum == datum)
            .Sum(bestellung => bestellung.GesamtpreisBerechnen());
    }

    public IReadOnlyDictionary<string, decimal> ProduktumsatzBerechnen(DateOnly datum)
    {
        var produktumsatz = new SortedDictionary<string, decimal>(StringComparer.Ordinal);
        var abgeschlosseneBestellungen = _bestellungen
            .Where(bestellung => bestellung.Status == Bestellstatus.Abgeschlossen
                && bestellung.Abschlussdatum == datum);

        foreach (var bestellung in abgeschlosseneBestellungen)
        {
            var produkte = bestellung.Positionen
                .GroupBy(position => position.Produkt.Beschreibung(), StringComparer.Ordinal)
                .Select(gruppe => new
                {
                    Beschreibung = gruppe.Key,
                    Warenwert = gruppe.Sum(position => position.Positionspreis)
                })
                .OrderBy(produkt => produkt.Beschreibung, StringComparer.Ordinal)
                .ToList();

            var warenwert = produkte.Sum(produkt => produkt.Warenwert);
            if (warenwert == 0m)
            {
                continue;
            }

            var gesamtpreis = bestellung.GesamtpreisBerechnen();
            var anteile = produkte.ToDictionary(
                produkt => produkt.Beschreibung,
                produkt => decimal.Round(
                    gesamtpreis * produkt.Warenwert / warenwert,
                    2,
                    MidpointRounding.AwayFromZero),
                StringComparer.Ordinal);

            var rundungsrest = gesamtpreis - anteile.Values.Sum();
            if (rundungsrest != 0m)
            {
                var empfaenger = produkte
                    .OrderByDescending(produkt => produkt.Warenwert)
                    .ThenBy(produkt => produkt.Beschreibung, StringComparer.Ordinal)
                    .First();
                anteile[empfaenger.Beschreibung] += rundungsrest;
            }

            foreach (var anteil in anteile)
            {
                produktumsatz[anteil.Key] = produktumsatz.GetValueOrDefault(anteil.Key) + anteil.Value;
            }
        }

        return produktumsatz;
    }
}
