# Spectre Auswahlprompts Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans (inline) to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Use titled Spectre selection prompts for fixed choices and retain validated text prompts for free numeric input.

**Architecture:** Configure the existing `IAnsiConsole` with injected input and output. Move main menu, open-order choice, and product choice to `SelectionPrompt<T>`; use explicit back/cancel choices in workflows. Keep order/quantity parsing in validated text entry.

**Tech Stack:** .NET 11, C#, Spectre.Console 0.57.2, Spectre.Console.Testing 0.57.2, xUnit.

**Spec:** `docs/superpowers/specs/2026-11-05-spectre-auswahlprompts-design.md`

## Global Constraints

- Keep the existing business rules and all current capabilities.
- Keep `CafeKonsolenMenue` using injected input/output; do not call static global prompts.
- Fixed choice lists use a titled `SelectionPrompt<T>`; free numbers remain validated text input.
- Preserve back/cancel behavior for each action.

## Review Focus

- Selection prompts consume only the injected input and render only to the injected output.
- Main menu exit remains available and returns cleanly.
- Empty open-order list reports the existing message without attempting a selection.
- Back/cancel at order and product selection makes no order changes.
- Invalid quantity repeats the same text prompt; zero cancels the workflow.

### Task 1: Replace numbered choice lists with Spectre prompts

**Files:**
- Modify: `CafeBestellsystem/CafeKonsolenMenue.cs`
- Modify: `CafeBestellsystem/CafeBestellsystem.csproj`
- Modify: `CafeBestellsystem.Tests/CafeBestellsystem.Tests.csproj`
- Test: `CafeBestellsystem.Tests/KonsolenmenueTests.cs`

**Interfaces:**
- Preserve the existing menu constructor and `Starten()`.
- Use `SelectionPrompt<MenueAktion>`, `SelectionPrompt<Bestellung>` and `SelectionPrompt<Produkt>` or equivalent typed choices with display converters.
- Add `Spectre.Console.Testing` to test keyboard input and recorded output.

- [x] Add tests for arrow-key menu selection, order and product choice, cancel/back actions, and quantity retry.
- [x] Run those tests to confirm the current number-based interaction fails the new expectations.
- [x] Configure Spectre input from the injected console and route selection prompts through `_konsole`.
- [x] Convert the main menu, open-order choices, and product choices to titled Spectre prompts with styled highlights and descriptive labels.
- [x] Add visible back/exit choices while preserving all current menu actions; retain validated quantity input.
- [x] Run all tests, build the solution, and inspect the final diff.

## Implementation note

Ruling: In interactive terminals, all fixed choices use Spectre `SelectionPrompt` with titled choices. For redirected/scripted I/O, the existing numbered fallback remains available because arrow-key prompts require an interactive terminal and would break scripted input. This preserves test automation and command-line scripts; if this call is wrong, redirected users will continue to see the numbered interface.
