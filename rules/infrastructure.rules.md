# Infrastructure Rules

> **Status:** Active · **Version:** 1.0 · **Last updated:** 2026-06-12
> Governs all environments and deployment. Implements ADR-0009 and ADR-0012. Changes require an ADR.

## Scope
Cloud resources, container runtime, CI/CD, configuration, secrets, observability, and disaster recovery.

## Infrastructure as Code
- **INF-1** All infrastructure is **Bicep** under `build/bicep`. The Azure portal is read-only / break-glass — no manual production changes.
- **INF-2** Environments **dev / staging / prod** share one parameterized topology; promotion happens through pipelines.

## Runtime
- **INF-3** Services deploy as containers to **Azure Container Apps**; releases use rolling / blue-green with health-gated traffic shifting.
- **INF-4** Autoscaling via **KEDA** (HTTP concurrency + queue depth); scale-to-zero is allowed for idle non-critical services.
- **INF-5** RabbitMQ runs **HA-clustered with persistent storage** in staging/prod — never a single point of failure (ADR-0008).

## Configuration & secrets
- **INF-6** Configuration via Azure App Configuration; secrets via **Key Vault + Managed Identity**. Nothing secret in images or the repo (SEC-9).
- **INF-7** Container images are versioned, **scanned**, and pulled from a private ACR. **No `:latest` in production.**

## Reliability & DR
- **INF-8** Azure SQL with **point-in-time restore + geo-redundant backups**; documented RPO/RTO; restore tested quarterly.
- **INF-9** **Liveness/readiness** health checks on every service; gateways route only to healthy revisions.
- **INF-10** Defined **SLOs and alerting** (latency, error rate, saturation, queue depth) routed to on-call.

## CI/CD
- **INF-11** Per-service pipelines triggered by monorepo path filters: build → test → scan → containerize → deploy. No deploy if any gate fails.
- **INF-12** Full traceability: commit → image tag → deployed revision.

## Observability & governance
- **INF-13** OpenTelemetry is wired via Aspire `ServiceDefaults` → Azure Monitor; logs are centralized, structured, and correlated across services and the broker (ADR-0012).
- **INF-14** All resources are **tagged** (env, service, owner, cost-center); **least-privilege RBAC**; no standing admin access.

## Enforcement
IaC scanning + policy-as-code in CI (INF-1), pipeline gates (INF-11), and infrastructure review.
