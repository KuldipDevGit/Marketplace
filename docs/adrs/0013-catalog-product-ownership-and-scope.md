# ADR-0013: Catalog Product Ownership & Scope

- **Status:** Accepted
- **Date:** 2026-06-12
- **Deciders:** Product owner, Lead Architect

## Context
Catalog is the first and **reference** bounded context (the template all other services are cloned from). Three domain decisions define its data model and ripple into Inventory, Pricing, and Search: how sellers relate to products, whether to model product variants, and who governs the category taxonomy.

## Options considered
- **Ownership:** (a) **Seller-owned products** — each seller owns their listings (`Product.SellerId`); vs (b) **Shared catalog + offers** — one canonical product (Amazon ASIN) with many seller offers.
- **Variants:** (a) **Simple single-SKU** products with free-form attributes; vs (b) a **full variant matrix** (parent product + child SKU variants) now.
- **Taxonomy:** (a) **Admin-curated** category tree; vs (b) **seller-proposed** categories with moderation.

## Decision
For v1: **(a) seller-owned products**, **(a) simple single-SKU products** with free-form `ProductAttribute`s, and **(a) admin-curated taxonomy**. Catalog owns Product, Category, Brand, attributes, and media only — **price lives in Pricing, stock in Inventory** (DB-1/DB-2); cross-context references are by id with no foreign key, and Search assembles the read model from Catalog events.

## Consequences
- (+) Self-contained, quick-to-ship reference service; Inventory and Pricing key cleanly on `ProductId`; clean taxonomy governance.
- (−) No cross-seller comparison page (shared ASIN) until a future evolution; possible duplicate listings across sellers. Moving to a shared catalog later needs a product-matching/canonicalization effort (new ADR).
- (−) Introducing a variant matrix later requires parent/child SKU rework. Free-form attributes bridge the gap in the interim.
