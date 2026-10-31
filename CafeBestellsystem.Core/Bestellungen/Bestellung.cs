using CafeBestellsystem.Core.Produkte;

namespace CafeBestellsystem.Core.Bestellungen;

public class Bestellung
{
    private const decimal RabattSchwelle = 20.00m;
    private const decimal RabattFaktor = 0.90m;
    private readonly List<Bestellposition> _positionen = new();

    public Bestellung(int nummer)
    {
        if (nummer <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(nummer), "Die Bestellnummer muss größer als 0 sein.");
        }

        Nummer = nummer;
    }

    public int Nummer { get; }

    public IReadOnlyList<Bestellposition> Positionen => _positionen.AsReadOnly();

    public Bestellstatus Status { get; private set; } = Bestellstatus.Offen;

    public DateOnly? Abschlussdatum { get; private set; }

    public bool PositionHinzufuegen(Produkt produkt, int menge)
    {
        if (Status != Bestellstatus.Offen)
        {
            return false;
        }

        _positionen.Add(new Bestellposition(produkt, menge));
        return true;
    }

    public decimal WarenwertBerechnen()
    {
        return _positionen.Sum(position => position.Positionspreis);
    }

    public decimal GesamtpreisBerechnen()
    {
        var warenwert = WarenwertBerechnen();
        if (warenwert < RabattSchwelle)
        {
            return warenwert;
        }

        return decimal.Round(warenwert * RabattFaktor, 2, MidpointRounding.AwayFromZero);
    }

    public int AnzahlPositionen()
    {
        return _positionen.Count;
    }

    public bool Abschliessen(DateOnly datum)
    {
        if (Status != Bestellstatus.Offen)
        {
            return false;
        }

        Status = Bestellstatus.Abgeschlossen;
        Abschlussdatum = datum;
        return true;
    }

    public bool Stornieren()
    {
        if (Status != Bestellstatus.Offen)
        {
            return false;
        }

        Status = Bestellstatus.Storniert;
        return true;
    }
}
