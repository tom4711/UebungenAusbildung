# Café-Konsolen-UI Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Make the existing Café console clearer with Spectre.Console, screen refreshes, and early feedback for closed orders.

**Architecture:** Keep menu control and business rules where they are. Use Spectre.Console for terminal rendering while preserving injected readers/writers for automated tests; handle closed order detection in the menu before asking for a product.

**Tech Stack:** .NET 11, C#, Spectre.Console NuGet, xUnit.

**Spec:** `docs/superpowers/specs/2026-11-05-cafe-konsolen-ui-design.md`

## Global Constraints

- Retain existing menu options and business rules.
- Keep automated console tests possible with `StringReader` and `StringWriter`.
- Do not add order-selection submenus or alter Core behavior.

## Review Focus

- Closed and cancelled orders: reject before the product list or any product prompt.
- Missing order numbers: keep the existing not-found response and return to the menu.
- Invalid and exhausted input: preserve clean termination and validation behavior.
- Non-interactive output: do not require a real terminal to run tests.
- Terminal rendering: escape user/product text through Spectre.Console formatting APIs where applicable.

### Task 1: Upgrade console rendering and validate order status early

**Files:**
- Modify: `CafeBestellsystem/CafeBestellsystem.csproj`
- Modify: `CafeBestellsystem/CafeKonsolenMenue.cs`
- Test: `CafeBestellsystem.Tests/KonsolenmenueTests.cs`

**Interfaces:**
- Retain the existing `CafeKonsolenMenue(TextReader, TextWriter, Bestellverwaltung, IReadOnlyList<Produkt>)` constructor and `Starten()` entry point.
- Use Spectre.Console rendering with the injected writers; only clear when output is interactive.

- [x] Add a test that closes an order, then tries to add a product and asserts the closed-order notice appears before any product prompt/list.
- [x] Run the focused test and verify it fails because the status is currently checked only after product and quantity entry.
- [x] Add Spectre.Console package reference to the Console project and adapt menu/product/status output to colored markup and readable tables while retaining input parsing.
- [x] Clear the screen before each fresh menu when the output is interactive; preserve captured output behavior for StringWriter.
- [x] Check order status immediately after lookup in `ProduktHinzufuegen`; report closed/cancelled state and return before collecting product input.
- [x] Run `dotnet test CafeBestellsystem.Tests/CafeBestellsystem.Tests.csproj` and verify all tests pass (36 tests).
- [x] Run `dotnet build CafeBestellsystem.slnx` and inspect `git diff`; the pre-existing `TemperaturMesswerte/Program.cs` change is untouched.
