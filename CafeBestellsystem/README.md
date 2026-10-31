# Café-Bestellsystem

Ein kleines Konsolenprojekt zum Üben von C#-Klassen, Vererbung, Listen, Eingabeprüfung und automatisierten Tests. Die Fachregeln liegen im Projekt `CafeBestellsystem.Core`; die Konsole übernimmt nur Ein- und Ausgabe.

Voraussetzung ist das .NET 11 SDK.

Führe alle Befehle vom Repository-Ordner aus.

## Starten

```bash
dotnet run --project CafeBestellsystem/CafeBestellsystem.csproj
```

## Tests ausführen

```bash
dotnet test CafeBestellsystem.slnx
```

## Bauen

```bash
dotnet build CafeBestellsystem.slnx
```

Das Testprojekt prüft einzelne Regeln und vollständige Abläufe mit xUnit.
