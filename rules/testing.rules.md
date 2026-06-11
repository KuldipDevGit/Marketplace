# Testing Rules

> **Status:** Active · **Version:** 1.0 · **Last updated:** 2026-06-12
> Governs all automated testing. **No production code without tests.** Changes require an ADR.

## Scope
All test code across services, gateways, frontends, and shared libraries.

## Strategy
- **TEST-1** Follow the **test pyramid** — many unit tests, fewer integration tests, few E2E tests.
- **TEST-2** **Coverage gates in CI:** Domain + Application ≥ **80%** line/branch; overall service ≥ **70%**. The build fails below threshold.

## Unit
- **TEST-3** **xUnit + FluentAssertions**; mock ports with NSubstitute. The Domain layer is tested with **zero mocks** (pure logic).
- **TEST-4** One behavior per test; Arrange-Act-Assert; fully deterministic — inject clock/random, no real time, network, or filesystem.

## Integration
- **TEST-5** Use **Testcontainers** (SQL Server + RabbitMQ) for real infrastructure in integration tests. No in-memory database substitute for EF-behavior-critical tests.
- **TEST-6** Test each MediatR handler end-to-end through the real `DbContext` and Outbox.

## Contracts & architecture
- **TEST-7** **Contract tests** (producer + consumer) for every integration event — schema and semantics.
- **TEST-8** **NetArchTest** enforces Clean Architecture boundaries and naming conventions; runs in CI (BE-16).

## API & end-to-end
- **TEST-9** API functional tests via `WebApplicationFactory`, asserting status codes and ProblemDetails contracts (API-4).
- **TEST-10** **Playwright** E2E for critical journeys: browse → cart → checkout → order; seller onboarding; admin moderation.
- **TEST-11** Frontend unit tests for services and components, plus axe accessibility assertions on key pages (FE-8).

## Process
- **TEST-12** Tests run in CI on every PR. Flaky tests are quarantined and fixed — never ignored.
- **TEST-13** Every bug fix adds a regression test that reproduces the bug.

## Enforcement
CI pipeline gates (coverage, architecture, contract, E2E) block merge on failure.
