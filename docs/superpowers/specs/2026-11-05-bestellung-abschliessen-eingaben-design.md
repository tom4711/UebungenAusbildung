# Bestellabschluss und Eingabewiederholung

## Absicht

Bestellungen sollen beim Abschließen leicht auffindbar sein, und Eingabefehler sollen innerhalb des aktuellen Schritts korrigiert werden können, ohne den Vorgang neu zu starten.

## Umfang

- „Bestellung abschließen“ zeigt zuerst offene Bestellungen mit Nummer, Inhalt und Gesamtpreis.
- Die Bedienperson wählt eine Bestellnummer aus dieser offenen Liste.
- Ungültige oder unbekannte Eingaben bei Bestellnummer, Produktnummer, Menge und Menüauswahl wiederholen genau das betroffene Eingabefeld.
- `0` bricht den aktuellen Vorgang ab und kehrt zum Hauptmenü zurück.
- Bei keiner offenen Bestellung erscheint eine Meldung ohne Bestellnummernabfrage.

## Nicht im Umfang

Keine Änderungen an Core-Bestellregeln, Preisberechnung, Auswahl abgeschlossener Bestellungen oder vorhandenen Produktdaten.

## Ansätze

- **Nur Abschlussauswahl führen:** Offene Bestellungen vor dem Abschluss anzeigen, andere Fehler weiter zum Hauptmenü zurückführen. Kleinere Änderung, lässt den Produkterfassungsablauf weiter unterbrechen.
- **Alle Eingaben im Vorgang wiederholen (gewählt):** Validierungsfehler wiederholen das konkrete Feld; `0` bietet einen einheitlichen Abbruch. Verhindert den Neustart eines bereits begonnenen Vorgangs.

## Umsetzungsspielraum

Die Wiederholung bleibt in der Konsolen-UI. Die vorhandene offene Bestellübersicht und Spectre.Console-Tabellen werden wiederverwendet; Core bleibt unverändert.
