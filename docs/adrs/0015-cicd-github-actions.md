# ADR-0015: CI/CD with GitHub Actions

- **Status:** Accepted
- **Date:** 2026-06-12
- **Deciders:** Product owner, Lead Architect

## Context
ADR-0014 adopted PR-based GitHub Flow on the expectation that pull requests are gated by automated checks, and `Directory.Build.props` delegates code-style enforcement to `dotnet format` **in CI** (`EnforceCodeStyleInBuild=false`). Neither the gate nor the format check existed yet, so PRs were ungated and the style promise was unfulfilled. `infrastructure.rules.md` (INF-11) requires build→test→scan pipelines and `security.rules.md` (SEC-15/16) requires dependency and SAST scanning. We need CI now.

## Options considered
1. **GitHub Actions** — native to the GitHub-hosted repository, no extra service, free for this use.
2. **Azure DevOps Pipelines** — capable, but a second platform to operate for a repo already on GitHub.
3. **No automation** — unacceptable given the PR-gated workflow.

## Decision
Adopt **GitHub Actions**:
- **`ci.yml`** — on every PR to `main` and push to `main`: restore → `dotnet format --verify-no-changes` (style gate) → `dotnet build -c Release` (warnings-as-errors) → `dotnet test`. Runs on `ubuntu-latest` (.NET is cross-platform; ubuntu runners include Docker for Testcontainers integration tests).
- **`codeql.yml`** — CodeQL SAST for C# (SEC-16).
- **`dependabot.yml`** — weekly NuGet + GitHub-Actions dependency updates (SEC-15).
- Branch protection on `main` (require the CI check to pass before merge) is enabled once CI is green.

## Consequences
- (+) Every PR is built, style-checked, tested, and security-scanned; closes the `dotnet format`-in-CI loop; satisfies INF-11 / SEC-15 / SEC-16; demonstrates professional CI.
- (−) Consumes GitHub Actions minutes; CodeQL adds a few minutes per run.
- **Scope:** this is **CI**. Continuous **deployment** (containerize → push to ACR → deploy to Azure Container Apps via Bicep) is a separate later ADR, added when the IaC and Azure environment exist.
