# ADR-0001: Record Architecture Decisions

- **Status:** Accepted
- **Date:** 2026-06-12
- **Deciders:** Product owner, Lead Architect

## Context
A full-production, multi-vendor marketplace built as microservices involves many consequential decisions, made incrementally over time. We need a durable, reviewable record of *why* each decision was made, so future contributors (human or AI) neither relitigate settled choices nor silently violate them. This is required by the project governance framework.

## Options considered
1. **Rely on chat / commit history** — zero overhead, but rationale is scattered and unsearchable; decisions erode silently.
2. **One monolithic `ARCHITECTURE.md`** — centralized, but becomes a large, frequently-merged file with no decision-level history.
3. **Lightweight ADRs (Nygard format), one file per decision** — small per-decision overhead; durable, granular, reviewable history.

## Decision
Adopt **Architecture Decision Records** in `docs/adrs/`, using the Nygard format (Context / Options / Decision / Consequences), sequentially numbered, and **immutable once Accepted** — superseded by a new ADR rather than edited in place.

## Consequences
- (+) Durable, granular rationale that supports the governance approval gates.
- (+) The AI agent must file an ADR for every major decision (`ai-agent.rules.md`, AI-4).
- (−) Minor overhead per decision; requires discipline to keep current. Mitigated by a copy-paste template.
