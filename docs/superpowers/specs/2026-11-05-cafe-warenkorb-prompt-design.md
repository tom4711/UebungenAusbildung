# Interaktiver Café-Warenkorb

## Ziel

Der Ablauf „Produkt hinzufügen“ soll mehrere Katalogprodukte in einem Rutsch zu einer offenen Bestellung hinzufügen. Er soll sich wie ein geführter Bestellassistent anfühlen und Auswahl, Mengen, Summe und Bestätigung gut lesbar mit Spectre.Console darstellen.

## Ansatz und Alternativen

- **Gewählt: Sammelauswahl mit Vorschau.** `MultiSelectionPrompt` wählt mehrere vorhandene Produkte. Für jedes gewählte Produkt wird lokal die Menge abgefragt. Erst nach vollständiger Eingabe wird eine formatierte Warenkorb-Vorschau mit Gesamtsumme angezeigt und bestätigt; danach werden alle Positionen der offenen Bestellung hinzugefügt. Das bildet das Pizza-Beispiel ab, ohne Produkte oder Beläge im Fachmodell zu erfinden.
- **Nur visuelle Auffrischung des Einzelauswahl-Ablaufs.** Wäre kleiner, bietet aber keine Sammelauswahl wie im Beispiel.
- **Beläge/Konfiguration als neue Fachlogik.** Passt nicht zum bestehenden Café-Katalog, der fertige Produkte kennt.

## Grenzen und Verhalten

- Änderungen betreffen den Konsolenablauf, keine Core-Geschäftsregeln oder Produktmodelle.
- Auswahl darf leer bleiben; Abbrechen oder keine Auswahl ändert die Bestellung nicht.
- Ungültige Mengen werden direkt erneut abgefragt. Erst nach erfolgreicher Bestätigung werden die Positionen hinzugefügt, um Teiländerungen bei Abbruch zu vermeiden.
- Die Vorschau zeigt Produkt, Menge, Einzelpreis und Positionspreis; die Bestellsumme wird über die bestehende Geschäftslogik berechnet.
- Konsolenausgabe bleibt über die injizierte Spectre-Konsole, damit bestehender I/O-Aufbau und Script-Fallback erhalten bleiben.
- Zusätzlich werden passende Titel, Status-/Hinweistexte und Panels für den geführten Ablauf verwendet.
