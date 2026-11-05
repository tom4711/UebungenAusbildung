# Offene Bestellungen: geführter Ablauf

## Absicht

Die Bedienperson soll offene Café-Bestellungen samt bereits erfassten Produkten überblicken und eine bestehende Bestellung leichter bearbeiten können, ohne sich die Bestellnummer merken zu müssen.

## Umfang

- Neuer Menüpunkt „Offene Bestellungen anzeigen“.
- Die Übersicht zeigt Bestellnummer, Positionen mit Produktbeschreibung und Menge sowie den aktuellen Gesamtpreis; leere Bestellungen werden als solche gekennzeichnet.
- „Produkt hinzufügen“ zeigt offene Bestellungen zur Auswahl. Nach Auswahl werden die vorhandenen Positionen und der aktuelle Gesamtpreis gezeigt, bevor ein weiteres Produkt und seine Menge abgefragt werden.
- Bei keiner offenen Bestellung erscheint eine verständliche Meldung und es werden keine Produktdaten abgefragt.
- Nicht-offene und unbekannte Bestellungen können in diesem Auswahlfluss nicht gewählt werden.

## Nicht im Umfang

Keine Änderungen an Core-Fachregeln, Bestellnummern oder Preisberechnung. Keine Bearbeitung/Entfernung vorhandener Positionen und kein Umbau in ein interaktives Listen-Widget.

## Ansätze

- **Separate Übersicht:** Nur ein neuer Anzeigepunkt; die Bestellnummer muss beim Hinzufügen weiter manuell eingegeben werden. Klein, löst die Orientierung beim Bearbeiten aber nur teilweise.
- **Geführter Auswahlfluss (gewählt):** Übersicht und Produkt-Hinzufügen greifen auf offene Bestellungen zu; die Auswahl erfolgt über die angezeigte Bestellnummer, danach wird der aktuelle Inhalt gezeigt. Beseitigt das Merken der Nummer und vermeidet Änderungen an der Fachlogik.

## Umsetzungsspielraum

Die vorhandenen Konsolenkomponenten und Spectre.Console-Tabellen bleiben bestehen. Bestehende Optionen werden beibehalten; der neue Menüpunkt erhält eine freie Ziffer, die Menüvalidierung wird angepasst.
