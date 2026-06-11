# AI Agent Rules

> **Status:** Active · **Version:** 1.0 · **Last updated:** 2026-06-12
> This file governs how the AI agent operates on this repository. It takes precedence over convenience. Changes require an ADR.

## Scope
Every interaction in which the agent analyzes, designs, or modifies this repository.

## Rules

- **AI-1 — Output Format.** Every substantive task is answered with: *Understanding · Questions · Proposed Solution · Impacted Files · Risks · Approval Required.* No section is skipped.
- **AI-2 — Approval gates.** Follow the 10-step workflow (Requirement Analysis → Clarifying Questions → Architecture → Folder Structure → Data Model → API Contract → Approval → Code → Tests → Documentation). **Never write production code before architecture and API contracts are approved.**
- **AI-3 — Reasoning is explicit.** For every major decision, state reasoning, trade-offs, risks, and at least one alternative.
- **AI-4 — ADR per decision.** Any decision affecting architecture, data model, security, or infrastructure is captured as an ADR in `docs/adrs/` before or alongside implementation.
- **AI-5 — STOP on conflict.** If an instruction conflicts with any rule file, halt, explain the conflict, and request clarification. Do not silently resolve it.
- **AI-6 — No destructive actions without approval.** Deleting data, dropping tables/columns, force-pushing, rewriting history, or removing files the agent did not create require explicit approval.
- **AI-7 — Production-only code.** No TODO/FIXME placeholders, mock or sample data, commented-out code, temporary shims, or hardcoded secrets/configuration in committed code.
- **AI-8 — Read before you write.** Read the relevant rule file(s) before generating code in a layer; cite the rule IDs that drive a change.
- **AI-9 — Reproducibility.** Pin dependency versions (`Directory.Packages.props`); never rely on machine-specific or ambient state.
- **AI-10 — Small, reviewable changes.** One bounded concern per change; keep diffs focused and explainable.
- **AI-11 — Security & privacy first.** Never log secrets or PII; never weaken authentication, authorization, or validation for convenience.
- **AI-12 — Faithful reporting.** Report outcomes honestly — failing tests, skipped steps, and assumptions are stated plainly.

## Enforcement
Self-enforced by the agent and verified by the human reviewer at each approval gate. Violations block the approval.
