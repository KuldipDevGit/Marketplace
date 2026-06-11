# ADR-0008: Messaging — RabbitMQ + MassTransit

- **Status:** Accepted
- **Date:** 2026-06-12
- **Deciders:** Product owner, Lead Architect

## Context
Microservices (ADR-0003) need reliable asynchronous eventing for integration events and for distributed transactions (e.g., the place-order workflow spanning Ordering, Inventory, and Payments).

## Options considered
1. **RabbitMQ + MassTransit** — portable, low-cost broker; MassTransit provides sagas, retries, scheduling, outbox, and idempotency.
2. **Azure Service Bus + MassTransit** — fully managed broker, minimal ops, but Azure-coupled and higher cost.
3. **Kafka / Azure Event Hubs** — high-throughput durable log enabling event sourcing/replay; heavier to operate.

## Decision
Adopt **RabbitMQ** as the broker with **MassTransit**. Use a transactional **Outbox** per service for reliable publishing (DB-9), **Saga orchestration** (MassTransit state machines) for cross-service workflows, and an **Inbox** / idempotent consumers for exactly-once *effect* (DB-10).

## Consequences
- (+) Portable and low-cost; rich MassTransit feature set (retry, scheduling, sagas, outbox) reduces bespoke code.
- (−) We operate RabbitMQ ourselves — containerized locally via Aspire, and **HA-clustered with persistence** in staging/prod (INF-5). It is not a managed service.
