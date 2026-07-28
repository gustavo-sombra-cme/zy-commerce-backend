# Full Ecommerce Skill Routing Regression Report

Date: 2026-07-22

## Scope And Method

This report reruns all twelve routing and conditional-loading scenarios after the focused repairs. The canonical root is `.agents/skills/`. Metadata was inspected for all ten Skills before any scenario body; bodies and references were loaded only after a scenario trigger applied. Results distinguish routing success from an expected `BLOCKED` operational status. No existing Skill, instruction, runtime, configuration, migration, or prior report was modified.

The current dirty worktree was preserved as required scenario evidence. The only writes authorized by prompt 101 are this report and `docs/prompts/101-full-ecommerce-skill-routing-regression-test.md`.

## Skills Discovered

| Skill | Entrypoint | Metadata | Bundled reference |
|---|---|---|---|
| architecture-decision-check | `.agents/skills/architecture-decision-check/SKILL.md` | `agents/openai.yaml` | `references/adr-review-checklist.md` |
| branch-start-check | `.agents/skills/branch-start-check/SKILL.md` | `agents/openai.yaml` | None |
| code-review-check | `.agents/skills/code-review-check/SKILL.md` | `agents/openai.yaml` | None |
| commit-readiness | `.agents/skills/commit-readiness/SKILL.md` | `agents/openai.yaml` | None |
| migration-safety-check | `.agents/skills/migration-safety-check/SKILL.md` | `agents/openai.yaml` | None |
| project-memory-update | `.agents/skills/project-memory-update/SKILL.md` | `agents/openai.yaml` | None |
| prompt-log-writer | `.agents/skills/prompt-log-writer/SKILL.md` | `agents/openai.yaml` | None |
| push-readiness | `.agents/skills/push-readiness/SKILL.md` | `agents/openai.yaml` | None |
| secret-scan-check | `.agents/skills/secret-scan-check/SKILL.md` | `agents/openai.yaml` | None |
| verification-runner | `.agents/skills/verification-runner/SKILL.md` | `agents/openai.yaml` | None |

Inventory result: ten `SKILL.md` files, all under the canonical root, with no extra discoverable or duplicate Skill root.

## Baseline Validation

| Check | Result | Evidence |
|---|---|---|
| Skill count | PASS | 10 directories and 10 entrypoints |
| Directory naming | PASS | All names use valid kebab case and match frontmatter `name` |
| Frontmatter | PASS | Required name/description, allowed keys only, valid lengths |
| `agents/openai.yaml` | PASS | All ten present with display name, short description, and `$skill-name` default prompt |
| Trigger descriptions | PASS | Positive scope and exclusions are explicit |
| Output contracts | PASS | All canonical labels found exactly in their owning Skills |
| Canonical paths | PASS | No `SKILL.md` exists outside `.agents/skills/` |
| Conditional reference | PASS | ADR checklist resolves and is loaded only by architecture routing |
| `HintId`/unfinished placeholders | PASS | No `HintId`, TODO, TBD, or unresolved placeholder marker |
| Broken references | PASS | Zero broken local Skill references |
| Balanced Markdown fences | PASS | Zero affected-file fence errors |
| Secret exposure | PASS | Zero high-confidence secret matches across changed and untracked files; values were not reported |
| `git diff --check` | PASS | No whitespace errors |

The official Python validator is unavailable because the installed `python` command is a nonfunctional application alias and `py` is absent. The existing equivalent PowerShell validation was used and returned zero errors.

## Scenario Traces

### SCENARIO A — Routine Catalog planning

REQUEST:
`Plan the next Catalog feature to update product descriptions using the existing architecture and API conventions. Do not execute.`

SKILLS CONSIDERED:
All ten metadata descriptions. `prompt-log-writer` was applicable; architecture materiality was considered and did not require its Skill because the request stays within implemented conventions.

SKILLS INVOKED:
- `prompt-log-writer` — governed repository planning.

SKILLS SKIPPED:
- `architecture-decision-check` — no new durable decision; description updates already exist.
- The remaining eight — no execution start, review, database change, verified completion, verification, commit, push, or credential scope.

INSTRUCTION FILES LOADED:
- `AGENT.md`, `instructions/06-loading-index.md`, `instructions/01-execution-and-planning.md`, `instructions/04-documentation-and-memory.md`.
- `docs/project/PROMPT_TEMPLATE.md`, planning workflow guidance, relevant `PROJECT_STATUS.md` sections.
- Prompts 053/054 and targeted Product/handler/validator/controller evidence confirming the feature already exists.

INSTRUCTION FILES SKIPPED:
- Architecture/database/completion instructions, ADRs, unrelated memory, Auth/Orders/assistant source, migrations, build/test execution.

SKILL REFERENCES LOADED:
- None.

OUTPUT CONTRACT:
```text
Prompt log: required before a real plan; no per-scenario artifact created by this test
Number: not allocated
Status: PLANNED would apply
PROMPT_LOG: BLOCKED
PLAN_STATUS: PENDING_APPROVAL
```

RESULT:
PASS — prompt logging triggered, the duplicate feature was identified, and no execution Skill ran.

CONTEXT_LOADING:
EFFICIENT — one Skill body, no ADR reference, targeted feature evidence only.

### SCENARIO B — Material architecture change

REQUEST:
`Plan replacing the deterministic Catalog assistant with a bounded autonomous LLM sub-agent that selects from allowlisted Catalog tools.`

SKILLS CONSIDERED:
All ten. Runtime AI autonomy directly triggered architecture review; planning triggered prompt logging.

SKILLS INVOKED:
- `architecture-decision-check`.
- `prompt-log-writer`.

SKILLS SKIPPED:
- The other eight — no execution, migration, review diff, verified completion, verification, commit, push, or credential action.

INSTRUCTION FILES LOADED:
- Routing baseline; instructions 00–04; prompt template and planning profile.
- Relevant project memory and assistant-architecture sections.
- ADR identifier inventory, ADR-007 whole, prompts 096/097, and targeted bounded-agent implementation evidence.

INSTRUCTION FILES SKIPPED:
- Completion actions, Text-to-SQL database guidance, migrations, provider configuration values, unrelated modules, command execution.

SKILL REFERENCES LOADED:
- `.agents/skills/architecture-decision-check/references/adr-review-checklist.md`.

