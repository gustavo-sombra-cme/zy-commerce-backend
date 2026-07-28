# Skill Routing Defect Regression Report

## Test Scope

- Date: 2026-07-22
- Governing prompt: `docs/prompts/100-fix-ecommerce-skill-routing-and-loading-defects.md`
- Defect source: `docs/skills/testing/skill-routing-and-loading-test-report.md`
- Method: Skill Creator structural validation, source-of-truth consistency checks, and seven read-only forward-test scenarios evaluated against the repaired repository files.
- Runtime application behavior, migrations, commits, pushes, and pull requests were out of scope.
- The pre-existing `src/Api/Ecommerce.Api/appsettings.json` Text-to-SQL flag change was inspected only for scope isolation and was not modified.

## Overall Result

`PASS`

All seven focused regression scenarios passed after secondary workflow guidance and the Skill architecture catalog were aligned with the canonical Skill contracts. No unresolved routing, progressive-loading, or terminal-status defect remains in the tested scope.

## Defects Repaired

1. Replaced the malformed loading index with a closed, canonical, progressive-loading index that distinguishes always-loaded routing, conditional instructions, Skill metadata, Skill bodies, Skill references, and task evidence.
2. Aligned prompt logging across planning, approved execution, artifact-producing testing, documentation changes, Skill maintenance, findings-first review, and global workflow policy, while preserving `SKIP PROMPT LOG` and explanation-only exclusions.
3. Made push readiness consume `COMMIT_READINESS: PASS`, `VERIFICATION_STATUS: PASS`, and `SECRET_SCAN_STATUS: PASS` and fail closed when evidence is missing or non-passing.
4. Standardized secret scanning on `SECRET_SCAN_STATUS: PASS | BLOCKED`.
5. Limited migration-safety routing to database-impacting work and separated plan safety from migration execution approval.
6. Split verification into a dry-run classification contract and an executed contract using `VERIFICATION_STATUS: PASS | FAIL | BLOCKED`.
7. Made architecture-compliance code review load architecture guidance while keeping ADR and database guidance conditional on the actual diff.
8. Made commit readiness compose current branch, scope, review, verification, secret, applicable migration, prompt-log, and project-memory evidence without rerunning producer Skills.
9. Limited project-memory loading and updates to verified persistent state changes and affected memory-file purposes.
10. Removed the obsolete empty `.agents/skills/workflow/architecture-decision-check/` directory and its now-empty `.agents/skills/workflow/` parent.
11. Corrected confirmed documentation drift: Catalog/Auth migration counts, prompt 097 lifecycle status, implemented Catalog/Orders runtime sub-agent wording, and secondary workflow/status-contract references.

## Files Changed

### Skill entrypoints

- `.agents/skills/commit-readiness/SKILL.md`
- `.agents/skills/migration-safety-check/SKILL.md`
- `.agents/skills/project-memory-update/SKILL.md`
- `.agents/skills/prompt-log-writer/SKILL.md`
- `.agents/skills/push-readiness/SKILL.md`
- `.agents/skills/secret-scan-check/SKILL.md`
- `.agents/skills/verification-runner/SKILL.md`
- `.agents/skills/code-review-check/SKILL.md`

### Skill metadata

- `.agents/skills/commit-readiness/agents/openai.yaml`
- `.agents/skills/migration-safety-check/agents/openai.yaml`
- `.agents/skills/verification-runner/agents/openai.yaml`

### Routing, workflow, and project documentation

- `instructions/01-execution-and-planning.md`
- `instructions/04-documentation-and-memory.md`
- `instructions/05-completion.md`
- `instructions/06-loading-index.md`
- `docs/agents/workflow/planning-sub-agent.md`
- `docs/agents/workflow/documentation-sub-agent.md`
- `docs/project/PROMPT_TEMPLATE.md`
- `docs/project/AI_SKILLS_SUBAGENT_ARCHITECTURE.md`
- `docs/project/PROJECT_STATUS.md`
- `docs/project/AI_HANDOFF.md`
- `docs/project/NEXT_SESSION.md`

