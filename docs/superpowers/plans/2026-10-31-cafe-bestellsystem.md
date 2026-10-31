# Café-Bestellsystem Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build the complete Café-Bestellsystem exercise as a runnable console app with a separate, testable business library and xUnit coverage.

**Architecture:** Add a Core class library for product, order, and administration rules; a console app that handles only input/output; and an xUnit project referencing Core. Use ordinary C# classes, standard collections, and explicit date inputs, with no production dependencies or architecture framework.

**Tech Stack:** C#, .NET 11 (`net11.0`), .NET SDK, xUnit, Microsoft.NET.Test.Sdk.

**Spec:** `docs/superpowers/specs/2026-10-31-cafe-bestellsystem-design.md`

## Global Constraints

- Keep the existing `KlassenUebung` and `TemperaturMesswerte` projects unchanged.
- Target `net11.0` in all new projects.
- Keep business rules independent of `Console.ReadLine` and `Console.WriteLine`.
- Use `decimal` for prices; accept zero and reject negative product prices.
- Use ordinary classes, constructors, properties, lists, inheritance, and methods; avoid records and pattern-heavy scaffolding.
- Use only xUnit and the standard .NET test SDK as test dependencies; add no production package dependencies.
- Use exact business rules from the design: discount at subtotal >= 20.00, 10% off, round payable amount to 2 decimals with `MidpointRounding.AwayFromZero`; repeated products remain separate positions.
- Supply completion and revenue dates explicitly as `DateOnly`; do not read hidden current time in Core.
- Validate user input and constructor arguments; invalid product/quantity data throws argument exceptions, and invalid console input must allow the menu to continue.

## Review Focus

- A menu choice or numeric value that is blank, nonnumeric, or outside the available range must produce a message and return to a usable menu; cover this in Task 4's console input tests.
- A cancelled/closed order must not accept a new position; cover each terminal state in Task 2's status tests.
- A completed order on a different date must not affect today's revenue; cover this in Task 3's date tests.
- Discount arithmetic near a half-cent must use the specified rounding mode; cover this in Task 2's decimal rounding test.
- Looking up or acting on an unknown order number must not crash the menu; cover this in Task 4's menu behavior test.

---

### Task 1: Scaffold projects and product hierarchy

**Files:**
- Create: `CafeBestellsystem.slnx`
- Create: `CafeBestellsystem.Core/CafeBestellsystem.Core.csproj`
- Create: `CafeBestellsystem/CafeBestellsystem.csproj`
- Create: `CafeBestellsystem.Tests/CafeBestellsystem.Tests.csproj`
- Create: `CafeBestellsystem.Core/Produkte/Produkt.cs`
- Create: `CafeBestellsystem.Core/Produkte/Getraenk.cs`
- Create: `CafeBestellsystem.Core/Produkte/Speise.cs`
- Create: `CafeBestellsystem.Core/Produkte/Kaffee.cs`
- Test: `CafeBestellsystem.Tests/ProdukteTests.cs`

**Interfaces:**
- Produces abstract `Produkt(string name, decimal preis)` with read-only `Name` and `Preis`, and abstract `string Beschreibung()`.
- Produces `Getraenk(string name, decimal preis, bool istHeiss, int groesseInMl)`, `Speise(string name, decimal preis, bool istVegetarisch)`, and `Kaffee(string name, decimal preis, bool istHeiss, int groesseInMl, string bohnenSorte)`, each implementing `Beschreibung()`.
- All projects enable implicit usings and nullable reference types. Core targets `net11.0`; Console references Core; Tests references Core and uses xUnit/Test SDK.

- [x] **Step 1: Scaffold the three projects and add product tests before product implementation.**

Create project/solution files and the test class. Tests must cover valid product properties and descriptions; empty names, negative prices, nonpositive drink size, and blank bean variety; and polymorphic descriptions from a `List<Produkt>`.

- [x] **Step 2: Run the product tests and verify the expected compile failure.**

Run: `dotnet test CafeBestellsystem.Tests/CafeBestellsystem.Tests.csproj --filter FullyQualifiedName~ProdukteTests`
Expected: FAIL because the product types do not exist yet.

- [x] **Step 3: Implement product classes with the declared constructors and validation.**

Trim names and bean varieties before storing. Throw `ArgumentException` for blank text and `ArgumentOutOfRangeException` for a negative price or nonpositive size. Descriptions include each type's meaningful distinguishing properties.

- [x] **Step 4: Run the product tests.**

