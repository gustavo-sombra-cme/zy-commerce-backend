# 099 - Ecommerce Skill Routing And Conditional Loading Test

Date: 2026-07-21

## Purpose

Test all repository-local Codex Skills and verify conditional instruction and reference loading without changing application behavior.

## Full Prompt

```text
APPROVED: EXECUTE TEST ECOMMERCE SKILLS AND CONDITIONAL INSTRUCTION LOADING

Use Skill Creator guidance to validate every Skill under `.agents/skills/` and dry-run twelve routing scenarios. Record explicit observable routing traces, expected-versus-actual Skill and instruction-loading matrices, trigger-quality and overlap reviews, context efficiency, defects, and recommendations. Initially load only AGENT.md and instructions/06-loading-index.md, then load Skill bodies, instruction files, project documents, ADRs, and Skill references only when their declared triggers apply. Create only the required report under docs/skills/testing/ and this prompt log. Do not modify Skills or runtime code, create or apply migrations, expose secrets, package Skills, clean/reset the worktree, commit, or push.
```

The complete approved scenario definitions, trace schema, matrices, acceptance criteria, scope restrictions, and final-report contract are preserved in the task attachment supplied for this execution.

## Status

EXECUTED

## Result Summary

Discovered and structurally validated all ten repository-local Skills, then forward-tested twelve routing scenarios with metadata-first discovery and conditional body/reference loading. Safe blocking worked for the dirty execution start, unapproved migration creation/application, incomplete verification, mixed-scope commit readiness, push readiness, proposed committed secrets, and ambiguous architecture/deployment request. The report contains twelve complete traces, a 120-pair Skill interaction matrix, a 228-row instruction-loading matrix, trigger-quality ratings, overlap ownership, context-efficiency ratings, verified defects, and unapplied recommendations.

No Skill was modified. The test created only `docs/skills/testing/skill-routing-and-loading-test-report.md` and this prompt log. No runtime code, API, database schema, migration, authentication, infrastructure, secret, package, branch, commit, push, reset, clean, or deployment action was created or changed. The pre-existing unrelated `src/Api/Ecommerce.Api/appsettings.json` Text-to-SQL flag modification was inspected safely, reported as a review blocker, and left untouched.