### Historical status, prompt log, and test evidence

- `docs/prompts/097-bounded-autonomous-catalog-agent-execution.md` — lifecycle status only.
- `docs/prompts/100-fix-ecommerce-skill-routing-and-loading-defects.md`
- `docs/skills/testing/skill-routing-defect-regression-report.md`

### Removed directories

- `.agents/skills/workflow/architecture-decision-check/` — verified empty.
- `.agents/skills/workflow/` — removed after its only child was removed and it became empty.

## Trigger Descriptions Before And After

| Skill | Before | After |
|---|---|---|
| `prompt-log-writer` | Metadata centered on approved execution despite a broader global rule | Explicitly covers repository planning, approved execution, artifact-producing testing, documentation changes, Skill maintenance, required review, and global workflow policy; excludes valid opt-out and explanation-only interactions |
| `push-readiness` | Checked branch/upstream/worktree/commits/approval without naming upstream evidence dependencies | Requires and consumes commit-readiness, executed-verification, secret-scan, branch, remote/upstream, outgoing-scope, worktree, approval, and blocker evidence |
| `secret-scan-check` | Trigger was already appropriate | Trigger remains before commit/push and for configuration or credential handling; output label is standardized |
| `migration-safety-check` | Applied broadly to connection strings and privileged database configuration | Applies only to database-impacting schema, migration, SQL, execution, permission, target, or ownership changes; explicitly excludes credential-only/API-key-only review |
| `verification-runner` | Applied after implementation and implied execution | Supports explicit dry-run planning and executed verification, with separate truthful contracts |
| `code-review-check` | Review trigger was appropriate but reference-loading rules were underspecified | Trigger remains findings-first/applicable pre-commit review; body now requires architecture instructions for explicit compliance and keeps ADR/database references conditional |
| `commit-readiness` | Required general verification/review/secret results but did not define the complete evidence composition | Requires current branch-start, scope, review, executed verification, secret scan, applicable migration, required prompt log, project-memory disposition, freshness, and blocker evidence |
| `project-memory-update` | Centered on completion of approved execution and loaded all four memory files | Triggers only for verified persistent state changes, including workflow/source-of-truth changes; excludes plans, hypotheses, unverified work, grammar-only edits, and temporary artifacts; loads only affected memory files |

## Output Contracts Before And After

| Skill | Before | After |
|---|---|---|
| `prompt-log-writer` | `PROMPT_LOG: PASS | BLOCKED` | Unchanged terminal label; applicability and lifecycle timing clarified |
| `push-readiness` | `PUSH_READINESS: PASS | BLOCKED` without explicit dependency fields | Same terminal label, with commit-readiness, verification, secret-scan, and unresolved-blocker fields |
| `secret-scan-check` | `SECRET_SCAN: PASS | BLOCKED` | `SECRET_SCAN_STATUS: PASS | BLOCKED` |
| `migration-safety-check` | `MIGRATION_SAFETY: PASS | FAIL | BLOCKED` | `MIGRATION_PLAN_SAFETY: PASS | FAIL | BLOCKED | NOT_APPLICABLE`, `MIGRATION_EXECUTION_APPROVAL: APPROVED | NOT_APPROVED | NOT_APPLICABLE`, and final `MIGRATION_SAFETY: PASS | FAIL | BLOCKED | NOT_APPLICABLE` |
| `verification-runner` | `VERIFICATION: PASS | BLOCKED` for both selection and execution | Dry run: `VERIFICATION_DRY_RUN: COMPLETE | BLOCKED`; executed run: `VERIFICATION_STATUS: PASS | FAIL | BLOCKED` |
| `code-review-check` | `CODE_REVIEW: PASS | BLOCKED` | Terminal label unchanged; conditional reference-loading behavior clarified |
| `commit-readiness` | `COMMIT_READINESS: PASS | BLOCKED` | Terminal label unchanged and exclusive; evidence/freshness fields made explicit |
| `project-memory-update` | `PROJECT_MEMORY_UPDATE: PASS | BLOCKED` | Terminal label unchanged; selective-file and non-applicability behavior clarified |

