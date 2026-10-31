# Café-Bestellsystem: Design

## Intent

Create a complete, runnable example for the attached Café-Bestellsystem exercise. It should demonstrate the requested C# fundamentals, xUnit-first development, simple separation of responsibilities, and a few useful SOLID ideas without teaching architecture that the exercise does not require.

## Goals

- Cover the exercise's product hierarchy, order positions, order totals and discounts, order status, order administration, console menu, and automated tests.
- Keep business rules independent of console input and output.
- Make the primary rules easy to discover in code and verify with xUnit.
- Include a short README with build, run, and test commands.

## Non-goals and anti-goals

- No database, persistence, network API, GUI, or bonus features.
- No dependency-injection container, repository pattern, event system, or extra abstraction added only to claim SOLID compliance.
- Do not alter the existing `KlassenUebung` or `TemperaturMesswerte` projects.
- Avoid advanced language features that obscure the exercise's classes, constructors, properties, lists, methods, inheritance, and tests.

## Approaches considered

1. Put business logic and console interaction in one executable project and add a test project. This is compact, but makes the boundary between user interaction and business rules less visible.
2. Use a small class-library project for business rules, a console project for input/output, and an xUnit project. This adds one project boundary but directly supports the exercise's requirement to keep console I/O out of the domain and makes that separation visible to learners.
3. Use a larger layered architecture with interfaces, repositories, dependency injection, and separate application/domain projects. This demonstrates more patterns but adds concepts the assignment does not require.

**Chosen approach:** option 2. It reflects the accepted direction: a small, clear separation with no architecture framework. The class library contains the product/order rules and `Bestellverwaltung`; the console project owns the menu; tests reference the business-logic library and exercise both individual rules and complete workflows.

## Proposed project structure

Add `CafeBestellsystem.slnx` with:

- `CafeBestellsystem.Core`: a class library for `Produkt`, `Getränk`, `Speise`, `Kaffee`, `Bestellposition`, `Bestellung`, and `Bestellverwaltung`.
- `CafeBestellsystem`: a console executable referencing Core. `Program.cs` handles prompts, parsing, display, and menu navigation only.
- `CafeBestellsystem.Tests`: an xUnit project referencing Core, with focused unit tests and a small number of workflow integration tests.

Keep the existing solution and exercises intact. All projects target the repository's current `net11.0` framework.

## Business rules

- Product names and coffee bean names must be non-empty after trimming. Prices must be zero or greater. Invalid constructor inputs throw `ArgumentException` (or `ArgumentOutOfRangeException` for numeric ranges).
- `Produkt` is abstract and exposes an abstract `Beschreibung()` method. `Getränk` validates a positive size in milliliters; `Speise` records whether it is vegetarian; `Kaffee` specializes `Getränk` with a required bean variety. A `List<Produkt>` demonstrates polymorphic descriptions.
- A `Bestellposition` requires a product and a positive quantity. Its price is the product price times quantity using `decimal`.
- A `Bestellung` accepts repeated products as separate positions. An empty order costs zero. Mutations are allowed only while open.
- Discount eligibility is based on the subtotal before discount: orders of at least 20.00 euros receive 10 percent off. The final payable amount is rounded to two decimal places using `MidpointRounding.AwayFromZero`.
- Order states are `Offen`, `Abgeschlossen`, and `Storniert`. A successful close stores the explicitly supplied `DateOnly`; a cancelled or closed order cannot be changed or closed again. Closing and cancelling return `bool` for clear, testable outcomes.
- `Bestellverwaltung` assigns increasing integer order numbers, finds an order by number, filters open or completed orders, sorts by payable total, and sums the payable totals of completed orders for an explicitly supplied `DateOnly`.
- There is no hidden dependency on the current clock. Console code supplies the date when closing an order and when asking for daily revenue.

## Console behavior

Provide the exercise's six menu actions plus exit: show products, start an order, add a product to an order, show the current total, complete an order, and show daily revenue. Use a small, preloaded list of example products so the learner can try the complete flow immediately. Keep console parsing and display in `Program.cs` or a small console-only helper; do not put `Console.ReadLine` or `Console.WriteLine` into Core. Invalid menu input should produce a readable message and allow another attempt rather than crash.

## Test approach

Use xUnit tests for product validation and polymorphism, position price and quantity validation, empty and multi-position totals, discount boundaries, duplicate positions, status transitions and immutability after close, administration searches/filters/sorting, and daily revenue. Add a few integration tests that exercise product-to-order-to-close-to-daily-revenue workflows. Tests assert observable business results rather than implementation details.

## Builder choices

- Use ordinary classes and explicit constructors/properties; avoid records, generics beyond standard collections, and design-pattern scaffolding.
- Keep class and test names in German to match the assignment.
- Use no external production dependencies. xUnit and the standard .NET test SDK are the only test dependencies.
- If .NET 11 is still preview in the active environment, build and test with the installed SDK and report that toolchain limitation clearly.

## Remaining decisions for the builder

Use straightforward file grouping by concept. Keep menu navigation readable and small. Choose sensible output wording and whether the current order number is printed before or after an action; neither choice changes the business rules.
