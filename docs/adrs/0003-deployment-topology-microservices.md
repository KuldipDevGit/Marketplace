# ADR-0003: Deployment Topology — Microservices

- **Status:** Accepted
- **Date:** 2026-06-12
- **Deciders:** Product owner, Lead Architect

## Context
The marketplace comprises distinct bounded contexts (catalog, ordering, payments, sellers, inventory, …) with a full-production intent and goals of independent scaling and fault isolation. Note that internal layering (Clean Architecture, ADR-0004) is an **orthogonal** concern to deployment topology — both axes are decided independently.

## Options considered
1. **Modular monolith (microservices-ready)** — one deployable, enforced module boundaries; lowest operational cost; extract services later. *(Architect's recommendation.)*
2. **Microservices from day one** — one independently deployable service per bounded context, each with its own database and an event bus.
3. **Serverless functions** — low idle cost and auto-scale, but cold-start friction and weaker fit for rich domain workflows.

## Decision
Adopt **microservices** — one service per bounded context, each independently deployable with its own database (ADR-0005) and communicating via an event broker (ADR-0008).

## Consequences
- (+) Independent deploy, scale, and fault isolation; clear ownership per context.
- (−) Distributed-systems cost from day one: eventual consistency, sagas, the broker as critical infrastructure, mandatory distributed tracing, and N deployment pipelines. **Mitigated** by .NET Aspire + Azure Container Apps (ADR-0009), MassTransit sagas/outbox (ADR-0008), and OpenTelemetry (ADR-0012).
- **Note:** Chosen over the architect's modular-monolith recommendation; the team explicitly accepts the operational trade-off in exchange for maximal independence and scalability.
