# ADR-0011: Edge — YARP Gateway + Per-App BFFs

- **Status:** Accepted
- **Date:** 2026-06-12
- **Deciders:** Product owner, Lead Architect

## Context
Three frontends (storefront, seller-portal, admin) have different data-shaping needs, and internal services must not be exposed directly to the public internet.

## Options considered
1. **YARP + per-app BFFs** — a lightweight .NET reverse proxy with a Backend-for-Frontend per client; code-first, low cost, full control over auth and aggregation.
2. **Azure API Management (APIM)** — managed gateway with rate-limiting, policies, and a developer portal; powerful but costly and heavier to operate.
3. **Both — APIM at the edge + YARP/BFF inside** — most capable, most expensive; usually justified only at larger scale.

## Decision
Adopt a **YARP**-based reverse proxy with a **Backend-for-Frontend per client** (Storefront, Seller, Admin). The BFFs handle token validation, response aggregation, and frontend-shaped payloads. **APIM is deferred** until external/partner API management is required.

## Consequences
- (+) Frontend-optimized APIs, a single controlled ingress, code-first flexibility, and low cost.
- (−) BFFs are code we maintain; some cross-cutting concerns (e.g., advanced rate-limiting, developer portal) are hand-rolled until/unless APIM is introduced.
