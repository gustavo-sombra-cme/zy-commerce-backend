# Backend Code Review Checklist

Use this checklist before every commit that includes code, project/configuration files, migrations, CI workflow files, or runtime-behavior documentation changes.

Documentation-only maintenance changes are excluded from mandatory code review, but still require documentation self-review.

## Review Focus

Findings must lead the review and be ordered by severity.

Check:

* The change matches the approved scope.
* No unrelated files are included.
* No secrets, tokens, passwords, real connection strings, or API keys are included.
* No generated artifacts such as `bin`, `obj`, `TestResults`, `coverage`, `dist`, or package caches are included.
* Clean Architecture dependency direction is preserved.
* Module isolation is preserved.
* API controllers remain thin.
* CQRS command/query responsibilities are not mixed.
* Domain behavior remains in Domain models where applicable.
* Application handlers coordinate use cases without owning domain behavior.
* Infrastructure owns persistence and external concerns.
* Assistant and MCP adapters do not call EF Core DbContexts, repositories, Domain objects, or module internals directly.
* Text-to-SQL safety is not weakened.
* Push remains explicitly human-approved.

## Required Output

Use this format:

```text
Findings:
- Severity, file/line, issue, why it matters, recommended fix.

Open Questions:
- ...

Review Result:
PASS or BLOCKED
```

If there are no findings, say that clearly and list any remaining test gaps or residual risk.