Run: `dotnet test CafeBestellsystem.Tests/CafeBestellsystem.Tests.csproj --filter FullyQualifiedName~ProdukteTests`
Expected: PASS.

### Task 2: Order positions, totals, discounts, and status

**Files:**
- Create: `CafeBestellsystem.Core/Bestellungen/Bestellposition.cs`
- Create: `CafeBestellsystem.Core/Bestellungen/Bestellung.cs`
- Test: `CafeBestellsystem.Tests/BestellpositionTests.cs`
- Test: `CafeBestellsystem.Tests/BestellungTests.cs`

**Interfaces:**
- Consumes: `Produkt` and its immutable `Preis` property.
- Produces `Bestellposition(Produkt produkt, int menge)` with `Produkt`, `Menge`, and calculated `decimal Positionspreis`.
- Produces `Bestellung(int nummer)` with `Nummer`, `IReadOnlyList<Bestellposition> Positionen`, `Bestellstatus Status`, nullable `DateOnly? Abschlussdatum`, `bool PositionHinzufuegen(Produkt produkt, int menge)`, `decimal WarenwertBerechnen()`, `decimal GesamtpreisBerechnen()`, `int AnzahlPositionen()`, `bool Abschliessen(DateOnly datum)`, and `bool Stornieren()`.
- Produces enum `Bestellstatus` values `Offen`, `Abgeschlossen`, and `Storniert`.

- [x] **Step 1: Write failing tests for position construction and price calculation.**

Include quantity one, a larger quantity, null product, and zero/negative quantity. Assert money using exact `decimal` values.

- [x] **Step 2: Run `BestellpositionTests` and verify they fail because the type is absent.**

Run: `dotnet test CafeBestellsystem.Tests/CafeBestellsystem.Tests.csproj --filter FullyQualifiedName~BestellpositionTests`
Expected: FAIL for missing type.

- [x] **Step 3: Implement `Bestellposition` with constructor validation and calculated price.**

- [x] **Step 4: Run `BestellpositionTests` and verify they pass.**

- [x] **Step 5: Write failing order tests for empty and multi-position totals, separate repeated-product positions, the 20-euro discount boundary, and cent rounding.**

Assert that an empty order totals zero; adding 2 x 5.00 and 1 x 10.00 yields three positions and a 20.00 subtotal; repeated additions remain separate; 19.99 receives no discount; 20.00 receives 10%; and a 20.05 subtotal produces 18.05 after half-cent rounding away from zero.

- [x] **Step 6: Write failing order-status tests.**

Assert that a new order is open, closing stores the supplied date, a closed order cannot close again or accept positions, a cancelled order cannot close or accept positions, and a closed order cannot be cancelled.

- [x] **Step 7: Run `BestellungTests` and verify failures come from missing `Bestellung`/`Bestellstatus` behavior.**

Run: `dotnet test CafeBestellsystem.Tests/CafeBestellsystem.Tests.csproj --filter FullyQualifiedName~BestellungTests`
Expected: FAIL for missing type.

- [x] **Step 8: Implement `Bestellung` and `Bestellstatus` using the declared signatures and rules.**

Only an open order accepts positions. `PositionHinzufuegen` returns false after closure/cancellation; otherwise construct a validated position and append it. `GesamtpreisBerechnen` returns the subtotal below 20.00, otherwise rounds `Warenwert * 0.90m` to 2 places using `MidpointRounding.AwayFromZero`.

- [x] **Step 9: Run all Core unit tests.**

Run: `dotnet test CafeBestellsystem.Tests/CafeBestellsystem.Tests.csproj`
Expected: PASS.

### Task 3: Order administration and workflow integration tests

**Files:**
- Create: `CafeBestellsystem.Core/Bestellungen/Bestellverwaltung.cs`
- Test: `CafeBestellsystem.Tests/BestellverwaltungTests.cs`
- Test: `CafeBestellsystem.Tests/BestellablaufTests.cs`

**Interfaces:**
- Consumes: `Bestellung`, its status, number, payable total, and completion date.
- Produces `Bestellverwaltung.BestellungErstellen()`, `Bestellung? BestellungSuchen(int nummer)`, `IReadOnlyList<Bestellung> OffeneBestellungenAnzeigen()`, `IReadOnlyList<Bestellung> AbgeschlosseneBestellungenAnzeigen()`, `IReadOnlyList<Bestellung> NachGesamtpreisSortieren()`, and `decimal TagesumsatzBerechnen(DateOnly datum)`.
- Numbering starts at 1 and increases by one for each new order. Sorting is ascending by payable total.

