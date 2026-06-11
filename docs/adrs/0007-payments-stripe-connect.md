# ADR-0007: Payments — Stripe Connect

- **Status:** Accepted
- **Date:** 2026-06-12
- **Deciders:** Product owner, Lead Architect, Security

## Context
A multi-vendor marketplace must split each payment across one or more sellers, onboard sellers with KYC, run payouts, and handle refunds — while minimizing PCI-DSS scope.

## Options considered
1. **Stripe Connect** — purpose-built for marketplaces: seller onboarding + KYC, destination/split charges, automated payouts, strong .NET SDK; client-side Elements keep us in PCI SAQ-A.
2. **PayPal / Braintree** — strong buyer trust and multi-party support; different payout ergonomics.
3. **Adyen for Platforms** — enterprise-grade global acquiring and risk tooling; heavier integration.

## Decision
Adopt **Stripe Connect** (Express/Custom accounts) with **destination charges**. Card data is captured exclusively by **Stripe.js / Elements** on the client and never touches our servers (**PCI SAQ-A**, SEC-2). Settlement is handled asynchronously via **signed, idempotent webhooks**. The **Payments service is the only Stripe integration point**.

## Consequences
- (+) Marketplace-native (KYC, split payments, payouts), minimal PCI scope, excellent .NET SDK and webhook model.
- (−) Coupling to Stripe and its fees; webhook idempotency complexity, handled via the Inbox pattern (DB-10) and signature verification (SEC-14).
