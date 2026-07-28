---
name: migration-safety-check
description: Reviews proposed or changed database schemas, EF migrations, raw SQL schema or data operations, migration-affecting database users or permissions, and connection changes that alter the target database or ownership boundary. Use for database-impacting work, not credential-only or API-key-only handling. Do not create or execute migrations unless the current task explicitly authorizes those actions.
---

# Migration Safety Check

## Required input

Obtain the proposed or approved database scope, changed files or proposal, affected module and database, intended verification or rollout plan, and separate evidence for migration creation and migration execution approval.

## Workflow

1. Classify applicability. Trigger for schema changes, EF migration creation or modification, raw SQL schema or data operations, migration execution, database users or permissions that affect migration behavior, or connection changes that alter the target database or ownership boundary.
2. Do not trigger for an API key, credential-only secret exposure, placeholder connection string, or connection-string credential handling when no database target, ownership, permission, schema, migration, or SQL operation changes. Route those concerns to `secret-scan-check` and security guidance.
3. When applicable, read database and security rules in `instructions/03-cqrs-database-testing-security.md` and current database state in `docs/project/PROJECT_STATUS.md`.
4. When Text-to-SQL views or read-only users are affected, also read `docs/project/ASSISTANT_TEXT_TO_SQL_READONLY_DB.md`.
5. Assess plan safety independently from authorization to create or execute a migration. Creation approval does not imply execution approval.
6. Inspect module ownership, generated operations, destructive changes, defaults and backfills, indexes and constraints, raw SQL scope, rollback implications, target database, privileges, and startup behavior.
7. Reject embedded credentials, real private connection strings, startup auto-migration, ownership leakage, unsafe data changes, or out-of-scope changes.
8. Define build, test, script inspection, and manual database verification without creating or executing a migration unless separately authorized.

## Result meanings

- `MIGRATION_PLAN_SAFETY: PASS` means the reviewed database-impacting proposal is safe within scope; `FAIL` means a concrete unsafe defect exists; `BLOCKED` means required evidence is unavailable; `NOT_APPLICABLE` means no database-impacting work exists.
- `MIGRATION_EXECUTION_APPROVAL: APPROVED` requires explicit authorization to execute; `NOT_APPROVED` means database work is applicable but execution authorization is absent; `NOT_APPLICABLE` means no execution is proposed or the Skill is not applicable.
- `MIGRATION_SAFETY` reflects the requested action. A safe plan may be `PASS` while execution approval is `NOT_APPLICABLE` or `NOT_APPROVED`; a requested execution without explicit execution approval is `BLOCKED`.

## Output contract

```text
Affected database:
Migration or schema files:
Approval evidence:
Data and rollback risks:
Verification required:
MIGRATION_PLAN_SAFETY: PASS | FAIL | BLOCKED | NOT_APPLICABLE
MIGRATION_EXECUTION_APPROVAL: APPROVED | NOT_APPROVED | NOT_APPLICABLE
MIGRATION_SAFETY: PASS | FAIL | BLOCKED | NOT_APPLICABLE
```

## Validation scenarios

- Positive: explicitly approved additive module-owned migration plan with reviewed operations -> plan safety `PASS`; execution is `APPROVED` only when separately authorized.
- Negative: API-key or credential-only concern with no target, ownership, permission, schema, migration, or SQL change -> do not trigger.
- Valid output: unsafe reviewed change -> `FAIL`; safe plan with no execution requested -> final `PASS` and execution approval `NOT_APPLICABLE`.
- Blocked: requested execution lacks explicit execution approval, or required files, ownership, target, or context are unavailable -> `BLOCKED`.
