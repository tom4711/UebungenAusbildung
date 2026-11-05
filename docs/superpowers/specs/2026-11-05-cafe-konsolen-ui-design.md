# Café-Konsolen-UI: gezielte Auffrischung

## Absicht

Die Konsolenbedienung soll besser lesbar und vorhersehbar werden. Spectre.Console ergänzt strukturierte, farbige Anzeigen; zwischen Menüschritten wird die Anzeige aufgeräumt. Bei einer Bestellung, die nicht mehr offen ist, erscheint der Hinweis direkt nach Eingabe der Bestellnummer.

## Umfang

- Spectre.Console wird als NuGet-Abhängigkeit im Konsolenprojekt verwendet.
- Menüs, Produktlisten und relevante Status-/Fehlermeldungen werden mit passenden Farben und Tabellen/Markup formatiert.
- Bildschirmbereinigung erfolgt vor einer neuen Menüanzeige, soweit die Ausgabeschnittstelle dies unterstützt; Tests mit StringWriter bleiben möglich.
- Die Bestellnummer wird unmittelbar auf Existenz und Offen-Status geprüft, bevor Produkt- oder weitere Eingaben abgefragt werden.
- Bestehende Fachregeln und Menüoptionen bleiben erhalten.

## Nicht im Umfang

Kein neues Untermenü, keine Auswahl offener Bestellungen, keine Änderungen an Fachlogik oder Bestellregeln.

## Ansätze

- **Gezielte Auffrischung (gewählt):** Spectre.Console für Ausgabeformatierung, bestehender Ablauf bleibt bestehen. Geringes Änderungsrisiko und deckt die gemeldeten Probleme direkt ab.
- **Größerer Umbau:** Untermenüs und geführte Bestellauswahl. Größere Bedienänderung, für den ersten Schritt nicht nötig.

## Umsetzungsspielraum

Die konkrete Spectre.Console-Ausgabeform und der Umgang mit nicht interaktiven Ausgabestreams bleiben Implementierungsentscheidungen, solange vorhandene Tests automatisierbar bleiben.
