# Prompt 105: Assistant Description Search Verification Blocker Cleanup Planning

- **Prompt Number:** 105
- **Date:** 2026-07-28
- **Purpose:** Plan a small cleanup that removes two baseline full-suite verification blockers while preserving the approved assistant catalog description-search behavior and its uncommitted regression coverage.
- **Status:** PLANNED

## Full Prompt

> Plan verification blocker cleanup for Assistant catalog description search.
>
> Context:
> Execution for Assistant catalog search by product description is incomplete because full-suite verification failed on two baseline issues:
> 1. Architecture ProjectGraph scans nested .worktrees projects and reports duplicate project names.
> 2. A Text-to-SQL test expects disabled settings, while committed settings enable them.
>
> The actual focused description-search regression passed, build passed, unit tests passed, and secret scan passed. CodeReview correctly blocked commit readiness because full verification failed.
>
> Goal:
> Create a small cleanup plan to unblock verification without changing the approved assistant description-search behavior.
>
> Important:
> - Planning only.
> - Do not write code.
> - Do not modify files except required prompt log if repo rules require it.
> - Do not commit, push, or create a PR.
> - Preserve the uncommitted description-search test work.
> - Do not change frontend.
> - Do not change MCP.
> - Do not add migrations.
> - Do not weaken architecture tests.
> - Do not weaken Text-to-SQL safety.
> - Identify whether project memory edits should be reverted or kept until verification passes.
>
> End with:
> PLAN_STATUS: PENDING_APPROVAL

## Result Summary

Planning identified two narrow baseline corrections. `ProjectGraph` should exclude only project files beneath the exact `.worktrees` directory segment while continuing to scan the rest of the checkout for unapproved or duplicate projects, with focused coverage proving both behaviors. Committed `Assistant:TextToSql:Enabled` should return to its documented fail-closed `false` default after an unrelated Skill-maintenance commit changed it to `true`, and the existing architecture test should parse configuration and assert the exact Text-to-SQL property rather than search for any false flag. The uncommitted description-search regression and failed prompt 104 must be preserved. Project-memory completion edits should remain reverted until cleanup verification and CodeReview pass; after that, current-state memory can be updated factually. No ADR, API, frontend, MCP, migration, schema, or assistant description-search behavior change is required.
