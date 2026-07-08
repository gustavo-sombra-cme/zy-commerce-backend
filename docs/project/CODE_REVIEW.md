# Backend Code Review Checklist

Use this checklist before every commit that includes code, project/configuration files, migrations, CI workflow files, or runtime-behavior documentation changes.

Documentation-only maintenance changes are excluded from mandatory code review, but still require documentation self-review.

## Review Focus

Findings must lead the review and be ordered by severity.

Check:

* Correctness: the implementation matches the approved scope, handles expected edge cases, and avoids behavior regressions.
* The change matches the approved scope.
* No unrelated files are included.
* No secrets, tokens, passwords, real connection strings, or API keys are included.
* No generated artifacts such as `bin`, `obj`, `TestResults`, `coverage`, `dist`, or package caches are included.
* Clean Architecture dependency direction is preserved.
* Module isolation is preserved.
* Frontend contract safety is preserved: request/response shapes, route names, status codes, auth requirements, and documented behavior are not changed unless explicitly approved and documented.
* API controllers remain thin.
* CQRS command/query responsibilities are not mixed.
* Domain behavior remains in Domain models where applicable.
* Application handlers coordinate use cases without owning domain behavior.
* Infrastructure owns persistence and external concerns.
* Assistant and MCP adapters do not call EF Core DbContexts, repositories, Domain objects, or module internals directly.
* Migrations, schema changes, raw SQL, database users, and connection strings have explicit approval and do not introduce startup auto-migration or committed credentials.
* Tests are present for behavior changes, explicitly skipped with rationale, or not applicable for documentation-only maintenance.
* Runtime-behavior documentation accurately describes the current implementation and does not imply unbuilt behavior is complete.
* AI assistant safety is preserved: no raw SQL exposure, no `genericTable` exposure, no cross-user data, no provider prompt/response logging, and no admin/write tool expansion.
* Text-to-SQL safety is not weakened: generated SQL remains untrusted, validated, feature-flagged, read-only, and hidden from frontend responses.
* MCP boundaries are preserved: no new tools, resources, prompts, auth behavior, SQL/database access, or cross-user access without explicit approval.
* Push remains explicitly human-approved.

## Required Output

Use this format:

```text
Findings:
- Severity [BLOCKER|HIGH|MEDIUM|LOW], file/line, issue, why it matters, recommended fix.

Open Questions:
- ...

Review Result:
PASS or BLOCKED

Commit Blocked:
YES or NO

Required Follow-up:
- ...
```

If there are no findings, say that clearly and list any remaining test gaps or residual risk.
