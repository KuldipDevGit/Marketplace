# ADR-0004: Per-Service Internal Architecture — Clean Architecture

- **Status:** Accepted
- **Date:** 2026-06-12
- **Deciders:** Product owner, Lead Architect

## Context
Each microservice (ADR-0003) needs a maintainable internal structure that isolates business logic from infrastructure concerns (database, framework, transport), keeps the domain testable, and makes boundaries enforceable.

## Options considered
1. **Clean / Onion Architecture** — concentric layers, dependencies point inward; highly testable; some project ceremony.
2. **Anemic layered / transaction script** — fast to start, but logic leaks into services and erodes over time.
3. **Vertical Slice Architecture** — feature-cohesive, less ceremony, but weaker global structural guarantees across a large domain.

## Decision
Adopt **Clean Architecture per service**: **Domain** (entities, value objects, domain events — no dependencies) → **Application** (CQRS use cases, ports, validators) → **Infrastructure** (EF Core, adapters) → **Api** (controllers). Dependencies point inward only. Vertical-slice **feature folders** are permitted *within* the Application layer for cohesion.

## Consequences
- (+) Testable domain (zero-mock unit tests), swappable infrastructure, and boundaries enforceable by NetArchTest (BE-16, TEST-8).
- (−) More projects and mapping ceremony per service. Mitigated by a service template generated in Phase 1.
