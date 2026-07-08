# migration-safety-check

## Purpose

Guard database schema and migration changes.

## When to use

Required whenever migrations, raw SQL, schema changes, database users, or connection strings are involved.

## Inputs

* Changed files.
* Approved migration scope.
* Affected database.

## Outputs

* Migration safety result.
* Approval status.
* Verification expectations.

## Required reading

* `instructions/03-cqrs-database-testing-security.md`
* `docs/project/PROJECT_STATUS.md`
* `docs/project/ASSISTANT_TEXT_TO_SQL_READONLY_DB.md` when Text-to-SQL views are involved.

## Files it may read

* Migration files.
* DbContext configuration.
* Database documentation.

## Files it may update

None directly.

## Stop conditions

* Migration or schema change lacks explicit approval.
* Migration execution is attempted without explicit approval.
* Startup auto-migration is introduced.
* Real database passwords or read-only connection strings are committed.
* Raw SQL exceeds approved scope.

## Verification expectations

Confirm restore/build/test requirements and manual DB verification expectations when applicable.

## Final output format

```text
Migration files:
Approval:
Risks:
Migration safety: PASS or BLOCKED
```