OUTPUT CONTRACT:
```text
ADR_ACTION: NOT_REQUIRED
Affected areas: runtime AI autonomy, API-layer orchestration, Catalog tool authority
Existing ADRs: ADR-007 owns and accepts the exact design
Repository evidence: ADR-007, prompts 096/097, CatalogAssistantSubAgent, CatalogAgentToolRegistry
Rationale: the requested replacement is already implemented; no new delta exists
Next step: identify a material change to ADR-007 before requesting CREATE or UPDATE

PROMPT_LOG: BLOCKED
```

RESULT:
PASS — architecture and prompt logging triggered; execution Skills stayed skipped.

CONTEXT_LOADING:
ACCEPTABLE — broader context was justified by material runtime autonomy.

### SCENARIO C — Approved implementation start

REQUEST:
`APPROVED: EXECUTE the already approved Catalog update plan. Before changing files, perform all required execution-start checks. Do not modify files during this test.`

SKILLS CONSIDERED:
All ten; approved execution directly triggered branch start and prompt logging.

SKILLS INVOKED:
- `branch-start-check`.
- `prompt-log-writer`.

SKILLS SKIPPED:
- Architecture and all completion/producer/readiness Skills because implementation did not start.

INSTRUCTION FILES LOADED:
- Routing baseline; instructions 01, 04, and 05; prompt template; execution workflow guidance.
- Read-only path, branch, worktree, upstream, and prompt-number evidence.

INSTRUCTION FILES SKIPPED:
- Source/tests, project memory, ADRs, database/configuration contents, build/test/EF actions.

SKILL REFERENCES LOADED:
- None.

OUTPUT CONTRACT:
```text
Repository: C:/ZippyYum/Learning/zy-commerce-backend
Starting branch: feature/backend-bounded-autonomous-catalog-agent
Worktree: dirty and intentionally preserved
Main freshness: not established; no fetch/switch allowed in this test state
Task branch: no dedicated Catalog-update branch created
BRANCH_START: BLOCKED

Prompt log: required; per-scenario write prohibited
Number: not allocated
Status: APPROVED would apply
PROMPT_LOG: BLOCKED
```

RESULT:
BLOCKED — expected safe stop; routing PASS.

CONTEXT_LOADING:
EFFICIENT — two Skill bodies and Git metadata only.

### SCENARIO D — Migration proposal

REQUEST:
`Plan adding a nullable Product subtitle column with an Entity Framework Core migration. Do not create or apply the migration.`

SKILLS CONSIDERED:
All ten; architecture, migration safety, and planning log were applicable.

SKILLS INVOKED:
- `architecture-decision-check`.
- `migration-safety-check`.
- `prompt-log-writer`.

SKILLS SKIPPED:
- All execution, review, memory, verification, commit, push, and secret Skills.

INSTRUCTION FILES LOADED:
- Routing baseline; instructions 01–04; prompt template and planning guidance.
- Relevant Catalog database state, Product aggregate/mapping/snapshot, ADR identifiers and ADR-001.

INSTRUCTION FILES SKIPPED:
- Completion actions, Text-to-SQL database guide, Auth/Orders, configuration values, EF/build/test execution.

SKILL REFERENCES LOADED:
- ADR review checklist.

OUTPUT CONTRACT:
```text
ADR_ACTION: NOT_REQUIRED
Affected database: Catalog, catalog.Products
Migration or schema files: proposal only; none created
Approval evidence: planning only; creation/application not authorized
Data and rollback risks: nullable addition avoids backfill; type/length and read-model/API alignment remain unspecified; rollback would discard subtitle data
Verification required: inspect generated operations/snapshot/script, build/tests, and manual database verification after separate approval
MIGRATION_PLAN_SAFETY: BLOCKED
MIGRATION_EXECUTION_APPROVAL: NOT_APPLICABLE
MIGRATION_SAFETY: BLOCKED
PROMPT_LOG: BLOCKED
```

RESULT:
PASS — applicability and separate approval concepts were correct; no migration was created or applied.

CONTEXT_LOADING:
ACCEPTABLE — one conditional reference and targeted persistence evidence.

### SCENARIO E — Architecture and security code review

REQUEST:
`Review the current working-tree diff for correctness, security, architecture compliance, missing tests, and regressions. Do not modify files.`

SKILLS CONSIDERED:
All ten; findings-first review, explicit architecture compliance, configuration presence, and global review logging were evaluated.

SKILLS INVOKED:
- `code-review-check`.
- `secret-scan-check`.
- `prompt-log-writer`.

SKILLS SKIPPED:
- `architecture-decision-check` — intended workflow changes do not change runtime/module architecture.
- `migration-safety-check` — the unrelated configuration path does not establish schema, migration, SQL, target, ownership, or permission change.
- Producer/readiness/memory Skills — not requested and not rerun.

INSTRUCTION FILES LOADED:
- Routing baseline; instructions 02–05.
- `docs/project/CODE_REVIEW.md`, code-review/security workflow profiles, relevant AI handoff security warnings.
- Complete changed/untracked path inventory, diff statistics, whitespace check, and redacted secret-pattern scan.

INSTRUCTION FILES SKIPPED:
- Text-to-SQL database guide, migrations/schema/SQL, ADR bodies, unrelated runtime source/tests, configuration values.

SKILL REFERENCES LOADED:
- None.

OUTPUT CONTRACT:
```text
Findings:
- HIGH: the working tree contains an unrelated pre-existing runtime configuration change outside the Skill/report scope.
- MEDIUM: the worktree combines prior normalization, repair, regression-test artifacts, and the unrelated configuration change.
Open questions: eventual commit scope and separate authorization/verification for the unrelated configuration change
Verification evidence: structural validation and git diff --check pass; no runtime verification applies to this report-only test
CODE_REVIEW: BLOCKED
Commit blocked: YES
Required follow-up: keep the unrelated configuration change outside any workflow-report commit

Files checked: complete changed/untracked file set using non-disclosing pattern checks
Findings: no high-confidence credential detected
Redaction required: NO
SECRET_SCAN_STATUS: PASS

PROMPT_LOG: BLOCKED
```

RESULT:
BLOCKED — operationally correct; architecture instructions loaded and unrelated database guidance remained skipped.

CONTEXT_LOADING:
EFFICIENT — three applicable bodies; no ADR or migration over-read.