## Regression Scenarios

| # | Scenario | Expected routing and contract | Result |
|---|---|---|---|
| 1 | Routine Catalog planning | Invoke `prompt-log-writer`; architecture decision remains conditional; no execution Skills; `PROMPT_LOG: PASS`, then `PLAN_STATUS: PENDING_APPROVAL` | PASS |
| 2 | Documentation-only Skill documentation change | Invoke `prompt-log-writer` before edits and executed documentation verification after edits; skip architecture/migration when no such impact; `VERIFICATION_STATUS: PASS` only after checks run | PASS |
| 3 | Credential-only API key or connection-string exposure | Invoke `secret-scan-check`; do not invoke migration safety unless target, ownership, permissions, schema, migration, execution, or SQL behavior changes; exposure ends `SECRET_SCAN_STATUS: BLOCKED` | PASS |
| 4 | Explicit architecture-compliance code review | Invoke `code-review-check`; load `instructions/02-architecture-and-modules.md`; load only possibly governing ADRs; do not load Text-to-SQL/database guidance for an unrelated flag | PASS |
| 5 | Verification dry run | Classify restore, build, unit, integration, architecture, migration, manual API, security, frontend, documentation self-review, and diff validation as Required, Not required, Deferred, or Blocked; execute nothing; end `VERIFICATION_DRY_RUN: COMPLETE` | PASS |
| 6 | Commit readiness with missing evidence | Consume rather than reproduce `BRANCH_START`, scope, review, verification, secret, applicable migration, prompt-log, and memory evidence; any missing required item ends `COMMIT_READINESS: BLOCKED` | PASS |
| 7 | Push readiness with missing/non-passing evidence | Consume commit-readiness, executed-verification, and secret-scan results; any missing or non-passing result ends `PUSH_READINESS: BLOCKED`; no producer check is rerun | PASS |

### Scenario 1 — Planning log

- Skills considered: all ten canonical repository Skills.
- Skills invoked: `prompt-log-writer`.
- Skills skipped: `architecture-decision-check` for a routine non-architectural fixture; all execution, review, migration, memory, verification, commit, push, and secret Skills.
- Files loaded: `AGENT.md`; `instructions/06-loading-index.md`; prompt-log Skill; `instructions/01-execution-and-planning.md`; `instructions/04-documentation-and-memory.md`; `docs/project/PROMPT_TEMPLATE.md`; planning sub-agent guidance.
- Files skipped: ADRs, project memory, Catalog source/tests, unrelated instructions, other Skill bodies, build/test and Git mutation surfaces.
- Expected: prompt log required unless explicitly skipped; no execution; `PROMPT_LOG: PASS`, then `PLAN_STATUS: PENDING_APPROVAL`.
- Actual: matched expected routing and contracts. PASS.

### Scenario 2 — Documentation-only repository change

- Skills considered: all ten canonical repository Skills.
- Skills invoked: `prompt-log-writer`, then `verification-runner` in executed documentation-only mode.
- Skills skipped: architecture and migration for a fixture with no architecture/database impact; code review for ordinary documentation maintenance; lifecycle Skills not requested; project memory for a prose-only fixture with no persistent state change.
- Files loaded: `AGENT.md`; loading index; prompt-log and verification Skills; `instructions/04-documentation-and-memory.md`; `instructions/05-completion.md`; documentation sub-agent guidance; relevant prompt-template, AI handoff, and Skill-architecture sections.
- Files skipped: source/tests/config, ADRs, database guidance, migration Skill, unrelated Skill bodies, application build/test commands.
- Expected: `PROMPT_LOG: PASS` and, after actual documentation self-review/diff validation, `VERIFICATION_STATUS: PASS`.
- Actual: matched expected routing and contracts. PASS.

