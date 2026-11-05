# Bestellabschluss und Eingabewiederholung Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans (inline) to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Show open orders before closing one and keep invalid input in its current prompt, with `0` cancelling the active action.

**Architecture:** Keep the change in `CafeKonsolenMenue`. Reuse the open-order listing and order-content display; add focused prompt loops for menu selection, order selection, product number, and quantity.

**Tech Stack:** .NET 11, C#, Spectre.Console, xUnit.

**Spec:** `docs/superpowers/specs/2026-11-05-bestellung-abschliessen-eingaben-design.md`

## Global Constraints

- Core order and pricing rules remain unchanged.
- `0` cancels the current action and returns to the main menu.
- Failed validation retries only the current input field.

## Review Focus

- Closing lists only open orders and accepts only one of those numbers.
- No open orders: explain and return without asking for a number.
- Invalid text, out-of-range product number, and nonpositive quantity: keep the current prompt active.
- `0` at each action prompt cancels without changing the order.
- Menu selection errors repeat the selection prompt without losing the menu context.

### Task 1: Guided close flow and retryable prompts

**Files:**
- Modify: `CafeBestellsystem/CafeKonsolenMenue.cs`
- Test: `CafeBestellsystem.Tests/KonsolenmenueTests.cs`

**Interfaces:**
- Keep the existing public constructor and `Starten()` API.
- Reuse `Bestellverwaltung.OffeneBestellungenAnzeigen()` and existing Spectre.Console order tables.

- [x] Add tests for closing via the open-order list, no-open-orders behavior, and local retry after an invalid input.
- [x] Run the new tests and confirm they fail on the current one-shot prompts.
- [x] Change menu, order, product, and quantity input to retry the same prompt after invalid input; `0` cancels the current action.
- [x] Show open orders and the selected order's current contents before completing it.
- [x] Run all 47 tests, build the solution, and inspect the diff.