### SCENARIO F — Project-memory update

REQUEST:
`Assume an approved Catalog feature was implemented and verified successfully. Determine which project-memory files must be updated. Do not modify them.`

SKILLS CONSIDERED:
All ten; only verified persistent-state memory targeting applied.

SKILLS INVOKED:
- `project-memory-update`.

SKILLS SKIPPED:
- The other nine; verification was stipulated and no architecture/database/review/commit/push action was requested.

INSTRUCTION FILES LOADED:
- Routing baseline and `instructions/04-documentation-and-memory.md`.
- `PROJECT_STATUS.md` and `NEXT_SESSION.md` purposes/current sections.

INSTRUCTION FILES SKIPPED:
- `AI_HANDOFF.md` and `ROADMAP.md` contents because the generic premise supplies no new constraint, gotcha, or roadmap item.
- Source/tests, ADRs, prompts, API contracts, migration references, other Skill bodies.

SKILL REFERENCES LOADED:
- None.

OUTPUT CONTRACT:
```text
Memory files updated: none — dry-run determination only
Facts recorded: none
Files unchanged and rationale:
- PROJECT_STATUS.md: would require the verified feature/API/database/test facts
- NEXT_SESSION.md: would require last-completed/current-state facts
- AI_HANDOFF.md: conditional on new constraints or gotchas
- ROADMAP.md: conditional on a tracked milestone or candidate changing state
PROJECT_MEMORY_UPDATE: PASS
```

RESULT:
PASS — only verified-state routing applied and memory content was selectively loaded.

CONTEXT_LOADING:
EFFICIENT — one Skill and two selected memory files.

### SCENARIO G — Verification dry run

REQUEST:
`Assume an approved API feature has been implemented. Determine and report the required verification commands without executing them.`

SKILLS CONSIDERED:
All ten; only verification planning applied.

SKILLS INVOKED:
- `verification-runner`.

SKILLS SKIPPED:
- All producer, readiness, architecture, database, memory, prompt-log, and secret Skills.

INSTRUCTION FILES LOADED:
- Routing baseline; instructions 03 and 05; prompt-template verification defaults; AI handoff verification guidance.

INSTRUCTION FILES SKIPPED:
- Source/test contents, migrations, ADRs, unrelated memory, database targets, configuration values, other Skill bodies.

SKILL REFERENCES LOADED:
- None.

OUTPUT CONTRACT:
```text
Mode: dry-run
Verification plan:
- Restore: Required — dotnet restore Ecommerce.sln
- Build: Required — dotnet build Ecommerce.sln
- Unit tests: Required — dotnet test Ecommerce.sln includes unit projects
- Integration tests: Deferred — exact feature and integration project coverage were not supplied
- Architecture tests: Required — solution test plus targeted architecture rerun when needed
- Migration checks: Deferred — changed files do not establish whether database impact exists
- Manual API checks: Required — exact route/auth/status checks deferred until feature details are supplied
- Security checks: Required — authorization, input, logging, and secret review
- Frontend checks: Deferred — public contract impact is unspecified
- Documentation self-review: Required — verify behavior documentation
- Diff validation: Required — scope review and git diff --check
VERIFICATION_DRY_RUN: COMPLETE
```

RESULT:
PASS — no command ran and no `VERIFICATION_STATUS: PASS` was emitted.

CONTEXT_LOADING:
EFFICIENT — one Skill and verification-only guidance.

### SCENARIO H — Commit readiness

REQUEST:
`Assess whether the current changes are ready for a local commit. Do not commit.`

SKILLS CONSIDERED:
All ten; commit readiness applied while producer Skills remained evidence-only.

SKILLS INVOKED:
- `commit-readiness`.

SKILLS SKIPPED:
- Branch-start, code-review, verification, secret-scan, migration, prompt-log, and memory producer Skills were not rerun.
- Push readiness and architecture were unrelated.

INSTRUCTION FILES LOADED:
- Routing baseline; relevant commit/branch sections of instructions 01 and 05; current Git state.

INSTRUCTION FILES SKIPPED:
- Producer Skill bodies, source/tests, database/ADR guidance, command execution.

SKILL REFERENCES LOADED:
- None.

OUTPUT CONTRACT:
```text
Branch: feature/backend-bounded-autonomous-catalog-agent
Branch-start evidence: BRANCH_START: BLOCKED
Changed files: mixed prior task scopes plus prompt 101
Scope: BLOCKED
Verification: missing current VERIFICATION_STATUS: PASS
Code review: CODE_REVIEW: BLOCKED
Secret scan: prior/in-progress evidence is not a final current-task readiness result
Migration safety: NOT_APPLICABLE
Prompt log: current log not yet finalized
Project memory: NOT_REQUIRED — regression artifacts do not change persistent implementation state
Unresolved blockers: branch start, mixed scope, review, verification, final log evidence
Generated artifacts: none detected
COMMIT_READINESS: BLOCKED
```

RESULT:
BLOCKED — expected; no producer Skill was rerun and no commit occurred.

CONTEXT_LOADING:
HIGHLY_EFFICIENT — one readiness body and existing evidence only.

### SCENARIO I — Push readiness

REQUEST:
`Assess whether the current branch is ready to push to its configured remote. Do not push.`

SKILLS CONSIDERED:
All ten; only push readiness applied, with required producer results treated as inputs.

SKILLS INVOKED:
- `push-readiness`.

SKILLS SKIPPED:
- `commit-readiness`, `verification-runner`, and `secret-scan-check` were not rerun.
- All other Skills were unrelated.

INSTRUCTION FILES LOADED:
- Routing baseline; relevant push sections of instructions 01 and 05; branch/upstream/outgoing-commit/worktree metadata.

INSTRUCTION FILES SKIPPED:
- Producer bodies, source/tests/configuration values, migrations, ADRs, broader memory, remote mutation.

SKILL REFERENCES LOADED:
- None.

OUTPUT CONTRACT:
```text
Branch: feature/backend-bounded-autonomous-catalog-agent
Upstream: origin/feature/backend-bounded-autonomous-catalog-agent
Outgoing commits: none
Worktree: dirty
Approval: absent
Commit readiness evidence: COMMIT_READINESS: BLOCKED
Verification evidence: missing VERIFICATION_STATUS: PASS
Secret scan evidence: missing final current-task SECRET_SCAN_STATUS: PASS
Unresolved blockers: dirty worktree, no outgoing commit, missing evidence, no push approval
PUSH_READINESS: BLOCKED
```