### Scenario 3 — Credential-only review

- Skills considered: all ten canonical repository Skills.
- Skills invoked: `secret-scan-check`.
- Skills skipped: `migration-safety-check` because database target, ownership, permission, schema, migration, execution, and SQL behavior did not change; all unrelated Skills.
- Files loaded: `AGENT.md`; loading index; secret-scan Skill; security instruction; security-review sub-agent guidance; relevant AI handoff secret-handling sections.
- Files skipped: migration Skill, schema/migration/SQL files, Text-to-SQL database guide, connection targets and values, other Skill bodies.
- Expected: confirmed exposure ends `SECRET_SCAN_STATUS: BLOCKED`; no migration status is emitted.
- Actual: matched expected routing and contracts. PASS.

### Scenario 4 — Architecture code review

- Skills considered: all ten canonical repository Skills.
- Skills invoked: `code-review-check`.
- Skills skipped: all other Skill bodies; architecture-decision Skill was not needed because the fixture requested compliance review rather than a new decision.
- Files loaded: `AGENT.md`; loading index; code-review Skill; `instructions/02-architecture-and-modules.md`; `docs/project/CODE_REVIEW.md`; prompt 100; defect report; current diff evidence.
- Files skipped: every ADR body because no possibly governing architectural decision was identified; database, migration, raw SQL, and Text-to-SQL guidance; unrelated application/test trees.
- Expected: architecture instruction loads; unrelated database guidance does not; current mixed worktree makes operational review `CODE_REVIEW: BLOCKED` without indicating a routing failure.
- Actual: routing PASS; review correctly reported the unrelated appsettings flag and mixed-scope worktree as blockers.

### Scenario 5 — Verification dry run

- Skills considered: all ten canonical repository Skills.
- Skills invoked: `verification-runner` only.
- Skills skipped: the other nine Skill bodies.
- Files loaded: verification Skill; `instructions/05-completion.md`; relevant verification sections in the prompt template and AI handoff.
- Files skipped: source/tests, architecture/database/API/frontend guidance not needed for the documentation/Skill fixture, and every executable verification action.
- Expected: every required category classified; no command executed; no `VERIFICATION_STATUS: PASS`; terminal `VERIFICATION_DRY_RUN: COMPLETE`.
- Actual: matched expected routing and contract. PASS.

### Scenario 6 — Commit readiness

- Skills considered: all ten canonical repository Skills.
- Skills invoked: `commit-readiness` only for evidence composition.
- Skills skipped: every producer Skill body, push readiness, source/tests, repository mutation commands.
- Files loaded: `AGENT.md`; commit-readiness Skill; relevant branch/commit sections of `instructions/01-execution-and-planning.md` and `instructions/05-completion.md`.
- Files skipped: branch-start, code-review, verification, secret-scan, migration, prompt-log, and memory producer bodies; actual Git/diff execution in the static missing-evidence fixture.
- Expected: missing any required evidence produces `COMMIT_READINESS: BLOCKED` without rerunning producers.
- Actual: matched expected fail-closed composition. PASS.

### Scenario 7 — Push readiness

- Skills considered: all ten canonical repository Skills.
- Skills invoked: `push-readiness` only for evidence composition.
- Skills skipped: commit-readiness, verification, secret-scan, and all other producer Skill bodies; push action itself.
- Files loaded: `AGENT.md`; push-readiness Skill; relevant push sections of `instructions/01-execution-and-planning.md` and `instructions/05-completion.md`.
- Files skipped: implementation/source/tests, producer Skill bodies, Git mutations, remote operations, and unrelated instructions.
- Expected: any missing/non-passing `COMMIT_READINESS`, `VERIFICATION_STATUS`, or `SECRET_SCAN_STATUS` evidence produces `PUSH_READINESS: BLOCKED` without rerunning producers.
- Actual: all missing/non-passing evidence fixtures matched the fail-closed contract. PASS.

