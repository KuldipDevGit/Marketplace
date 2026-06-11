# ADR-0010: Repository Strategy — Monorepo

- **Status:** Accepted
- **Date:** 2026-06-12
- **Deciders:** Product owner, Lead Architect

## Context
The platform comprises many services, three Angular apps, and shared contract/kernel libraries, built by a small team. The repository layout affects how cross-cutting changes (especially shared event contracts) are made and released.

## Options considered
1. **Monorepo** — all services, gateways, shared libraries, and Angular apps in one Git repository.
2. **Polyrepo** — one repository per service; strong isolation and independent release cadence, but cross-cutting changes require coordinated multi-repo PRs.

## Decision
Adopt a single **monorepo** with **per-service CI pipelines** triggered by path filters (INF-11). Shared `Contracts` and `SharedKernel` libraries are referenced directly within the repo.

## Consequences
- (+) Atomic cross-service and contract changes, one shared build configuration, and simple dependency management — ideal for a small team.
- (−) A larger repository; CI must use path-based triggers to avoid rebuilding everything; access control is repository-wide (coarse-grained).
