# ADR-0006: Authentication & Authorization

- **Status:** Accepted
- **Date:** 2026-06-12
- **Deciders:** Product owner, Lead Architect, Security

## Context
The marketplace has three end-user client types (buyers, sellers, admins) plus service-to-service calls. As a full-production system with GDPR obligations, we want to avoid the risk and compliance burden of a homegrown identity system.

## Options considered
1. **Microsoft Entra External ID (managed IdP)** — offloads MFA, social login, breach detection, and much GDPR/security compliance; Azure-native; per-MAU cost.
2. **Duende IdentityServer (self-hosted OIDC)** — full control; commercially licensed above a revenue threshold; we own more of the compliance burden.
3. **ASP.NET Core Identity (self-managed)** — no external dependency, but we implement MFA, lockout, social login, and token issuance ourselves.
4. **Auth0 / Okta** — polished managed IdP; multi-cloud; cost scales with MAU.

## Decision
Use **Microsoft Entra External ID** for end-user identity (OIDC + PKCE, MFA, social login, password-breach detection). Service-to-service authentication uses **Managed Identity** / client credentials. Each service performs **RBAC + resource-based authorization** (SEC-6).

## Consequences
- (+) Offloads MFA, breach detection, and a large share of compliance; Azure-native; no homegrown-auth risk.
- (−) Coupling to Entra External ID; per-MAU cost; local development requires a test tenant or a stub. Mitigated by an auth abstraction in the Identity service.