## Progressive Loading Evidence

- Each fresh evaluator inspected Skill metadata before loading a body.
- Only triggered Skill bodies were loaded.
- Scenario 3 did not load the migration-safety body for credential-only scope.
- Scenario 4 loaded architecture instructions because compliance was explicit, loaded no ADR body because no governing architectural decision was identified, and loaded no database/Text-to-SQL guidance for the unrelated flag.
- Scenarios 6 and 7 loaded readiness bodies but did not load or rerun producer Skill bodies.
- The removed `workflow` directory no longer appears as a false Skill-discovery candidate.

## Contract Evidence

### Verification dry run

The dry-run contract covers:

- Restore
- Build
- Unit tests
- Integration tests
- Architecture tests
- Migration checks
- Manual API checks
- Security checks
- Frontend checks
- Documentation self-review
- Diff validation

It emits only `VERIFICATION_DRY_RUN: COMPLETE | BLOCKED`. It cannot emit `VERIFICATION_STATUS: PASS`.

### Commit readiness

Required evidence includes current-task `BRANCH_START: PASS`, `Scope: PASS`, `VERIFICATION_STATUS: PASS`, `SECRET_SCAN_STATUS: PASS`, applicable `CODE_REVIEW: PASS`, applicable `MIGRATION_SAFETY: PASS`, required `PROMPT_LOG: PASS`, and required `PROJECT_MEMORY_UPDATE: PASS`. Explicit `NOT_REQUIRED` or `NOT_APPLICABLE` rationales are required where allowed. Missing, stale, contradictory, failed, or blocked evidence produces `COMMIT_READINESS: BLOCKED`.

### Push readiness

Required composed evidence is exactly:

- `COMMIT_READINESS: PASS`
- `VERIFICATION_STATUS: PASS`
- `SECRET_SCAN_STATUS: PASS`

Missing or non-passing evidence produces `PUSH_READINESS: BLOCKED`.

## Structural And Documentation Validation

| Check | Result |
|---|---|
| Skill packages discovered | 10 |
| Equivalent Skill Creator structural validation errors | 0 |
| Non-Skill child directories under `.agents/skills/` | 0 |
| Missing `agents/openai.yaml` files | 0 |
| Broken concrete canonical Skill references | 0 |
| Unbalanced Markdown fences in affected files | 0 |
| Stale old routing/status phrases in active Skill and workflow sources | 0 |
| High-confidence secret-pattern matches in affected files | 0 |
| `git diff --check` | PASS |
| Prompt 100 prefix collisions | 0 |

The Skill Creator `quick_validate.py` script could not run because neither `python` nor `py` is installed. A PowerShell equivalent applied the script's frontmatter, allowed-key, name-format, name-length, description, and package-presence checks to all ten Skills, plus `openai.yaml` presence and required UI-field checks.

## Verification Selection

- Documentation self-review: required and passed.
- Diff validation: required and passed.
- Skill structural validation: required and passed with the documented equivalent validator.
- Focused forward tests: required; 7 of 7 passed.
- Secret scan: required and passed for the affected files.
- .NET restore/build/unit/integration/architecture tests: not required because the approved scope changes only repository Skills, routing instructions, workflow documentation, prompt logs, and test documentation.
- Migration checks and manual API checks: not required because no database, migration, SQL, API, authentication, or runtime behavior was changed.
- Frontend checks: not required because no frontend or public API contract was changed.

## Scope Isolation

The unrelated `src/Api/Ecommerce.Api/appsettings.json` diff still changes `Assistant:TextToSql:Enabled` from `false` to `true`. It predates this repair task, remains outside its approved scope, was not edited, and has no runtime verification from this task. It must not be included in any future scoped commit for these workflow repairs.

## Remaining Defects

No remaining Skill-routing defects were found in the seven tested scenarios.

REGRESSION_STATUS: PASS
