# ADR-0005: Cloud & Data — Azure + Azure SQL, Database-per-Service

- **Status:** Accepted
- **Date:** 2026-06-12
- **Deciders:** Product owner, Lead Architect

## Context
We need a managed cloud platform and a relational datastore. The team is Microsoft-oriented, and each microservice must own its data to preserve autonomy and avoid hidden coupling through a shared schema.

## Options considered
1. **Azure + Azure SQL** — tightest ASP.NET Core / EF Core integration, managed backups and PITR, first-class tooling (SSMS).
2. **Azure + PostgreSQL** — lower licensing cost, OSS, still first-class with EF Core (Npgsql).
3. **AWS + PostgreSQL** — viable, but loses Azure-native integration for a Microsoft stack.

**Data-ownership sub-choice:** shared database vs **database-per-service**.

## Decision
Adopt **Azure** with **Azure SQL Database** (elastic pool for cost efficiency), administered/inspected via **SSMS**. Enforce a strict **database-per-service** model: no cross-service foreign keys or joins; cross-context data flows via integration events into local read models (DB-1, DB-2).

## Consequences
- (+) Native .NET integration, managed point-in-time-restore backups, and true data autonomy per service.
- (−) No cross-service joins — controlled data duplication via events is required; SQL licensing cost, mitigated by an elastic pool.