- [x] **Step 1: Write failing tests for sequential numbers, known/unknown lookup, empty collections, status filters, ascending sort, and daily revenue by the explicitly supplied date.**

Use multiple orders with different totals, statuses, and completion dates. Assert unknown lookup returns null and daily revenue sums only completed orders whose completion date equals the argument.

- [x] **Step 2: Run `BestellverwaltungTests` and verify the expected missing-type failure.**

Run: `dotnet test CafeBestellsystem.Tests/CafeBestellsystem.Tests.csproj --filter FullyQualifiedName~BestellverwaltungTests`
Expected: FAIL for missing `Bestellverwaltung`.

- [x] **Step 3: Implement `Bestellverwaltung` with an in-memory `List<Bestellung>`.**

- [x] **Step 4: Run `BestellverwaltungTests` and verify they pass.**

- [x] **Step 5: Write workflow integration tests for creating an order, adding products, calculating discount, closing it, and getting daily revenue; also assert a closed order rejects further changes.**

- [x] **Step 6: Run all tests and verify unit and integration tests pass.**

Run: `dotnet test CafeBestellsystem.Tests/CafeBestellsystem.Tests.csproj`
Expected: PASS with nonzero test count.

### Task 4: Console menu, documentation, and end-to-end verification

**Files:**
- Create: `CafeBestellsystem/Program.cs`
- Create: `CafeBestellsystem/Produktausgabe.cs` only if it keeps `Program.cs` clearer
- Create: `CafeBestellsystem.Tests/KonsolenmenueTests.cs` if input/output is extracted into a small console-only class with injected `TextReader` and `TextWriter`
- Modify: `CafeBestellsystem.slnx`
- Create: `CafeBestellsystem/README.md`

**Interfaces:**
- Consumes: the Core constructors and `Bestellverwaltung` public methods from Tasks 1–3.
- Produces menu actions: show products, start order, add product to order, show total, complete order, show daily revenue, and exit. The Tests project references both Core and Console so it can exercise menu input/output with readers and writers.
- Produces `CafeKonsolenMenue(TextReader input, TextWriter output, Bestellverwaltung verwaltung, IReadOnlyList<Produkt> produkte)` and `void Starten()` so the menu flow can be tested with `StringReader` and `StringWriter` without an interactive terminal.

- [x] **Step 1: Write failing tests for `CafeKonsolenMenue` parsing and input behavior using `StringReader` and `StringWriter`.**

Cover invalid menu input, unknown order number, and exiting cleanly. Keep UI tests limited to input/output flow; business results remain covered in Core tests.

- [x] **Step 2: Implement `CafeKonsolenMenue` and the `Program` entry point in the Console project.**

Use a short preloaded product list, parse user input with `int.TryParse`, show friendly error messages, and loop until exit. Supply `DateOnly.FromDateTime(DateTime.Today)` only at the console boundary for close and daily-revenue actions.

- [x] **Step 3: Add README instructions.**

Document `dotnet test CafeBestellsystem.slnx` and `dotnet run --project CafeBestellsystem/CafeBestellsystem.csproj`.

- [x] **Step 4: Run the full solution test suite.**

Run: `dotnet test CafeBestellsystem.slnx`
Expected: PASS with a nonzero test count.

- [x] **Step 5: Build and run the console flow with scripted input.**

Run the console app using a scripted menu sequence that lists products, creates an order, adds products, displays total, completes the order, displays daily revenue, and exits. Confirm the full sequence terminates and prints the expected business totals.

- [x] **Step 6: Confirm the original projects and repository files are unchanged outside this feature.**

Run: `git status --short` and inspect the diff. Expected: only the design/plan docs and new Café project files are added; existing exercise code is unchanged.

## Execution notes

- **Ruling:** The Tests project references both Core and Console so `CafeKonsolenMenue` can be tested with `StringReader` and `StringWriter` while Core remains free of console I/O. The initial project-reference list named only Core, which made the planned menu tests uncompilable. The cost if this ruling is wrong is a tighter test-to-console-project dependency; there are still no runtime package dependencies.
- The full solution test run passed 35 tests. A scripted menu run printed the expected total (`7,00 €`), daily revenue (`7,00 €`), and exit message.
- The active SDK was .NET 11.0.100-rc.3; the SDK reports preview support status.
