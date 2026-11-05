# Tagesumsatz nach Produkt: Diagramm

## Absicht

Die Tagesumsatzansicht soll neben dem Gesamtbetrag zeigen, welche Produkte zum heutigen Umsatz beigetragen haben.

## Umfang

- Nur abgeschlossene Bestellungen mit Abschlussdatum heute zählen.
- Umsatz wird nach Produktbeschreibung gruppiert und in einer Tabelle mit Betrag und prozentualem Anteil angezeigt.
- Ein Spectre.Console `BreakdownChart` zeigt die Produktanteile. Das ist eine segmentierte proportionale Leiste, kein Kreisdiagramm; Spectre.Console stellt kein natives Kreisdiagramm-Widget bereit.
- Bestellrabatte werden anteilig auf die Produkte einer Bestellung verteilt. Rundungsreste werden deterministisch so ausgeglichen, dass die Produktbeträge exakt dem Tagesumsatz entsprechen.
- Bei null Umsatz erscheint eine verständliche Meldung statt eines leeren Diagramms.

## Nicht im Umfang

Keine Änderungen an Rabattregeln oder gespeicherten Bestellungen; keine externe Chart-Bibliothek und keine selbstgezeichnete Kreisgrafik.

## Ansätze

- **BreakdownChart mit anteiliger Rabattverteilung (Arbeitsannahme):** Nutzt das eingebaute proportionale Spectre-Widget und stimmt mit dem Tagesumsatz überein.
- **Produktwerte vor Rabatt:** Einfach, aber Summe der Produktwerte kann den tatsächlich gezahlten Tagesumsatz übersteigen.
- **Eigenes Kreisdiagramm:** Erfordert benutzerdefinierte Terminalgrafik und ist mit der vorhandenen Spectre-Oberfläche deutlich aufwendiger.

## Umsetzungsspielraum

Die Aufteilung erfolgt auf Produktbeschreibung über jede abgeschlossene Bestellung des Tages. Die Summe wird centgenau auf den Bestellendbetrag abgeglichen; Tabelle und Diagramm nutzen dieselben Werte.
