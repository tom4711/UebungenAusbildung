using CafeBestellsystem.Core.Bestellungen;
using CafeBestellsystem.Core.Produkte;
using CafeBestellsystem.Konsole;

var produkte = new List<Produkt>
{
    new Kaffee("Filterkaffee", 3.50m, true, 250, "Arabica"),
    new Getraenk("Apfelschorle", 3.00m, false, 330),
    new Speise("Käsekuchen", 4.20m, true),
    new Speise("Laugenstange", 2.50m, false)
};

var menue = new CafeKonsolenMenue(
    Console.In,
    Console.Out,
    new Bestellverwaltung(),
    produkte);

menue.Starten();
