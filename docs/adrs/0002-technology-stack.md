# ADR-0002: Technology Stack (.NET 10 LTS + Angular)

- **Status:** Accepted
- **Date:** 2026-06-12
- **Deciders:** Product owner, Lead Architect

## Context
We need a coherent, production-grade, strongly-typed stack with first-class Azure support for a Microsoft-oriented team. As a **full-production** system, the runtime must sit on a **Long-Term Support (LTS)** release for the longest security and patch window.

## Options considered
1. **.NET + ASP.NET Core + Angular + EF Core + NSwag** — one ecosystem, strong typing end-to-end, excellent Azure integration.
2. **Node / NestJS + Next.js (TypeScript full-stack)** — single language, large ecosystem, but weaker enterprise-backend story for this team.
3. **Java / Spring + React** — strong enterprise backend, but a second toolchain and less Azure-native than .NET.

**Runtime sub-choice:** **.NET 10 (LTS, ~3-year support)** vs .NET 9 (STS, support window lapsed by mid-2026).

## Decision
Adopt **Option 1 on .NET 10 (LTS)** — ASP.NET Core Web API services; Angular SPAs (storefront, seller-portal, admin); EF Core for persistence; NSwag to generate typed Angular clients from each service's OpenAPI document. **All projects target `net10.0`.**

## Consequences
- (+) One platform with strong end-to-end typing, excellent Azure integration, and NSwag-synchronized front/back contracts.
- (+) A **3-year LTS support window** appropriate for a production system; avoids the support-lifecycle risk of an STS release.
- (−) Two languages (C# / TypeScript) to maintain; less code-sharing than an all-JavaScript stack.
