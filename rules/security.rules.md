# Security Rules

> **Status:** Active · **Version:** 1.0 · **Last updated:** 2026-06-12
> Governs the entire platform. **Security takes priority over convenience.** Implements ADR-0006 and ADR-0007. Changes require an ADR.

## Scope
All code, infrastructure, data, and processes across every service and frontend.

## Standards & compliance
- **SEC-1** Target **OWASP ASVS Level 2**; explicitly address the OWASP Top 10 in design and review.
- **SEC-2** **PCI-DSS scope minimized to SAQ-A** — card data is handled exclusively by Stripe.js/Elements and Stripe. It is never transmitted to, processed by, or logged on our servers.
- **SEC-3** **GDPR** — track lawful basis and consent; support data-subject rights (access, rectification, erasure, portability); minimize PII; document data residency; enforce retention policies.

## Authentication & authorization
- **SEC-4** Authentication via **Microsoft Entra External ID** (OIDC + PKCE); **MFA enforced** for sellers and admins.
- **SEC-5** Service-to-service auth via **Managed Identity** / client credentials — no shared static API keys.
- **SEC-6** Authorization is **deny-by-default**: RBAC roles (Buyer/Seller/Admin) plus resource-based checks (a seller may act only on their own resources).
- **SEC-7** Validate JWT issuer, audience, expiry, and signature on every service; access tokens are short-lived with refresh.

## Data & secrets
- **SEC-8** **TLS 1.2+** in transit; **encryption at rest** (Azure SQL TDE with Key Vault-managed keys).
- **SEC-9** **All secrets in Azure Key Vault** — never in source, config files, container images, or git. Rotate regularly.
- **SEC-10** **Never log** secrets, tokens, card data, or PII; redact in logs and telemetry.

## Application security
- **SEC-11** Validate and encode all input/output; **parameterized queries only** (EF Core) — no string-concatenated SQL.
- **SEC-12** Security headers (HSTS, CSP, X-Content-Type-Options, etc.) and a CORS allowlist enforced at the gateway/BFF.
- **SEC-13** Rate limiting, abuse protection, and lockout/backoff on public and authentication endpoints.
- **SEC-14** **Verify webhook signatures** (Stripe) and enforce idempotency on all inbound webhooks.

## Supply chain & audit
- **SEC-15** Dependency / SCA scanning in CI; no known-critical CVEs are shipped.
- **SEC-16** SAST, secret scanning, and IaC scanning run in CI.
- **SEC-17** **Immutable, tamper-evident audit log** for security-relevant events: authentication, payments, payouts, admin actions, and PII access.
- **SEC-18** Threat-model each new bounded context; a security review is a release gate.

## Enforcement
CI security scanning (SEC-15/16), gateway policy configuration, audit-log assertions in tests, and mandatory security review before release.
