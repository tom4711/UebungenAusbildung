# Einheitliche Spectre-Auswahlprompts

## Absicht

Die Konsolenbedienung soll Spectre.Console für feste Auswahlmenüs nutzen, damit verfügbare Aktionen klar sichtbar und per Tastatur bequem wählbar sind.

## Umfang

- Das Hauptmenü nutzt einen `SelectionPrompt` mit aussagekräftigem Titel und allen vorhandenen Aktionen.
- Auswahl offener Bestellungen und Produktauswahl nutzen ebenfalls `SelectionPrompt` mit Details wie Bestellnummer, Produkten, Gesamtpreis bzw. Produktpreis.
- Zurück/Abbrechen erscheint als sichtbare Auswahl in jedem Unterablauf; Auswahlfragen werden mit passenden Titeln versehen.
- Freie numerische Eingaben, insbesondere Mengen, bleiben Texteingaben mit Validierung und Wiederholung am selben Prompt.
- Spectre-Prompts laufen über die injizierte Konsole, damit Ausgabe und Eingabe weiterhin testbar bleiben.
- Bestehende Geschäftsregeln und Möglichkeiten bleiben erhalten.

## Nicht im Umfang

Keine Änderungen an Core, Bestellregeln, Preisberechnung oder Umsatzdiagramm.

## Ansätze

- **Einheitliche Auswahlprompts (Arbeitsannahme):** Alle festen Auswahlfelder werden mit Pfeiltasten auswählbar und zeigen Zurück/Abbrechen sichtbar an. Eine Bedienlogik, bessere Führung.
- **Nur interaktive Auswahlprompts:** Pfeiltasten im echten Terminal, Zahlenfallback für umgeleitete Ein-/Ausgaben. Erhält Skriptkompatibilität, führt aber zwei Bedienpfade ein.

## Umsetzungsspielraum

Die vorhandene `IAnsiConsole` wird mit Eingabe und Ausgabe konfiguriert. Tests sollen die gleichen Prompt-Entscheidungen über Spectre.Console-Testeingabe abbilden; kein direkter Aufruf globaler `AnsiConsole`-Prompts.