RESULT:
BLOCKED — expected; no producer rerun or push occurred.

CONTEXT_LOADING:
HIGHLY_EFFICIENT — one readiness body and Git metadata only.

### SCENARIO J — Secret-handling review

REQUEST:
`Review a proposed change that adds an API key and database connection string to appsettings.json.`

SKILLS CONSIDERED:
All ten; findings-first review, secret handling, and review logging applied. Migration applicability was evaluated from metadata and rejected.

SKILLS INVOKED:
- `code-review-check`.
- `secret-scan-check`.
- `prompt-log-writer`.

SKILLS SKIPPED:
- `migration-safety-check` — credential storage alone does not change database target, ownership, permission, schema, migration, execution, or SQL behavior.
- `architecture-decision-check` — this is an unsafe configuration proposal, not a new durable design.
- Other lifecycle/readiness Skills.

INSTRUCTION FILES LOADED:
- Routing baseline; instructions 03–05; code-review and security workflow guidance; relevant AI handoff secret rules.

INSTRUCTION FILES SKIPPED:
- Actual configuration values, migration/schema/SQL files, Text-to-SQL database guide, ADRs, runtime source/tests.

SKILL REFERENCES LOADED:
- None.

OUTPUT CONTRACT:
```text
Findings: BLOCKER — committed API keys and credential-bearing private connection strings violate repository security rules
Open questions: whether values are placeholders and which non-committed secret provider will be used
Verification evidence: proposal only; no safe complete diff
CODE_REVIEW: BLOCKED
Commit blocked: YES
Required follow-up: use non-committed secret resolution and provide a redacted diff

Files checked: proposal description only
Findings: proposed secret-bearing values are disallowed
Redaction required: YES
SECRET_SCAN_STATUS: BLOCKED

PROMPT_LOG: BLOCKED
```

RESULT:
BLOCKED — correct security outcome; migration safety did not trigger.

CONTEXT_LOADING:
EFFICIENT — three applicable bodies, no database/ADR over-read.

### SCENARIO K — Negative grammar-only trigger

REQUEST:
`Fix the grammar in a README paragraph without changing technical meaning.`

SKILLS CONSIDERED:
All ten; documentation prompt logging applied. Code review and project memory were excluded by their grammar-only rules.

SKILLS INVOKED:
- `prompt-log-writer`.

SKILLS SKIPPED:
- The other nine Skills; no approved execution, technical behavior, verified state, review, database, verification, commit, or push action.

INSTRUCTION FILES LOADED:
- Routing baseline; instructions 01 and 04; `README.md`.

INSTRUCTION FILES SKIPPED:
- Architecture/database/completion guidance, memory, ADRs, source/tests/configuration, every other Skill body.

SKILL REFERENCES LOADED:
- None.

OUTPUT CONTRACT:
```text
Prompt log: required for a real repository documentation change; no per-scenario artifact created
Number: not allocated
Status: PLANNED would apply until execution approval
PROMPT_LOG: BLOCKED
EXECUTION: BLOCKED — APPROVED: EXECUTE is absent
```

RESULT:
BLOCKED — correct execution lock; prompt logging triggered and technical Skills stayed skipped.

CONTEXT_LOADING:
HIGHLY_EFFICIENT — one Skill and the tiny target document.

### SCENARIO L — Ambiguous architecture request

REQUEST:
`Change the architecture and deploy it.`

SKILLS CONSIDERED:
All ten; architecture/deployment materiality applied, while no authorized execution, deployment, commit, or push stage existed.

SKILLS INVOKED:
- `architecture-decision-check`.

SKILLS SKIPPED:
- The other nine, including prompt logging because the vague unapproved execution request produced no plan or repository artifact.

INSTRUCTION FILES LOADED:
- Routing baseline; instructions 00–05 under high-risk/unclear routing.
- Relevant current architecture, handoff, roadmap, and resume-guide sections.
- ADR filename/subject inventory.

INSTRUCTION FILES SKIPPED:
- Source/tests/configuration, deployment manifests/cloud state, full ADR bodies, migrations, build/test/deployment actions.

SKILL REFERENCES LOADED:
- ADR review checklist.

OUTPUT CONTRACT:
```text
ADR_ACTION: BLOCKED
Affected areas: unspecified architecture and deployment topology
Existing ADRs: ADR-001 through ADR-007 searched; no owner can be selected
Repository evidence: current strategy is a Clean Architecture modular monolith; material topology changes require explicit design and approval
Rationale: affected modules, desired design, environment, constraints, migration/rollback plan, and authorization are absent
Next step: provide a scoped architecture proposal and deployment target, then obtain explicit execution/deployment approval
```

RESULT:
BLOCKED — expected safe response; no deployment or external action occurred.

CONTEXT_LOADING:
ACCEPTABLE — broad instruction/memory loading was justified by a vague high-risk request.

## Skill Interaction Matrix

