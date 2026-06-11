# API Rules

> **Status:** Active · **Version:** 1.0 · **Last updated:** 2026-06-12
> Governs all HTTP APIs (services and BFF gateways). Implements ADR-0011. Changes require an ADR.

## Scope
Every HTTP endpoint exposed by a service or a BFF gateway, and its OpenAPI contract.

## Contract
- **API-1** **OpenAPI-first / contract-driven.** The OpenAPI document is the source of truth; Angular clients are generated from it via NSwag (FE-6).
- **API-2** RESTful resource modeling; correct HTTP verbs and status codes.
- **API-3** **URL versioning** — `/api/v{n}/...`. No breaking change without a major version bump.

## Errors & validation
- **API-4** Errors use **RFC 7807 ProblemDetails** (`type`, `title`, `status`, `detail`, `traceId`, `errors{}`), consistent across all services.
- **API-5** Validate all input (FluentValidation). Reject invalid requests with `400` + ProblemDetails.
- **API-6** Never leak stack traces or internal implementation details in responses.

## Semantics
- **API-7** Mutating endpoints (`POST`/`PUT`/`PATCH`/`DELETE`) accept an **`Idempotency-Key`** header and enforce idempotency.
- **API-8** List endpoints support pagination, sorting, and filtering with documented limits — no unbounded result sets.
- **API-9** Use ETags / concurrency tokens for updates where applicable (optimistic concurrency, DB-8).

## Cross-cutting
- **API-10** Every request/response carries a **W3C `traceparent`** correlation id (ADR-0012).
- **API-11** **Authentication required by default**; `[AllowAnonymous]` is explicit and intentional; authorize per resource (SEC-6).
- **API-12** Every endpoint is documented in OpenAPI (summary, parameters, responses, examples).
- **API-13** Public endpoints are rate-limited at the BFF/gateway (SEC-13).

## Enforcement
OpenAPI schema validation in CI, contract tests (TEST-7), gateway policies, and code review.
