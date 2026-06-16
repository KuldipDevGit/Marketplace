# ADR-0014: Git Workflow & Branching Strategy

- **Status:** Superseded by [ADR-0016](0016-add-dev-integration-branch.md)
- **Date:** 2026-06-12
- **Deciders:** Product owner, Lead Architect

## Context
The repository had no documented git workflow, and the initial scaffold + foundation were committed directly to `main`. For a portfolio that demonstrates professional practice — and to keep `main` always-deployable and reviewable — we need an explicit branching and pull-request strategy. This decision closes a gap in the Phase 0 governance (no rule previously covered git workflow).

## Options considered
1. **Trunk-based, direct commits to `main`** — simplest, but no review/CI gate per change and a weak history story.
2. **GitHub Flow** — short-lived branches off `main` + Pull Requests; `main` always deployable.
3. **Git Flow** (`develop` + `release/*` + `hotfix/*`) — powerful for scheduled releases, but heavy for a continuously-deployed, solo build.

## Decision
Adopt **GitHub Flow**:
- All changes land via **short-lived branches** named `feature/*`, `fix/*`, `chore/*`, or `docs/*` — **one branch per unit of functionality**.
- Every branch merges into `main` through a **Pull Request**; `main` is always deployable.
- PRs run CI (build + tests + analyzers) and are **squash-merged**; commit messages follow Conventional Commits.
- Even as a solo developer, work goes through PRs for traceability and the CI gate (self-merge after checks pass).
- Branch protection on `main` (require PR + passing status checks) is enabled once the CI pipeline exists.

## Consequences
- (+) `main` stays green and deployable; every change is reviewable and CI-gated; the history demonstrates professional workflow.
- (−) A small per-change overhead (branch + PR); acceptable and largely automatable.
- **Note:** commits `ab1efa1..3837130` predate this ADR and remain on `main` as the baseline; history is not rewritten.
