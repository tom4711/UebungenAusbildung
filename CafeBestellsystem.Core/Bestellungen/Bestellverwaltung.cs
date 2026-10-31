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
}
