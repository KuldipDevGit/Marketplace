# Marketplace — Multi-Vendor E-Commerce Platform

A production-grade, Amazon-inspired **multi-vendor marketplace** built as **microservices** on **.NET 10 (LTS)** and **Angular**, with strict architectural governance (rules + ADRs) and approval-gated delivery.

> **Status:** Phase 0 — governance scaffold. No application code yet. See [Delivery Phases](#delivery-phases).

---

## Architecture at a glance

| Concern | Decision | ADR |
|---|---|---|
| Intent | Full production (PCI-DSS SAQ-A, GDPR) | — |
| Vendor model | Multi-vendor marketplace | — |
| Backend | ASP.NET Core Web API on **.NET 10 LTS**, EF Core | [0002](docs/adrs/0002-technology-stack.md) |
| Frontend | Angular SPAs (storefront, seller, admin) + NSwag clients | [0002](docs/adrs/0002-technology-stack.md) |
| Deployment topology | **Microservices** (one per bounded context) | [0003](docs/adrs/0003-deployment-topology-microservices.md) |
| Per-service layering | **Clean Architecture** | [0004](docs/adrs/0004-per-service-clean-architecture.md) |
| Cloud & data | Azure + Azure SQL, **database-per-service** | [0005](docs/adrs/0005-cloud-and-data-azure-sql.md) |
| Auth | Microsoft Entra External ID + Managed Identity | [0006](docs/adrs/0006-authentication-authorization.md) |
| Payments | **Stripe Connect** (split charges + payouts) | [0007](docs/adrs/0007-payments-stripe-connect.md) |
| Messaging | **RabbitMQ + MassTransit** (Outbox + Sagas) | [0008](docs/adrs/0008-messaging-rabbitmq-masstransit.md) |
| Runtime / orchestration | **Azure Container Apps + .NET Aspire** | [0009](docs/adrs/0009-runtime-orchestration-aca-aspire.md) |
| Repository | Monorepo | [0010](docs/adrs/0010-repository-strategy-monorepo.md) |
| Edge | **YARP gateway + per-app BFFs** | [0011](docs/adrs/0011-edge-yarp-gateway-bff.md) |
| Observability | OpenTelemetry → Azure Monitor | [0012](docs/adrs/0012-observability-opentelemetry.md) |

Full decision records: [`docs/adrs/`](docs/adrs/).

## Governance

All implementation follows the rules in [`rules/`](rules/). **If a rule conflicts with a request, work stops and the conflict is raised** (see [`ai-agent.rules.md`](rules/ai-agent.rules.md)).

| Rule file | Governs |
|---|---|
| [ai-agent.rules.md](rules/ai-agent.rules.md) | How the AI agent operates (workflow, approval gates) |
| [backend.rules.md](rules/backend.rules.md) | Clean Architecture, CQRS, domain integrity |
| [frontend.rules.md](rules/frontend.rules.md) | Angular standards, state, accessibility |
| [api.rules.md](rules/api.rules.md) | OpenAPI-first, versioning, error format |
| [database.rules.md](rules/database.rules.md) | Database-per-service, naming, migrations, audit |
| [security.rules.md](rules/security.rules.md) | OWASP, PCI, GDPR, secrets, audit |
| [testing.rules.md](rules/testing.rules.md) | Test pyramid, coverage gates, Testcontainers |
| [infrastructure.rules.md](rules/infrastructure.rules.md) | Bicep IaC, deployment, DR, monitoring |

## Repository structure

```
src/
  Aspire/            .NET Aspire AppHost + ServiceDefaults (orchestration, OTel)
  Services/          Bounded-context microservices (Clean Architecture each)
                     Identity · Catalog · Search · Cart · Ordering · Payments · Sellers · Inventory
  Gateways/          YARP BFFs: Storefront.Bff · Seller.Bff · Admin.Bff
  BuildingBlocks/    SharedKernel · EventBus.MassTransit · Contracts · Observability
web/                 Angular workspace (apps/ + libs/)
tests/               Unit · integration (Testcontainers) · architecture (NetArchTest) · E2E (Playwright)
build/               Bicep IaC + CI/CD pipelines
docs/                ADRs · architecture · runbooks
rules/               Governance rule files
```

## Delivery phases

- **Phase 0 — Governance bootstrap** ✅ *(this scaffold: rules, ADRs, skeleton, config)*
- **Phase 1 — Contract-first foundation** — OpenAPI + data model + one reference service end-to-end
- **Phase 2 — Core vertical slice** — Identity · Catalog · Search · Cart · Ordering · Payments · Sellers · Inventory
- **Phase 3+ — Remaining capabilities** — Reviews · Promotions · Recommendations · Notifications · Analytics

## Prerequisites (from Phase 1 onward)

- .NET 10 SDK (LTS)
- Node.js LTS + Angular CLI
- Docker Desktop (WSL2) — required for .NET Aspire and Testcontainers
- Azure subscription (for cloud deployment)

## Getting started

Application code begins in **Phase 1**. Until then this repository contains governance and structure only.

## License

See [`LICENSE`](LICENSE) (to be finalized).
