# Database Rules

> **Status:** Active · **Version:** 1.0 · **Last updated:** 2026-06-12
> Governs all persistence (Azure SQL via EF Core). Implements ADR-0005. Changes require an ADR.

## Scope
Every database, schema, EF Core model, migration, and data-access path.

## Ownership
- **DB-1** **Database-per-service** — a service accesses only its own database.
- **DB-2** **No cross-service foreign keys or joins.** Data needed from another context is replicated via integration events into a local read model.

## Schema & naming
- **DB-3** **PascalCase** tables and columns; singular table names; a schema per bounded context where it adds clarity.
- **DB-4** Surrogate primary keys (sequential GUID or identity per service); natural keys expressed as unique constraints.
- **DB-5** Money is `decimal(19,4)` with an explicit currency column; timestamps are UTC `datetime2`. **Never** use floating-point for money.

## Lifecycle & audit
- **DB-6** Audit columns on every table: `CreatedAtUtc`, `CreatedBy`, `UpdatedAtUtc`, `UpdatedBy`.
- **DB-7** **Soft delete** via `IsDeleted` + an EF Core global query filter. Hard delete only with approval or for GDPR erasure (SEC-3).
- **DB-8** **Optimistic concurrency** via a `rowversion` token on mutable aggregates.

## Reliability
- **DB-9** **Transactional Outbox** table per service — integration events are written in the same transaction as the state change and published by a relay (ADR-0008).
- **DB-10** **Inbox / processed-message** table for idempotent consumers (exactly-once effect).

## Migrations & indexing
- **DB-11** All schema changes via **EF Core migrations** committed to source. No manual production schema edits. Reversibility is tested.
- **DB-12** Index all foreign keys and common query predicates; review execution plans for hot paths; always paginate (API-8).

## Security
- **DB-13** Connect via **Managed Identity** — no SQL passwords in configuration; connection metadata in Key Vault (SEC-9).
- **DB-14** Encrypt PII columns (Always Encrypted) where required. **Never store card data** — that lives only with Stripe (ADR-0007, SEC-2).

## Enforcement
EF Core migration review, integration tests against real SQL Server via Testcontainers (TEST-5), and code review.
