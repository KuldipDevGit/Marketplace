# Architecture Decision Records

Significant architecture decisions are recorded here using the **Nygard ADR format**. Per [ADR-0001](0001-record-architecture-decisions.md), records are **immutable once Accepted** — a change is made by adding a superseding ADR, not by editing an existing one.

| # | Title | Status |
|---|---|---|
| [0001](0001-record-architecture-decisions.md) | Record Architecture Decisions | Accepted |
| [0002](0002-technology-stack.md) | Technology Stack (.NET 10 LTS + Angular) | Accepted |
| [0003](0003-deployment-topology-microservices.md) | Deployment Topology — Microservices | Accepted |
| [0004](0004-per-service-clean-architecture.md) | Per-Service Internal Architecture — Clean Architecture | Accepted |
| [0005](0005-cloud-and-data-azure-sql.md) | Cloud & Data — Azure + Azure SQL, Database-per-Service | Accepted |
| [0006](0006-authentication-authorization.md) | Authentication & Authorization | Accepted |
| [0007](0007-payments-stripe-connect.md) | Payments — Stripe Connect | Accepted |
| [0008](0008-messaging-rabbitmq-masstransit.md) | Messaging — RabbitMQ + MassTransit | Accepted |
| [0009](0009-runtime-orchestration-aca-aspire.md) | Runtime & Orchestration — ACA + .NET Aspire | Accepted |
| [0010](0010-repository-strategy-monorepo.md) | Repository Strategy — Monorepo | Accepted |
| [0011](0011-edge-yarp-gateway-bff.md) | Edge — YARP Gateway + Per-App BFFs | Accepted |
| [0012](0012-observability-opentelemetry.md) | Observability — OpenTelemetry + Azure Monitor | Accepted |

**To propose a change:** copy [`template.md`](template.md) to the next sequential number, complete it, and submit for approval (see `rules/ai-agent.rules.md`, AI-4).
