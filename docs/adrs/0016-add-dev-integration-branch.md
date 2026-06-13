# ADR-0016: Add a `dev` Integration Branch

- **Status:** Accepted
- **Date:** 2026-06-13
- **Deciders:** Product owner, Lead Architect
- **Supersedes:** [ADR-0014](0014-git-workflow-and-branching.md)

## Context
ADR-0014 adopted GitHub Flow with feature branches merging directly into `main`. To keep `main` strictly release-ready and stop in-progress feature work from landing on it directly, we introduce a long-lived integration branch.

## Options considered
1. **Keep GitHub Flow** (feature → `main`) — simplest, but `main` absorbs every in-progress merge.
2. **`main` + `dev`** — feature → `dev` → `main`; `main` receives only reviewed, release-ready merges.
3. **Full Git Flow** (`develop` + `release/*` + `hotfix/*`) — heavier than a solo build needs.

## Decision
Adopt a two-tier model:
- **`main`** — always-deployable, **protected**: no direct pushes; updated only by a PR with passing CI.
- **`dev`** — the integration branch and the default target for day-to-day work.
- **`feature/*`, `fix/*`, `chore/*`, `docs/*`** branch off `dev` and merge back into `dev` via CI-gated PRs.
- **Releases**: open a `dev` → `main` PR when a set of changes is ready to ship.
- CI and CodeQL run on PRs into — and pushes to — both `main` and `dev`.

This supersedes ADR-0014's direct feature→`main` flow.

## Consequences
- (+) `main` stays clean and release-only; integration risk is absorbed on `dev`; clearer release cadence; a natural place for a future staging deploy.
- (−) One extra hop (feature → `dev` → `main`). Acceptable — the safety and clarity outweigh it.
