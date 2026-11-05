# Tagesumsatz Produktdiagramm Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Show today's completed revenue by product in a table and proportional Spectre.Console chart, with product shares matching the actual total after order discounts.

**Architecture:** Add product revenue aggregation to `Bestellverwaltung`, where today's completed orders and their discount-aware totals are already available. Render those aggregated values in the existing console menu with a Spectre `BreakdownChart` and a table.

**Tech Stack:** .NET 11, C#, Spectre.Console 0.57.2, xUnit.

**Spec:** `docs/superpowers/specs/2026-11-05-tagesumsatz-produktdiagramm-design.md`

## Global Constraints

- Only completed orders whose completion date matches the requested date contribute.
- Product revenue totals must equal actual order totals, including the existing order discount.
- Use the same aggregated amounts for the table and chart.

## Review Focus

- No completed sales: report zero revenue without attempting to draw an empty chart.
- Multiple same-product positions/orders: aggregate by product description.
- Discounted multi-product order: prorate the discount and reconcile cents deterministically.
- Open, cancelled, and other-day orders: exclude them.
- One product only: render a valid single-segment breakdown and 100% share.

### Task 1: Aggregate daily product revenue and display its composition

**Files:**
- Modify: `CafeBestellsystem.Core/Bestellungen/Bestellverwaltung.cs`
- Test: `CafeBestellsystem.Tests/BestellverwaltungTests.cs`
- Modify: `CafeBestellsystem/CafeKonsolenMenue.cs`
- Test: `CafeBestellsystem.Tests/KonsolenmenueTests.cs`

**Interfaces:**
- Add `IReadOnlyDictionary<string, decimal> ProduktumsatzBerechnen(DateOnly datum)` to `Bestellverwaltung`.
- Return rounded product revenue grouped by `Produkt.Beschreibung()`; values sum exactly to `TagesumsatzBerechnen(datum)`.

- [x] Test that today's completed sales group by product, allocate the order discount, and reconcile the final cent so product revenue equals daily total.
- [x] Test that other dates and open/cancelled orders are excluded, and empty data returns an empty result.
- [x] Implement deterministic per-order proportional allocation: round each product share to cents, then apply any residual to the largest product amount with ordinal description as tie-break.
- [x] Run focused Core tests.
- [x] Add `BreakdownChart` and a product/value/percentage table to the daily revenue action; show a no-sales message for zero totals.
- [x] Test the daily revenue output for a nonempty breakdown and a no-sales case.
- [x] Run the full test suite and solution build; inspect the diff.
