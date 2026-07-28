# Prompt 101: Full Ecommerce Skill Routing Regression Test

## Prompt Number

101

## Date

2026-07-22

## Purpose

Rerun the complete twelve-scenario repository-local Skill-routing and conditional-loading test after the focused routing repairs, validate all ten canonical Skills and their evidence contracts, and create a findings-first full regression report without changing existing repository behavior or governance sources.

## Full Prompt

`APPROVED: EXECUTE FULL ECOMMERCE SKILL ROUTING REGRESSION TEST`

Act as a senior Codex Skill engineer and repository-governance test engineer. Work only inside the Ecommerce repository. Confirm and validate all ten Skills under `.agents/skills/`, then rerun scenarios A through L from routine Catalog planning through ambiguous architecture/deployment. For every scenario record the request, all Skills considered/invoked/skipped, instruction files loaded/skipped, Skill references loaded, observable output contract, result, and context-loading efficiency without exposing private chain-of-thought.

Create a complete 120-row Skill interaction matrix and an instruction-loading matrix covering routing files, applicable instructions, Skill references, ADRs, memory files, and explicit skip categories. Compare results with `docs/skills/testing/skill-routing-and-loading-test-report.md` and `docs/skills/testing/skill-routing-defect-regression-report.md`.

Create only `docs/skills/testing/full-skill-routing-regression-report.md` and this next numbered prompt log. Do not modify existing Skills, instructions, previous reports, runtime source, APIs, schemas, authentication, infrastructure, configuration, or the unrelated existing `appsettings.json` change. Do not create or apply migrations, install packages, package Skills, commit, push, reset, clean, or expose configuration values.

## Status

EXECUTED

## Result Summary

Created `docs/skills/testing/full-skill-routing-regression-report.md` and reran all twelve scenarios against the ten canonical repository Skills. The report contains every required observable trace field, a 120-row Skill interaction matrix, a 144-row instruction-loading matrix, output-contract validation, context-efficiency review, and comparison with both prior reports. All 120 Skill-routing combinations passed and no Skill-routing regression was found. Equivalent PowerShell structural validation found ten Skills and zero errors; canonical paths, metadata, references, output labels, Markdown fences, non-disclosing secret checks, and `git diff --check` passed. The official Python validator remained unavailable because no usable Python launcher is installed. No existing file, runtime behavior, configuration, migration, commit, or push was changed.