| Scenario | Skill | Expected | Actual | Result |
|---|---|---|---|---|
| A | architecture-decision-check | Skip | Skip | PASS |
| A | branch-start-check | Skip | Skip | PASS |
| A | code-review-check | Skip | Skip | PASS |
| A | commit-readiness | Skip | Skip | PASS |
| A | migration-safety-check | Skip | Skip | PASS |
| A | project-memory-update | Skip | Skip | PASS |
| A | prompt-log-writer | Invoke | Invoke | PASS |
| A | push-readiness | Skip | Skip | PASS |
| A | secret-scan-check | Skip | Skip | PASS |
| A | verification-runner | Skip | Skip | PASS |
| B | architecture-decision-check | Invoke | Invoke | PASS |
| B | branch-start-check | Skip | Skip | PASS |
| B | code-review-check | Skip | Skip | PASS |
| B | commit-readiness | Skip | Skip | PASS |
| B | migration-safety-check | Skip | Skip | PASS |
| B | project-memory-update | Skip | Skip | PASS |
| B | prompt-log-writer | Invoke | Invoke | PASS |
| B | push-readiness | Skip | Skip | PASS |
| B | secret-scan-check | Skip | Skip | PASS |
| B | verification-runner | Skip | Skip | PASS |
| C | architecture-decision-check | Skip | Skip | PASS |
| C | branch-start-check | Invoke | Invoke | PASS |
| C | code-review-check | Skip | Skip | PASS |
| C | commit-readiness | Skip | Skip | PASS |
| C | migration-safety-check | Skip | Skip | PASS |
| C | project-memory-update | Skip | Skip | PASS |
| C | prompt-log-writer | Invoke | Invoke | PASS |
| C | push-readiness | Skip | Skip | PASS |
| C | secret-scan-check | Skip | Skip | PASS |
| C | verification-runner | Skip | Skip | PASS |
| D | architecture-decision-check | Invoke | Invoke | PASS |
| D | branch-start-check | Skip | Skip | PASS |
| D | code-review-check | Skip | Skip | PASS |
| D | commit-readiness | Skip | Skip | PASS |
| D | migration-safety-check | Invoke | Invoke | PASS |
| D | project-memory-update | Skip | Skip | PASS |
| D | prompt-log-writer | Invoke | Invoke | PASS |
| D | push-readiness | Skip | Skip | PASS |
| D | secret-scan-check | Skip | Skip | PASS |
| D | verification-runner | Skip | Skip | PASS |
| E | architecture-decision-check | Skip | Skip | PASS |
| E | branch-start-check | Skip | Skip | PASS |
| E | code-review-check | Invoke | Invoke | PASS |
| E | commit-readiness | Skip | Skip | PASS |
| E | migration-safety-check | Skip | Skip | PASS |
| E | project-memory-update | Skip | Skip | PASS |
| E | prompt-log-writer | Invoke | Invoke | PASS |
| E | push-readiness | Skip | Skip | PASS |
| E | secret-scan-check | Invoke | Invoke | PASS |
| E | verification-runner | Skip | Skip | PASS |
| F | architecture-decision-check | Skip | Skip | PASS |
| F | branch-start-check | Skip | Skip | PASS |
| F | code-review-check | Skip | Skip | PASS |
| F | commit-readiness | Skip | Skip | PASS |
| F | migration-safety-check | Skip | Skip | PASS |
| F | project-memory-update | Invoke | Invoke | PASS |
| F | prompt-log-writer | Skip | Skip | PASS |
| F | push-readiness | Skip | Skip | PASS |
| F | secret-scan-check | Skip | Skip | PASS |
| F | verification-runner | Skip | Skip | PASS |
| G | architecture-decision-check | Skip | Skip | PASS |
| G | branch-start-check | Skip | Skip | PASS |
| G | code-review-check | Skip | Skip | PASS |
| G | commit-readiness | Skip | Skip | PASS |
| G | migration-safety-check | Skip | Skip | PASS |
| G | project-memory-update | Skip | Skip | PASS |
| G | prompt-log-writer | Skip | Skip | PASS |
| G | push-readiness | Skip | Skip | PASS |
| G | secret-scan-check | Skip | Skip | PASS |
| G | verification-runner | Invoke | Invoke | PASS |
| H | architecture-decision-check | Skip | Skip | PASS |
| H | branch-start-check | Skip | Skip | PASS |
| H | code-review-check | Skip | Skip | PASS |
| H | commit-readiness | Invoke | Invoke | PASS |
| H | migration-safety-check | Skip | Skip | PASS |
| H | project-memory-update | Skip | Skip | PASS |
| H | prompt-log-writer | Skip | Skip | PASS |
| H | push-readiness | Skip | Skip | PASS |
| H | secret-scan-check | Skip | Skip | PASS |
| H | verification-runner | Skip | Skip | PASS |
| I | architecture-decision-check | Skip | Skip | PASS |
| I | branch-start-check | Skip | Skip | PASS |
| I | code-review-check | Skip | Skip | PASS |
| I | commit-readiness | Skip | Skip | PASS |
| I | migration-safety-check | Skip | Skip | PASS |
| I | project-memory-update | Skip | Skip | PASS |
| I | prompt-log-writer | Skip | Skip | PASS |
| I | push-readiness | Invoke | Invoke | PASS |
| I | secret-scan-check | Skip | Skip | PASS |
| I | verification-runner | Skip | Skip | PASS |
| J | architecture-decision-check | Skip | Skip | PASS |
| J | branch-start-check | Skip | Skip | PASS |
| J | code-review-check | Invoke | Invoke | PASS |
| J | commit-readiness | Skip | Skip | PASS |
| J | migration-safety-check | Skip | Skip | PASS |
| J | project-memory-update | Skip | Skip | PASS |
| J | prompt-log-writer | Invoke | Invoke | PASS |
| J | push-readiness | Skip | Skip | PASS |
| J | secret-scan-check | Invoke | Invoke | PASS |
| J | verification-runner | Skip | Skip | PASS |
| K | architecture-decision-check | Skip | Skip | PASS |
| K | branch-start-check | Skip | Skip | PASS |
| K | code-review-check | Skip | Skip | PASS |
| K | commit-readiness | Skip | Skip | PASS |
| K | migration-safety-check | Skip | Skip | PASS |
| K | project-memory-update | Skip | Skip | PASS |
| K | prompt-log-writer | Invoke | Invoke | PASS |
| K | push-readiness | Skip | Skip | PASS |
| K | secret-scan-check | Skip | Skip | PASS |
| K | verification-runner | Skip | Skip | PASS |
| L | architecture-decision-check | Invoke | Invoke | PASS |
| L | branch-start-check | Skip | Skip | PASS |
| L | code-review-check | Skip | Skip | PASS |
| L | commit-readiness | Skip | Skip | PASS |
| L | migration-safety-check | Skip | Skip | PASS |
| L | project-memory-update | Skip | Skip | PASS |
| L | prompt-log-writer | Skip | Skip | PASS |
| L | push-readiness | Skip | Skip | PASS |
| L | secret-scan-check | Skip | Skip | PASS |
| L | verification-runner | Skip | Skip | PASS |

Matrix total: 120 scenario/Skill combinations; 120 PASS, 0 FAIL.

## Instruction-Loading Matrix

`Load` means semantic inspection for the scenario. Metadata inventory and filename discovery alone are not counted as semantic loading.

