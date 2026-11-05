# Café-Warenkorb-Prompt Implementation Plan

> Inline-Umsetzung in dieser Sitzung. Schritte sind einzeln überprüfbar.

**Ziel:** Mehrere Produkte für eine offene Bestellung interaktiv auswählen, Mengen erfassen, eine ansprechend formatierte Warenkorb-Vorschau zeigen und erst nach Bestätigung hinzufügen.

**Architektur:** Änderung bleibt in `CafeKonsolenMenue`; Spectre.Console-Prompts laufen über die injizierte `IAnsiConsole`. Die Mengen werden vor jeder Mutation gesammelt. Nach Bestätigung werden Positionen über die bestehenden `Bestellung`-Methoden hinzugefügt.

**Technik:** .NET 11, C#, Spectre.Console 0.57.2.

**Spezifikation:** `docs/superpowers/specs/2026-11-05-cafe-warenkorb-prompt-design.md`

## Grenzen

- Keine Änderung am Core-Domänenmodell und an Preis-/Rabattregeln.
- Abbruch oder leere Auswahl ändert keine Bestellung.
- Fehlerhafte Mengen werden an der aktuellen Eingabe wiederholt.
- Umgeleitete Ein-/Ausgabe behält den bestehenden nummerierten Eingabepfad.

## Schritte

1. `CafeKonsolenMenue.ProduktHinzufuegen` auf mehrere ausgewählte Produkte erweitern: interaktiv `MultiSelectionPrompt`, im Script-Fallback nummerierte Mehrfachauswahl mit Abbruchoption.
2. Mengen je ausgewähltem Produkt lokal erfassen; bei ungültiger Eingabe im selben Prompt wiederholen, bei Abbruch ohne Bestelländerung zurückkehren.
3. Warenkorb-Vorschau als Spectre-Panel mit Produkt, Menge, Einzel- und Positionspreis sowie berechnetem Bestellgesamtpreis rendern; vor dem Hinzufügen bestätigen.
4. Ausgabe im interaktiven und umgeleiteten Pfad auf konsistente Titel, Hinweise und Rückmeldung abstimmen, ohne bestehende Menüoptionen zu verändern.
5. Änderung diff-reviewen und `dotnet build` für die Solution ausführen. Keine Tests ergänzen oder ausführen.

## Review-Fokus

- Keine Auswahl und Abbruch bewirken keinerlei Mutationen.
- Eine Menge kleiner/gleich null kann nicht in den Warenkorb gelangen.
- Ein Abbruch bei späterer Mengeneingabe hinterlässt keine zuvor eingegebenen Artikel in der Bestellung.
- Ablehnung der Bestätigung hinterlässt die Bestellung unverändert.
- Der Script-Fallback bleibt mit nummerischer Mehrfachauswahl nutzbar.
