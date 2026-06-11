# Backend Rules

> **Status:** Active · **Version:** 1.0 · **Last updated:** 2026-06-12
> Governs all ASP.NET Core services under `src/Services/*`. Implements ADR-0003 (microservices) and ADR-0004 (Clean Architecture). Changes require an ADR.

## Scope
All .NET service code: Domain, Application, Infrastructure, and Api layers of every microservice, plus shared `BuildingBlocks`.

## Layering & dependencies
- **BE-1** Four projects per service: `*.Domain`, `*.Application`, `*.Infrastructure`, `*.Api`.
- **BE-2** **Dependency rule:** Domain depends on nothing; Application → Domain; Infrastructure → Application + Domain; Api → Application (and Infrastructure for DI wiring only). Dependencies point inward only — never the reverse.
- **BE-3** Domain contains **no** EF Core, framework, or I/O — only entities, value objects, domain events, and domain services.
- **BE-4** Application defines **ports** (interfaces); Infrastructure provides the implementations.

## CQRS & use cases
- **BE-5** **CQRS via MediatR** — one request per use case (Commands mutate state, Queries read). Handlers are thin; business logic lives in the Domain.
- **BE-6** Validate every command/query with **FluentValidation**, invoked by a MediatR pipeline behavior.
- **BE-7** Cross-cutting concerns (validation, logging, transactions, idempotency) are MediatR pipeline behaviors — not duplicated in handlers.

## Domain integrity
- **BE-8** **Rich domain model** — invariants enforced inside aggregates; no public setters that can break an invariant; no anemic entities.
- **BE-9** Aggregates are consistency boundaries; reference other aggregates **by identifier**, not by navigation property.
- **BE-10** Raise **domain events** for state changes; translate to **integration events** at the Application boundary. Domain events never cross a service boundary.

## Persistence & contracts
- **BE-11** EF Core entities never cross the Api boundary — map to DTOs.
- **BE-12** All I/O is `async` with `CancellationToken` propagated end-to-end.
- **BE-13** A service never reads or writes another service's database. Cross-service data flows only via API calls or integration events (ADR-0005).

## Quality & errors
- **BE-14** Nullable reference types enabled; **warnings-as-errors**; analyzers + style pass (`Directory.Build.props`, `.editorconfig`).
- **BE-15** Throw domain/application exceptions; global middleware maps them to **RFC 7807 ProblemDetails** (see `api.rules.md`). No swallowed exceptions.
- **BE-16** Clean Architecture boundaries and naming are **enforced by NetArchTest** in `tests/` — a boundary violation fails the build.

## Enforcement
Roslyn analyzers + `Directory.Build.props` (BE-14), NetArchTest architecture tests (BE-2, BE-16), and code review.