| Scenario | File | Expected | Actual | Reason | Result |
|---|---|---|---|---|---|
| A | `AGENT.md` | Load | Load | Always-loaded router | PASS |
| A | `instructions/06-loading-index.md` | Load | Load | Conditional-loading baseline required by test | PASS |
| A | `instructions/00`–`05` | Load 01,04; skip 00,02,03,05 | Load 01,04; skip 00,02,03,05 | Routine planning and prompt logging; template carries existing architecture defaults | PASS |
| A | `docs/project/PROMPT_TEMPLATE.md` | Load | Load | Plan contract | PASS |
| A | Memory: `PROJECT_STATUS`, `AI_HANDOFF`, `ROADMAP`, `NEXT_SESSION` | Load PROJECT_STATUS; skip others | Load PROJECT_STATUS; skip others | Confirm implemented Catalog capability without loading memory for reporting | PASS |
| A | `docs/decisions/ADR-*.md` | Skip | Skip | No material architecture decision | PASS |
| A | ADR checklist reference | Skip | Skip | Architecture Skill did not trigger | PASS |
| A | `docs/project/CODE_REVIEW.md` | Skip | Skip | No review diff | PASS |
| A | `ASSISTANT_TEXT_TO_SQL_READONLY_DB.md` | Skip | Skip | No Text-to-SQL impact | PASS |
| A | Workflow profiles | Load planning; skip others | Load planning; skip others | Planning responsibility only | PASS |
| A | `README.md` | Skip | Skip | Not the target | PASS |
| A | Target evidence | Load prompts 053/054 and targeted Catalog update files | Loaded as expected | Establish feature already exists | PASS |
| B | `AGENT.md` | Load | Load | Always-loaded router | PASS |
| B | `instructions/06-loading-index.md` | Load | Load | Conditional-loading baseline | PASS |
| B | `instructions/00`–`05` | Load 00–04; skip 05 | Load 00–04; skip 05 | Material AI autonomy planning, architecture, safety, ADR/log rules | PASS |
| B | `docs/project/PROMPT_TEMPLATE.md` | Load | Load | Plan contract | PASS |
| B | Memory files | Load relevant sections of all four | Loaded relevant sections | Current assistant architecture and future direction | PASS |
| B | `docs/decisions/ADR-*.md` | Load identifiers; ADR-007 whole | Loaded as expected | Existing owner search | PASS |
| B | ADR checklist reference | Load | Load | Material runtime autonomy | PASS |
| B | `docs/project/CODE_REVIEW.md` | Skip | Skip | Planning, not findings-first review | PASS |
| B | Text-to-SQL database guide | Skip | Skip | No database-view/user change | PASS |
| B | Workflow profiles | Load planning; skip others | Load planning; skip others | Planning responsibility | PASS |
| B | `README.md` | Skip | Skip | Unrelated | PASS |
| B | Target evidence | Load prompts 096/097 and bounded-agent source | Loaded targeted files/sections | Confirm accepted and implemented design | PASS |
| C | `AGENT.md` | Load | Load | Always-loaded router | PASS |
| C | `instructions/06-loading-index.md` | Load | Load | Conditional-loading baseline | PASS |
| C | `instructions/00`–`05` | Load 01,04,05; skip 00,02,03 | Load 01,04,05; skip 00,02,03 | Execution start, log, completion gates | PASS |
| C | `docs/project/PROMPT_TEMPLATE.md` | Load | Load | Approved-execution defaults | PASS |
| C | Memory files | Skip all | Skip all | No implementation state inspection needed | PASS |
| C | ADRs | Skip | Skip | No architecture delta | PASS |
| C | ADR checklist | Skip | Skip | Architecture Skill did not trigger | PASS |
| C | CODE_REVIEW | Skip | Skip | Implementation never started | PASS |
| C | Text-to-SQL guide | Skip | Skip | No database/runtime inspection | PASS |
| C | Workflow profiles | Load execution; skip others | Load execution; skip others | Execution-start responsibility | PASS |
| C | README | Skip | Skip | Unrelated | PASS |
| C | Target evidence | Load path/branch/status/upstream/prompt filenames | Loaded as expected | Branch and prompt-log gates | PASS |
| D | `AGENT.md` | Load | Load | Always-loaded router | PASS |
| D | Loading index | Load | Load | Conditional-loading baseline | PASS |
| D | `instructions/00`–`05` | Load 01–04; skip 00,05 | Load 01–04; skip 00,05 | Planning, architecture, database, ADR/log rules | PASS |
| D | Prompt template | Load | Load | Plan/database-impact contract | PASS |
| D | Memory files | Load PROJECT_STATUS; skip others | Loaded PROJECT_STATUS only | Current Catalog database state | PASS |
| D | ADRs | Load identifiers and ADR-001 | Loaded as expected | Read-model/persistence ownership search | PASS |
| D | ADR checklist | Load | Load | Persistence materiality | PASS |
| D | CODE_REVIEW | Skip | Skip | Proposal only | PASS |
| D | Text-to-SQL guide | Skip | Skip | Views/users unaffected | PASS |
| D | Workflow profiles | Load planning; skip others | Load planning; skip others | Planning responsibility | PASS |
| D | README | Skip | Skip | Unrelated | PASS |
| D | Target evidence | Load Product mapping/snapshot | Loaded targeted files/sections | Nullable-column safety | PASS |
| E | `AGENT.md` | Load | Load | Always-loaded router | PASS |
| E | Loading index | Load | Load | Conditional-loading baseline | PASS |
| E | `instructions/00`–`05` | Load 02–05; skip 00,01 | Load 02–05; skip 00,01 | Explicit architecture/security review and logging | PASS |
| E | Prompt template | Skip | Skip | No plan/execution expansion needed | PASS |
| E | Memory files | Load AI_HANDOFF; skip others | Loaded AI_HANDOFF only | Secret-handling warnings | PASS |
| E | ADRs | Skip | Skip | Diff does not change runtime architecture or request ADR compliance | PASS |
| E | ADR checklist | Skip | Skip | Architecture decision Skill skipped | PASS |
| E | CODE_REVIEW | Load | Load | Findings-first review | PASS |
| E | Text-to-SQL guide | Skip | Skip | Unrelated configuration path does not justify DB guidance | PASS |
| E | Workflow profiles | Load code-review/security | Loaded both | Review responsibilities | PASS |
| E | README | Skip | Skip | Not independently reviewed | PASS |
| E | Target evidence | Load status, names, stats, diff-check, redacted scan | Loaded as expected | Current-diff evidence without values | PASS |
| F | `AGENT.md` | Load | Load | Always-loaded router | PASS |
| F | Loading index | Load | Load | Conditional-loading baseline | PASS |
| F | `instructions/00`–`05` | Load 04 only | Load 04 only | Project-memory rule | PASS |
| F | Prompt template | Skip | Skip | No plan/execution expansion | PASS |
| F | Memory files | Load PROJECT_STATUS and NEXT_SESSION; skip AI_HANDOFF/ROADMAP | Loaded selected two | Generic feature affects state/resume; no stated constraint or roadmap item | PASS |
| F | ADRs | Skip | Skip | No architecture decision proposed | PASS |
| F | ADR checklist | Skip | Skip | Architecture Skill skipped | PASS |
| F | CODE_REVIEW | Skip | Skip | Verification stipulated | PASS |
| F | Text-to-SQL guide | Skip | Skip | No DB surface specified | PASS |
| F | Workflow profiles | Skip | Skip | Skill body supplies targeting rules | PASS |
| F | README | Skip | Skip | Unrelated | PASS |
| F | Target evidence | Skip source/tests | Skip | Generic verified premise; no factual update allowed | PASS |
| G | `AGENT.md` | Load | Load | Always-loaded router | PASS |
| G | Loading index | Load | Load | Conditional-loading baseline | PASS |
| G | `instructions/00`–`05` | Load 03,05; skip 00,01,02,04 | Load 03,05; skip others | Test/security selection and completion | PASS |
| G | Prompt template | Load | Load | Default verification commands | PASS |
| G | Memory files | Load AI_HANDOFF; skip others | Loaded AI_HANDOFF only | Current verification commands/cautions | PASS |
| G | ADRs | Skip | Skip | No architecture decision | PASS |
| G | ADR checklist | Skip | Skip | Architecture Skill skipped | PASS |
| G | CODE_REVIEW | Skip | Skip | Dry-run verification only | PASS |
| G | Text-to-SQL guide | Skip | Skip | Database impact unspecified | PASS |
| G | Workflow profiles | Skip | Skip | Verification body sufficient | PASS |
| G | README | Skip | Skip | Unrelated | PASS |
| G | Target evidence | Load solution/project inventory only | Loaded as expected | Select generic commands without source over-read | PASS |
| H | `AGENT.md` | Load | Load | Always-loaded router | PASS |
| H | Loading index | Load | Load | Conditional-loading baseline | PASS |
| H | `instructions/00`–`05` | Load 01,05; skip others | Load 01,05; skip others | Commit gates only | PASS |
| H | Prompt template | Skip | Skip | Commit Skill owns composition | PASS |
| H | Memory files | Skip all | Skip all | Memory producer not rerun | PASS |
| H | ADRs | Skip | Skip | No architecture decision | PASS |
| H | ADR checklist | Skip | Skip | Architecture Skill skipped | PASS |
| H | CODE_REVIEW | Skip | Skip | Existing result consumed, not reproduced | PASS |
| H | Text-to-SQL guide | Skip | Skip | Migration/DB owner not invoked | PASS |
| H | Workflow profiles | Skip | Skip | Readiness body sufficient | PASS |
| H | README | Skip | Skip | Unrelated | PASS |
| H | Target evidence | Load current branch/status/file set | Loaded as expected | Freshness/scope comparison | PASS |
| I | `AGENT.md` | Load | Load | Always-loaded router | PASS |
| I | Loading index | Load | Load | Conditional-loading baseline | PASS |
| I | `instructions/00`–`05` | Load 01,05; skip others | Load 01,05; skip others | Push gates only | PASS |
| I | Prompt template | Skip | Skip | Push Skill owns composition | PASS |
| I | Memory files | Skip all | Skip all | Not needed for push evidence | PASS |
| I | ADRs | Skip | Skip | No architecture decision | PASS |
| I | ADR checklist | Skip | Skip | Architecture Skill skipped | PASS |
| I | CODE_REVIEW | Skip | Skip | Producer evidence not rerun | PASS |
| I | Text-to-SQL guide | Skip | Skip | Unrelated | PASS |
| I | Workflow profiles | Skip | Skip | Push body sufficient | PASS |
| I | README | Skip | Skip | Unrelated | PASS |
| I | Target evidence | Load branch/upstream/outgoing/worktree | Loaded as expected | Push readiness state | PASS |
| J | `AGENT.md` | Load | Load | Always-loaded router | PASS |
| J | Loading index | Load | Load | Credential-only route | PASS |
| J | `instructions/00`–`05` | Load 03–05; skip 00–02 | Load 03–05; skip 00–02 | Security, review, and prompt-log rules | PASS |
| J | Prompt template | Skip | Skip | No plan/execution expansion | PASS |
| J | Memory files | Load AI_HANDOFF; skip others | Loaded AI_HANDOFF only | Secret-handling warnings | PASS |
| J | ADRs | Skip | Skip | Unsafe config proposal is not a new architecture decision | PASS |
| J | ADR checklist | Skip | Skip | Architecture Skill skipped | PASS |
| J | CODE_REVIEW | Load | Load | Findings-first proposal review | PASS |
| J | Text-to-SQL guide | Skip | Skip | Credential-only scope; no target/permission/schema change | PASS |
| J | Workflow profiles | Load code-review/security | Loaded both | Security review responsibilities | PASS |
| J | README | Skip | Skip | Unrelated | PASS |
| J | Target evidence | Proposal text only; values skipped | Loaded proposal only | Avoid secret exposure | PASS |
| K | `AGENT.md` | Load | Load | Always-loaded router | PASS |
| K | Loading index | Load | Load | Documentation route | PASS |
| K | `instructions/00`–`05` | Load 01,04; skip others | Load 01,04; skip others | Execution lock and prompt logging | PASS |
| K | Prompt template | Skip | Skip | Not an approved execution or plan expansion | PASS |
| K | Memory files | Skip all | Skip all | Grammar-only exclusion | PASS |
| K | ADRs | Skip | Skip | No technical decision | PASS |
| K | ADR checklist | Skip | Skip | Architecture Skill skipped | PASS |
| K | CODE_REVIEW | Skip | Skip | Simple proofreading excluded | PASS |
| K | Text-to-SQL guide | Skip | Skip | Unrelated | PASS |
| K | Workflow profiles | Skip | Skip | No edit authorized | PASS |
| K | README | Load | Load | Confirm requested target and technical meaning | PASS |
| K | Target evidence | Skip all technical source/tests | Skip | No technical change | PASS |
| L | `AGENT.md` | Load | Load | Always-loaded router | PASS |
| L | Loading index | Load | Load | High-risk/unclear route | PASS |
| L | `instructions/00`–`05` | Load all | Load all | Architecture, deployment, security, ADR, verification/approval context | PASS |
| L | Prompt template | Skip | Skip | Request is neither a valid plan nor approved execution | PASS |
| L | Memory files | Load relevant sections of all four | Loaded as expected | Current architecture, constraints, direction, resume warnings | PASS |
| L | ADRs | Load identifiers/subjects only | Loaded as expected | No owner selectable from vague scope | PASS |
| L | ADR checklist | Load | Load | Materiality and ownership uncertain | PASS |
| L | CODE_REVIEW | Skip | Skip | No diff | PASS |
| L | Text-to-SQL guide | Skip | Skip | No specific Text-to-SQL/database proposal | PASS |
| L | Workflow profiles | Skip | Skip | No valid planning/execution stage established | PASS |
| L | README | Skip | Skip | Unrelated | PASS |
| L | Target evidence | Skip source/deployment state/actions | Skip | Request lacks target and authorization | PASS |

