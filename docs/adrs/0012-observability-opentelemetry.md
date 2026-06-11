# ADR-0012: Observability — OpenTelemetry + Azure Monitor

- **Status:** Accepted
- **Date:** 2026-06-12
- **Deciders:** Product owner, Lead Architect

## Context
Operating and debugging distributed services (ADR-0003) is impossible without correlated traces, metrics, and logs that follow a request across service and broker boundaries.

## Options considered
1. **OpenTelemetry → Azure Monitor / Application Insights** — vendor-neutral instrumentation, Azure-native dashboards, wired via Aspire `ServiceDefaults`.
2. **Serilog + ELK (Elastic) stack** — flexible and powerful, but a self-managed observability platform to operate.
3. **Third-party APM (Datadog, etc.)** — turnkey and feature-rich, but additional vendor cost and coupling.

## Decision
Adopt **OpenTelemetry** (traces, metrics, logs) wired through Aspire `ServiceDefaults`, exported to **Azure Monitor / Application Insights**. Use **Serilog** for structured logging and propagate **W3C trace-context** correlation IDs across services *and* the message broker. Every service exposes liveness/readiness health checks (INF-9).

## Consequences
- (+) Vendor-neutral instrumentation, distributed tracing from day one, and Azure-native dashboards/alerting.
- (−) Telemetry volume incurs cost; a sampling strategy is required and is tuned per environment.
