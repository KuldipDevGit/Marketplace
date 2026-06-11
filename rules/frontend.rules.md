# Frontend Rules

> **Status:** Active · **Version:** 1.0 · **Last updated:** 2026-06-12
> Governs the Angular workspace under `web/` (storefront, seller-portal, admin). Changes require an ADR.

## Scope
All Angular application and library code, styling, and frontend tests.

## Components & structure
- **FE-1** **Standalone components** (no NgModules), latest Angular; feature-based folder structure.
- **FE-2** `ChangeDetectionStrategy.OnPush` on every component.
- **FE-3** **Smart vs. presentational split** — presentational ("dumb") components expose `@Input`/`@Output` only and inject no services.
- **FE-4** **No business logic in components** — delegate to services and state.

## State & data
- **FE-5** **Signals** for local/component state; **NgRx ComponentStore** for complex feature state; global NgRx only when justified by an ADR.
- **FE-6** All HTTP goes through **NSwag-generated typed clients**, regenerated from each service's OpenAPI document. No hand-written, untyped HTTP calls.
- **FE-7** No `any`. Strict TypeScript, typed reactive forms.

## UX & quality
- **FE-8** **WCAG 2.1 AA** — semantic HTML, ARIA where needed, full keyboard navigation, sufficient color contrast, managed focus; verified with axe.
- **FE-9** Mobile-first responsive design; theming via **PrimeNG** design tokens; no inline magic-number styles.
- **FE-10** Lazy-load feature routes; enforce bundle-size budgets in the build.
- **FE-11** **i18n-ready** — no hardcoded user-facing strings.
- **FE-12** Configuration via Angular environment files; **no secrets in the SPA**. Auth via OIDC/PKCE (Entra External ID); access tokens kept in memory with silent renew — avoid `localStorage`.

## Testing & errors
- **FE-13** Unit tests for services and components; **Playwright** for E2E. No feature merged without tests (see `testing.rules.md`).
- **FE-14** A central HTTP error interceptor provides consistent, user-friendly error handling and surfaces correlation IDs for support.

## Enforcement
ESLint + Angular strict mode, bundle-budget build failures, axe accessibility assertions, and code review.
