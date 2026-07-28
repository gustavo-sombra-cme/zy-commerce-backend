# Prompt 103: Assistant Catalog Description Search Planning

- **Prompt Number:** 103
- **Date:** 2026-07-28
- **Purpose:** Plan a small read-only assistant catalog-search extension so product discovery can match product description text while preserving existing assistant, API, MCP, Text-to-SQL, persistence, and security boundaries.
- **Status:** PLANNED

## Full Prompt

> Plan assistant catalog search by product description.
>
> Goal:
> Extend the existing read-only assistant catalog search so product discovery can match description text in addition to the current supported search behavior.
>
> Context:
> We want this small runtime feature to validate the repository-local workflow Skills, especially CodeReview. The implementation must exercise the normal workflow: branch start, prompt log, verification, secret scan, code review, commit readiness, and project memory update if needed.
>
> Constraints:
> Do not add a new API endpoint.
> Do not change frontend.
> Do not change MCP.
> Do not change Text-to-SQL internals.
> Do not add migrations unless the existing search model cannot support description safely.
> Do not add assistant write/admin behavior.
> Do not expose raw SQL or genericTable.
> Use selective loading. Do not read unrelated prompt logs or all historical project memory unless required.
>
> End with:
> PLAN_STATUS: PENDING_APPROVAL

## Result Summary

Planning inspected the existing assistant catalog-search path, its focused tests, relevant repository guidance, current project memory, and ADR ownership. `origin/main` already maps `Description` in the infrastructure-only product search read model but filters search text by SKU and Name only; the current bounded-agent feature branch already contains an overlapping unmerged Description predicate. The approval-ready plan therefore starts from latest `main`, adds the Description predicate only if it is still absent after branch setup, adds focused regression coverage, preserves the endpoint and read-only tool boundary, avoids Text-to-SQL/MCP/frontend/schema changes, and includes the required execution-stage workflow checks.
