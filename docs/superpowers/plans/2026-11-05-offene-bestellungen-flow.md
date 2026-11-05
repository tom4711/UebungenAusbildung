# Offene Bestellungen Flow Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Show open orders with their products and make product additions use a guided open-order selection.

**Architecture:** Keep order rules in Core and use `Bestellverwaltung.OffeneBestellungenAnzeigen()` as the source of open orders. Render the overview and the selected order's current positions with the existing Spectre.Console table conventions in `CafeKonsolenMenue`.

**Tech Stack:** .NET 11, C#, Spectre.Console, xUnit.

**Spec:** `docs/superpowers/specs/2026-11-05-offene-bestellungen-flow-design.md`

## Global Constraints

- Do not change Core business rules or price calculation.
- Retain existing menu actions and input/output injection.
- Use open orders only in the add-product selection flow.

## Review Focus

- No open orders: show a clear message and do not ask for a product.
- Empty open order: identify it as empty and show a zero total.
- Populated open order: list every position, quantity, and current total before product selection.
- Closed/cancelled/unknown order: never offer it as selectable.
- Invalid selection: report it and return to the menu without adding a position.

### Task 1: Add open-order overview and guided add-product selection

**Files:**
- Modify: `CafeBestellsystem/CafeKonsolenMenue.cs`
- Modify: `CafeBestellsystem.Tests/KonsolenmenueTests.cs`

**Interfaces:**
- Consume existing `Bestellverwaltung.OffeneBestellungenAnzeigen()` and order `Positionen`.
- Preserve the existing `CafeKonsolenMenue` constructor and input/output behavior.

- [x] Add menu action 7 for the open-order overview and accept selections 0–7.
- [x] Render each open order with its number, all product descriptions and quantities, and current total; make empty orders explicit.
- [x] Change product addition to show open orders, ask for an open order number, validate it against the open-order list, then show its current positions and total before asking for a new product.
- [x] Keep the existing product and quantity validation after order selection.
- [x] Run all 41 tests and inspect the diff; the pre-existing `TemperaturMesswerte/Program.cs` change is untouched.