Instruction-loading matrix result: all expected loads and skips matched actual routing.

## Output Contract Validation

| Skill | Contract validated | Result |
|---|---|---|
| architecture-decision-check | `ADR_ACTION: CREATE | UPDATE | NOT_REQUIRED | BLOCKED` | PASS |
| branch-start-check | `BRANCH_START: PASS | BLOCKED` | PASS |
| code-review-check | `CODE_REVIEW: PASS | BLOCKED` | PASS |
| commit-readiness | `COMMIT_READINESS: PASS | BLOCKED` and complete evidence fields | PASS |
| migration-safety-check | Plan safety, execution approval, and final `MIGRATION_SAFETY` with `NOT_APPLICABLE` | PASS |
| project-memory-update | `PROJECT_MEMORY_UPDATE: PASS | BLOCKED` | PASS |
| prompt-log-writer | `PROMPT_LOG: PASS | BLOCKED` | PASS |
| push-readiness | `PUSH_READINESS: PASS | BLOCKED` plus three required producer results | PASS |
| secret-scan-check | `SECRET_SCAN_STATUS: PASS | BLOCKED` | PASS |
| verification-runner | `VERIFICATION_DRY_RUN: COMPLETE | BLOCKED`; executed `VERIFICATION_STATUS: PASS | FAIL | BLOCKED` | PASS |

