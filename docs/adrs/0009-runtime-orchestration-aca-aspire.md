# ADR-0009: Runtime & Orchestration — Azure Container Apps + .NET Aspire

- **Status:** Accepted
- **Date:** 2026-06-12
- **Deciders:** Product owner, Lead Architect

## Context
We need a production container runtime for 8+ services and a manageable local-development experience that can run the whole system at once.

## Options considered
1. **Azure Container Apps (ACA) + .NET Aspire** — serverless containers with KEDA autoscale, scale-to-zero, and optional Dapr; Aspire orchestrates locally with a dashboard, service discovery, and telemetry wiring.
2. **Azure Kubernetes Service (AKS)** — maximum control and ecosystem, but significant, ongoing cluster/networking/upgrade operations.
3. **Azure App Service** — simple, but a weaker fit for many independently-scaled containerized services with a broker.

## Decision
Adopt **Azure Container Apps** for production (serverless containers, KEDA autoscaling, scale-to-zero, Dapr available) and **.NET Aspire** for local orchestration, the developer dashboard, service discovery, and OpenTelemetry wiring (`ServiceDefaults`). AppHost and ServiceDefaults target `net10.0`.

## Consequences
- (+) Production-grade microservices **without** managing Kubernetes; one-command local run of the full system.
- (−) Less low-level control than AKS and some ACA limits; Aspire is a young (Microsoft-backed) platform. A documented **AKS migration path** is retained should we outgrow ACA.