No dry-run scenario emitted `VERIFICATION_STATUS: PASS`. Commit and push readiness did not rerun producer Skills.

## Context Efficiency

| Scenario | Invoked Skills | References | Loading result |
|---|---:|---:|---|
| A | 1 | 0 | EFFICIENT — targeted implemented-feature evidence |
| B | 2 | 1 | ACCEPTABLE — material AI autonomy justified broader context |
| C | 2 | 0 | EFFICIENT — branch/log evidence only |
| D | 3 | 1 | ACCEPTABLE — persistence proposal required targeted schema context |
| E | 3 | 0 | EFFICIENT — architecture instruction loaded; unrelated DB guidance skipped |
| F | 1 | 0 | EFFICIENT — two memory files selected, two left unloaded |
| G | 1 | 0 | EFFICIENT — verification guidance only |
| H | 1 | 0 | HIGHLY_EFFICIENT — producer evidence consumed, not rerun |
| I | 1 | 0 | HIGHLY_EFFICIENT — producer evidence consumed, not rerun |
| J | 3 | 0 | EFFICIENT — migration and ADR over-triggering eliminated |
| K | 1 | 0 | HIGHLY_EFFICIENT — prompt route and tiny target only |
| L | 1 | 1 | ACCEPTABLE — broad loading justified by vague high-risk deployment request |

No file was loaded merely to report that it was skipped. Metadata inspection did not expose Skill-body fragments.

## Regression Comparison

Compared with `skill-routing-and-loading-test-report.md`:

- Planning/documentation prompt-log under-triggering is fixed in A, B, D, E, J, and K where the global rule applies.
- Scenario E now loads `instructions/02-architecture-and-modules.md` and skips unrelated Text-to-SQL/database guidance.
- Credential-only Scenario J skips migration safety.
- Scenario G uses the complete dry-run classification contract and does not claim executed success.
- Scenarios H and I compose evidence without rerunning producer Skills.
- Scenario I requires commit-readiness, executed-verification, and secret-scan PASS evidence.
- Secret scan uses only `SECRET_SCAN_STATUS`.
- Obsolete Skill-discovery directories and metadata body-fragment over-read are gone.

Compared with `skill-routing-defect-regression-report.md`:

- All seven focused repaired behaviors remain passing inside the expanded twelve-scenario suite.
- The additional A–L coverage found no new routing or conditional-loading regression.
- All 120 scenario/Skill combinations match expected behavior.

## Defects Found

No Skill-routing regressions found.

The current worktree remains operationally unsuitable for commit or push because it intentionally contains multiple prior task scopes and an unrelated pre-existing configuration change. That is external state correctly blocked by readiness Skills, not a Skill-routing defect.

## Files Created

- `docs/prompts/101-full-ecommerce-skill-routing-regression-test.md`
- `docs/skills/testing/full-skill-routing-regression-report.md`

## Files Modified

No pre-existing file was modified.

## Application Scope

No Ecommerce runtime behavior, source, API contract, database schema, migration, authentication, infrastructure, package, or configuration was changed. No Skill, instruction, memory document, or previous report was modified.

## Commit And Push Status

No commit created and no push performed.

## Migration Status

No migration created or applied.

## Secrets Review

The complete changed/untracked file set was scanned using non-disclosing high-confidence patterns. No likely credential was found, no configuration value was reported, and proposed secrets in Scenario J were treated as blocked.

## Deviations

The official Python validator was unavailable because no usable Python launcher is installed. The required existing PowerShell equivalent completed with zero validation errors. No other deviation occurred.

ECOMMERCE_FULL_SKILL_REGRESSION_STATUS: COMPLETE
