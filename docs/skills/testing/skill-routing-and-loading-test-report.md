# Ecommerce Skill Routing And Conditional Loading Test Report

Date: 2026-07-21

## Scope and Method

This report records read-only forward tests of the repository-local Skills. Skill Creator guidance governed metadata-first discovery, body loading after trigger selection, conditional reference loading, structural validation, and fresh evaluator scenario tests. No Skill, runtime file, migration, branch, commit, push, or deployment was changed by the test.

The routing baseline for every scenario was AGENT.md plus instructions/06-loading-index.md. All ten frontmatter descriptions were inspected before any scenario-specific body. A body was loaded only for an invoked Skill, except the explicitly reported premature migration-body load in Scenario E and metadata over-read in Scenario I.

## Skill Inventory

| Skill | Entrypoint | Metadata | Bundled reference |
|---|---|---|---|
| architecture-decision-check | .agents/skills/architecture-decision-check/SKILL.md | agents/openai.yaml | references/adr-review-checklist.md |
| branch-start-check | .agents/skills/branch-start-check/SKILL.md | agents/openai.yaml | None |
| code-review-check | .agents/skills/code-review-check/SKILL.md | agents/openai.yaml | None |
| commit-readiness | .agents/skills/commit-readiness/SKILL.md | agents/openai.yaml | None |
| migration-safety-check | .agents/skills/migration-safety-check/SKILL.md | agents/openai.yaml | None |
| project-memory-update | .agents/skills/project-memory-update/SKILL.md | agents/openai.yaml | None |
| prompt-log-writer | .agents/skills/prompt-log-writer/SKILL.md | agents/openai.yaml | None |
| push-readiness | .agents/skills/push-readiness/SKILL.md | agents/openai.yaml | None |
| secret-scan-check | .agents/skills/secret-scan-check/SKILL.md | agents/openai.yaml | None |
| verification-runner | .agents/skills/verification-runner/SKILL.md | agents/openai.yaml | None |

No additional valid Skills were present. The empty .agents/skills/workflow/architecture-decision-check directory is not a Skill.

## Baseline Validation

| Skill | Status | Evidence |
|---|---|---|
| architecture-decision-check | VALID | Kebab-case directory; one SKILL.md; exact two-key frontmatter; matching name; valid metadata; conditional reference resolves; concise; no placeholders/secrets |
| branch-start-check | VALID | Structure, frontmatter, metadata, exclusions, concision, and secret checks passed |
| code-review-check | VALID | Structure, frontmatter, metadata, exclusions, concision, and secret checks passed |
| commit-readiness | VALID | Structure, frontmatter, metadata, exclusions, concision, and secret checks passed |
| migration-safety-check | VALID | Structure, frontmatter, metadata, exclusions, concision, and secret checks passed |
| project-memory-update | VALID | Structure, frontmatter, metadata, exclusions, concision, and secret checks passed |
| prompt-log-writer | VALID | Structure, frontmatter, metadata, exclusions, concision, and secret checks passed |
| push-readiness | VALID | Structure, frontmatter, metadata, exclusions, concision, and secret checks passed |
| secret-scan-check | VALID | Structure, frontmatter, metadata, exclusions, concision, and secret checks passed |
| verification-runner | VALID | Structure, frontmatter, metadata, exclusions, concision, and secret checks passed |

Validator totals: ten Skills, ten entrypoints, zero Skill-level errors, no broken reference links, no HintId, no TODO placeholder, no duplicate authoritative workflow source, and no high-confidence secret pattern. Repository-level warning: one obsolete empty directory remains under .agents/skills/workflow.

## Scenario Traces

### SCENARIO: A — Routine Catalog planning

REQUEST:
Plan the next Catalog feature to update product descriptions using the existing architecture and API conventions. Do not execute.

SKILLS DISCOVERED:
- All ten canonical Skills listed in the inventory.

SKILLS CONSIDERED:
- architecture-decision-check: public contract, persistence, and existing-architecture language.
- prompt-log-writer: global planning-log rule conflicts with its approved-execution trigger.

SKILLS INVOKED:
- architecture-decision-check: evaluated materiality and existing ADR ownership.

SKILLS SKIPPED:
- prompt-log-writer: read-only planning test and no approved execution.
- branch-start-check, verification-runner, commit-readiness, push-readiness: execution/completion gates.
- migration-safety-check: no schema proposal.
- code-review-check, secret-scan-check: no diff or credential scope.
- project-memory-update: no verified implementation.

INSTRUCTION FILES LOADED:
- AGENT.md, instructions/06-loading-index.md: whole, routing baseline.
- instructions/01-execution-and-planning.md, instructions/02-architecture-and-modules.md, instructions/04-documentation-and-memory.md: whole, planning/architecture/ADR rules.
- docs/project/PROMPT_TEMPLATE.md and docs/agents/workflow/planning-sub-agent.md: whole, planning contract.
- docs/project/PROJECT_STATUS.md, AI_HANDOFF.md, ROADMAP.md, NEXT_SESSION.md: whole, current feature state.
- docs/decisions/ADR-001 through ADR-007: relevant identifier/subject sections.
- docs/prompts/053-catalog-update-product-details-planning.md and 054-catalog-update-product-details-execution.md: whole, existing feature evidence.

INSTRUCTION FILES NOT LOADED:
- instructions/00, 03, and 05; code-review/database/commit/push guidance: unnecessary for routine planning.
- Auth, Orders, assistant, CI, configuration, and migration categories: unrelated.

SKILL REFERENCES LOADED:
- .agents/skills/architecture-decision-check/references/adr-review-checklist.md: whole, contract/persistence materiality check.

SKILL REFERENCES NOT LOADED:
- No other Skill has a reference.

OUTPUT CONTRACT:
    ADR_ACTION: NOT_REQUIRED
    Existing ADRs: ADR-001 tangential; no new owning ADR.
    Evidence: Product description update already exists in Product.UpdateDetails, CQRS handler/validator, API request/controller, persistence mapping, tests, and prompts 053/054.
    Next step: clarify a genuinely distinct capability; do not duplicate the feature.

RESULT:
PASS

NOTES:
The evaluator also read the exact Catalog implementation/test files needed to establish that the requested capability already exists. One failed attempt to discover workflow/SKILL.md came from the orphan directory. Planning-log ambiguity remains.

### SCENARIO: B — Material Catalog autonomy architecture

REQUEST:
Plan replacing the deterministic Catalog assistant with a bounded autonomous LLM sub-agent that selects from allowlisted Catalog tools.

SKILLS DISCOVERED:
- All ten canonical Skills.

SKILLS CONSIDERED:
- architecture-decision-check: direct runtime-AI autonomy and orchestration trigger.
- prompt-log-writer: planning-log ambiguity.

SKILLS INVOKED:
- architecture-decision-check.

SKILLS SKIPPED:
- The other nine Skills: no execution start, diff, migration, memory update, verification, commit, push, or credential handling.

INSTRUCTION FILES LOADED:
- AGENT.md, instructions/06-loading-index.md: whole.
- instructions/00 through 05: whole under high-risk assistant-autonomy routing.
- docs/project/PROMPT_TEMPLATE.md, AI_SKILLS_SUBAGENT_ARCHITECTURE.md, and planning-sub-agent.md: whole.
- All four project-memory files: relevant current assistant sections.
- ADR-001 through ADR-007: ownership search; ADR-007 whole.
- Prompts 096 bounded-autonomy planning and 097 execution: whole.
- CatalogAssistantSubAgent.cs, CatalogAgentToolRegistry.cs, CatalogAgentOptions.cs, CatalogAgentInstructions.cs, IAssistantLanguageModel.cs, SearchCatalogProductsTool.cs, GetCatalogProductTool.cs: whole implementation evidence.

INSTRUCTION FILES NOT LOADED:
- Text-to-SQL database guide, migrations, provider secrets/config values, unrelated module implementations, build/test and completion actions.

SKILL REFERENCES LOADED:
- ADR checklist: whole because autonomy is material and ADR ownership exists.

SKILL REFERENCES NOT LOADED:
- None applicable beyond the ADR checklist.

OUTPUT CONTRACT:
    ADR_ACTION: NOT_REQUIRED
    Existing ADRs: ADR-004, ADR-005, ADR-006, and owning ADR-007.
    Evidence: ADR-007 and current implementation already establish the requested bounded Catalog model/tool loop.
    Next step: clarify a delta before CREATE or UPDATE.

RESULT:
PASS

NOTES:
The scenario wording duplicates an accepted and implemented decision. NOT_REQUIRED is more accurate than the generic expected UPDATE/CREATE choices.

### SCENARIO: C — Approved implementation start

REQUEST:
APPROVED: EXECUTE the already approved Catalog update plan. Perform execution-start checks; do not modify files.

SKILLS DISCOVERED:
- All ten canonical Skills.

SKILLS CONSIDERED:
- branch-start-check and prompt-log-writer: explicit approved-execution triggers.

SKILLS INVOKED:
- branch-start-check: safe start check.
- prompt-log-writer: required execution log check.

SKILLS SKIPPED:
- architecture-decision-check: no unresolved architecture delta supplied.
- All review, migration, memory, verification, commit, secret, and push Skills: implementation did not start.

INSTRUCTION FILES LOADED:
- AGENT.md, instructions/06-loading-index.md, instructions/01, 04, and 05: whole.
- docs/project/PROMPT_TEMPLATE.md and docs/agents/workflow/execution-sub-agent.md: whole.
- Git path, branch, status, remote/ref, ahead/behind evidence; prompt filenames only.

INSTRUCTION FILES NOT LOADED:
- Source/tests, project memory, ADR bodies, code-review guidance, migration/config contents, build/test/EF instructions.

SKILL REFERENCES LOADED:
- None.

SKILL REFERENCES NOT LOADED:
- ADR checklist; architecture did not trigger.

OUTPUT CONTRACT:
    Repository: C:/ZippyYum/Learning/zy-commerce-backend
    Starting branch: feature/backend-bounded-autonomous-catalog-agent
    Worktree: dirty
    Main freshness: local main is two commits behind cached origin/main; no fetch attempted
    Task branch: none created
    BRANCH_START: BLOCKED

    Prompt log: not created for the dry-run scenario
    Number: 100 would be next after current prompt 099
    Status: APPROVED would be required
    PROMPT_LOG: BLOCKED

RESULT:
BLOCKED

NOTES:
Safe blocking passed. No clean, reset, stash, checkout, branch creation, fetch, prompt write, commit, or push occurred.

### SCENARIO: D — Nullable Product subtitle migration

REQUEST:
Plan adding a nullable Product subtitle column with an EF Core migration. Do not create or apply it.

SKILLS DISCOVERED:
- All ten canonical Skills.

SKILLS CONSIDERED:
- architecture-decision-check: persistence materiality.
- migration-safety-check: explicit schema/migration trigger.
- prompt-log-writer: planning-log ambiguity.

SKILLS INVOKED:
- architecture-decision-check.
- migration-safety-check.

SKILLS SKIPPED:
- All execution, review, completion, memory, secret, commit, and push Skills.

INSTRUCTION FILES LOADED:
- AGENT.md, instructions/06, instructions/00 through 05, prompt template, planning sub-agent: whole.
- All four memory files and all ADRs: relevant sections; ADR-001 whole.
- Product.cs, ProductConfiguration.cs, ProductSearchReadModel.cs, CatalogDbContext.cs, CatalogReadDbContext.cs, 20260618090000_AddProductPrice.cs, CatalogDbContextModelSnapshot.cs, relevant product requests/responses/DTOs: whole.

INSTRUCTION FILES NOT LOADED:
- ASSISTANT_TEXT_TO_SQL_READONLY_DB.md, assistant-view migrations, Auth/Orders, connection-string contents, EF/build/test execution.

SKILL REFERENCES LOADED:
- ADR checklist: whole, persistence materiality.

SKILL REFERENCES NOT LOADED:
- None other.

OUTPUT CONTRACT:
    ADR_ACTION: NOT_REQUIRED
    Rationale: nullable additive field fits existing module and persistence architecture.

    Affected database: Catalog, catalog.Products
    Migration files: proposal only
    Approval evidence: planning-only; creation/application not approved
    Risks: max length and exposure unresolved; Down discards subtitle data
    MIGRATION_SAFETY: BLOCKED

RESULT:
PASS

NOTES:
Planning may continue while creation/application remains blocked. The migration Skill does not currently make that distinction explicit.

### SCENARIO: E — Current diff review

REQUEST:
Review the current working-tree diff for correctness, security, architecture compliance, missing tests, and regressions. Do not modify files.

SKILLS DISCOVERED:
- All ten canonical Skills.

SKILLS CONSIDERED:
- code-review-check, architecture-decision-check, secret-scan-check.
- migration-safety-check was considered because appsettings changed, then skipped because no connection string/schema/raw SQL/database-user change exists.
- verification-runner was evidence-only.

SKILLS INVOKED:
- code-review-check, architecture-decision-check, secret-scan-check.

SKILLS SKIPPED:
- migration-safety-check after content classification; all branch, commit, memory, prompt, push, and verification execution Skills.

INSTRUCTION FILES LOADED:
- AGENT.md, instructions/06, 01, 03, 04, 05: whole.
- docs/project/CODE_REVIEW.md and code-review-sub-agent.md: whole.
- Prompts 098 and 099, ADR-007, ADR checklist: whole.
- ASSISTANT_TEXT_TO_SQL_READONLY_DB.md lines 146–200 and AssistantOrchestrator.cs lines 1–175: relevant sections.
- appsettings.json: values suppressed during review.
- Current status/diff inventory and changed Skill/docs files.

INSTRUCTION FILES NOT LOADED:
- Full source/test trees, migration/schema files, CI/package/frontend categories, database execution, unrelated ADR bodies.
- instructions/02 was not loaded despite architecture-compliance review; recorded as a missing conditional load.

SKILL REFERENCES LOADED:
- ADR checklist: intended workflow packaging and runtime-flag materiality.

SKILL REFERENCES NOT LOADED:
- None other.

OUTPUT CONTRACT:
    HIGH: appsettings.json enables Assistant:TextToSql despite prompt-098 scope recording it as unrelated.
    MEDIUM: runtime configuration verification is absent.
    MEDIUM: worktree mixes prompt-098 normalization, prompt-099 testing, and unrelated config provenance.
    CODE_REVIEW: BLOCKED
    Commit blocked: YES

    ADR_ACTION: NOT_REQUIRED for intended Skill normalization.
    SECRET_SCAN: PASS; no plausible committed credential found.

RESULT:
BLOCKED

NOTES:
Migration body loading was unnecessary. The pre-existing runtime flag change was not modified by this test.

### SCENARIO: F — Project-memory targeting

REQUEST:
Assume an approved Catalog feature was implemented and verified. Determine memory updates; do not modify.

SKILLS DISCOVERED:
- All ten canonical Skills.

SKILLS CONSIDERED:
- project-memory-update: direct verified-completion trigger.

SKILLS INVOKED:
- project-memory-update.

SKILLS SKIPPED:
- The other nine Skills; verification was stipulated and only memory targeting was requested.

INSTRUCTION FILES LOADED:
- AGENT.md, instructions/06, instructions/04: whole.
- PROJECT_STATUS.md, AI_HANDOFF.md, ROADMAP.md, NEXT_SESSION.md: whole, as explicitly required by the Skill.

INSTRUCTION FILES NOT LOADED:
- Source/tests, ADRs, prompts, API contracts, demo slides, migration references, other instruction files and Skill bodies.

SKILL REFERENCES LOADED:
- None.

SKILL REFERENCES NOT LOADED:
- ADR checklist; no architecture decision was proposed.

OUTPUT CONTRACT:
    Memory files updated: PROJECT_STATUS.md, AI_HANDOFF.md, ROADMAP.md, NEXT_SESSION.md
    Facts recorded: implemented capability/API/database impact, verification, handoff constraints, completed roadmap item, last-completed/current-state guidance
    Files unchanged: none of the four should be omitted, but unaffected sections remain unchanged
    PROJECT_MEMORY_UPDATE: PASS

RESULT:
PASS

NOTES:
No file was actually modified and no unverified fact was recorded.

### SCENARIO: G — Verification planning after implementation

REQUEST:
Assume an API feature is implemented. Report required verification commands without executing them.

SKILLS DISCOVERED:
- All ten canonical Skills.

SKILLS CONSIDERED:
- verification-runner: direct post-implementation trigger.

SKILLS INVOKED:
- verification-runner.

SKILLS SKIPPED:
- The other nine Skills; no database impact, commit, push, memory update, or diff review was specified.

INSTRUCTION FILES LOADED:
- AGENT.md, instructions/06, instructions/03 and 05: whole.
- PROMPT_TEMPLATE.md lines 261–330 and 571–600; AI_HANDOFF.md lines 196–220.
- Solution/project filenames only.

INSTRUCTION FILES NOT LOADED:
- Source/test contents, config, migrations, ADRs, other memory, prompt logs, other Skill bodies.

SKILL REFERENCES LOADED:
- None.

SKILL REFERENCES NOT LOADED:
- ADR checklist.

OUTPUT CONTRACT:
    Change type: API/runtime feature
    Commands required: dotnet restore Ecommerce.sln; dotnet build Ecommerce.sln; dotnet test Ecommerce.sln; targeted architecture/module/manual API checks after exact route/auth/status scope is known
    Results: NOT RUN
    Deferred: exact endpoint and module checks
    VERIFICATION: BLOCKED

RESULT:
PASS

NOTES:
Routing passed and the Skill blocked truthful completion. Its formal contract lacks the requested Required, Not required, Deferred, and Blocked categories.

### SCENARIO: H — Commit readiness

REQUEST:
Assess whether current changes are ready for a local commit. Do not commit.

SKILLS DISCOVERED:
- All ten canonical Skills.

SKILLS CONSIDERED:
- commit-readiness plus branch, review, verification, and secret evidence owners.

SKILLS INVOKED:
- commit-readiness, verification-runner, code-review-check, secret-scan-check.

SKILLS SKIPPED:
- push-readiness and unrelated architecture/migration/memory/prompt Skills.
- branch-start-check remained evidence-only; no execution start was requested.

INSTRUCTION FILES LOADED:
- AGENT.md, instructions/06, instructions/01 relevant branch/commit sections, instructions/03 and 05 whole.
- PROMPT_TEMPLATE relevant execution/verification sections, AI_HANDOFF relevant warnings, CODE_REVIEW.md whole.
- Current Git status/diff-name evidence and Scenario-E review/scan evidence.

INSTRUCTION FILES NOT LOADED:
- Push/upstream readiness, migrations, memory mutation, full source/test trees, build/test execution.

SKILL REFERENCES LOADED:
- None.

SKILL REFERENCES NOT LOADED:
- ADR checklist.

OUTPUT CONTRACT:
    Branch: feature/backend-bounded-autonomous-catalog-agent
    Changed files: mixed scopes and unrelated pre-existing runtime flag
    Verification: BLOCKED
    Code review: BLOCKED
    Secret scan: PASS
    Generated artifacts: none among changed paths
    COMMIT_READINESS: BLOCKED

RESULT:
BLOCKED

NOTES:
No commit approval or commit action was inferred.

### SCENARIO: I — Push readiness

REQUEST:
Assess whether current branch is ready to push. Do not push.

SKILLS DISCOVERED:
- All ten canonical Skills.

SKILLS CONSIDERED:
- push-readiness, secret-scan-check, plus branch/commit/verification evidence owners.

SKILLS INVOKED:
- push-readiness and secret-scan-check.

SKILLS SKIPPED:
- commit-readiness and verification-runner were not invoked despite missing evidence; all unrelated Skills skipped.

INSTRUCTION FILES LOADED:
- AGENT.md, instructions/06, instructions/01 and 05 relevant push/branch sections, instructions/03 security section, AI_HANDOFF secret sections.
- Git branch, remote, status, upstream, and outgoing-commit inventories.

INSTRUCTION FILES NOT LOADED:
- Source/tests, config values, full diff contents, migrations, ADRs, broader memory, verification evidence.

SKILL REFERENCES LOADED:
- None.

SKILL REFERENCES NOT LOADED:
- ADR checklist.

OUTPUT CONTRACT:
    SECRET_SCAN: BLOCKED because intended dirty/untracked scope was undefined.
    Branch: feature/backend-bounded-autonomous-catalog-agent
    Upstream: origin/feature/backend-bounded-autonomous-catalog-agent
    Outgoing commits: none
    Worktree: dirty
    Approval: absent
    PUSH_READINESS: BLOCKED

RESULT:
BLOCKED

NOTES:
The evaluator’s metadata command exposed title/required-input fragments beyond frontmatter, an unnecessary test-harness load. More importantly, the push Skill body omits required verification, secret-scan, and commit-readiness evidence even though repository completion rules require secret scanning.

### SCENARIO: J — Proposed appsettings secrets

REQUEST:
Review adding an API key and database connection string to appsettings.json.

SKILLS DISCOVERED:
- All ten canonical Skills.

SKILLS CONSIDERED:
- secret-scan-check, code-review-check, architecture-decision-check, migration-safety-check; commit-readiness evidence only.

SKILLS INVOKED:
- All four review/security/architecture/database Skills.

SKILLS SKIPPED:
- Branch, commit, memory, prompt, push, and verification execution Skills.

INSTRUCTION FILES LOADED:
- AGENT.md, instructions/06, instructions/01 through 05 except 00: whole or relevant ADR sections.
- CODE_REVIEW.md whole; PROJECT_STATUS and AI_HANDOFF relevant database/secret sections.
- All ADR filenames/subjects; ADR-006 whole.
- ADR checklist whole.

INSTRUCTION FILES NOT LOADED:
- Actual appsettings values, source/tests, Git diff, migration files, environment/user-secret values, Text-to-SQL DB guide.

SKILL REFERENCES LOADED:
- ADR checklist: security/persistence materiality and owning ADR search.

SKILL REFERENCES NOT LOADED:
- None other.

OUTPUT CONTRACT:
    ADR_ACTION: BLOCKED
    CODE_REVIEW: BLOCKED; Commit blocked: YES
    MIGRATION_SAFETY: FAIL
    SECRET_SCAN: BLOCKED

RESULT:
PASS

NOTES:
No secret was echoed. The actual secret contract is SECRET_SCAN, while the scenario specification expects SECRET_SCAN_STATUS; this is a contract defect. Migration triggering solely for connection-string review is broader than the scenario’s conditional expectation.

### SCENARIO: K — README grammar correction

REQUEST:
Fix README grammar without technical change.

SKILLS DISCOVERED:
- All ten canonical Skills.

SKILLS CONSIDERED:
- prompt-log-writer only because repository documentation rules require logs.

SKILLS INVOKED:
- None.

SKILLS SKIPPED:
- All ten; code-review explicitly excludes simple proofreading and execution approval is absent.

INSTRUCTION FILES LOADED:
- AGENT.md, instructions/06; instructions/01 execution-lock section, instructions/04 documentation/log section, instructions/05 documentation self-review section.
- README.md whole; the requested paragraph does not exist.

INSTRUCTION FILES NOT LOADED:
- All Skill bodies/references, source/tests/config, memory, ADRs, Git state, build/test commands.

SKILL REFERENCES LOADED:
- None.

SKILL REFERENCES NOT LOADED:
- ADR checklist and all optional references.

OUTPUT CONTRACT:
    No Skill output applies.
    EXECUTION: BLOCKED because APPROVED: EXECUTE is absent.

RESULT:
PASS

NOTES:
Minimal routing worked, but global prompt logging conflicts with prompt-log metadata and the execution lock.

### SCENARIO: L — Ambiguous architecture and deploy

REQUEST:
Change the architecture and deploy it.

SKILLS DISCOVERED:
- All ten canonical Skills.

SKILLS CONSIDERED:
- architecture-decision-check: direct architecture/deployment trigger.

SKILLS INVOKED:
- architecture-decision-check.

SKILLS SKIPPED:
- All execution-start, verification, migration, review, commit, push, memory, prompt, and secret Skills because scope and approval are absent.

INSTRUCTION FILES LOADED:
- AGENT.md, instructions/06, instructions/00 through 05: whole under high-risk/unclear routing.
- All four memory files: relevant architecture/current-state sections.
- All ADR identifiers/subjects; no full owner selected.

INSTRUCTION FILES NOT LOADED:
- Source/tests/config/project files, prompt template, deployment manifests/cloud state, full ADR bodies, migrations, Git/build/test/deployment actions.

SKILL REFERENCES LOADED:
- ADR checklist whole because architecture is material and ownership is unclear.

SKILL REFERENCES NOT LOADED:
- None other.

OUTPUT CONTRACT:
    ADR_ACTION: BLOCKED
    Affected areas: unspecified architecture and deployment topology
    Existing ADRs: ADR-001 through ADR-007 searched; no owner selectable
    Next step: provide modules, desired architecture, environment/topology, constraints, rollback plan, and explicit execution/deployment authority

RESULT:
BLOCKED

NOTES:
No deployment Skill or deployment-specific approval contract exists; architecture routing can only classify and block.

## Skill Interaction Matrix

| Scenario | Skill | Expected | Actual | Result |
|---|---|---|---|---|
| A Routine planning | architecture-decision-check | Invoke | Invoke | PASS |
| A Routine planning | branch-start-check | Skip | Skip | PASS |
| A Routine planning | code-review-check | Skip | Skip | PASS |
| A Routine planning | commit-readiness | Skip | Skip | PASS |
| A Routine planning | migration-safety-check | Skip | Skip | PASS |
| A Routine planning | project-memory-update | Skip | Skip | PASS |
| A Routine planning | prompt-log-writer | Consider only | Skip | FAIL |
| A Routine planning | push-readiness | Skip | Skip | PASS |
| A Routine planning | secret-scan-check | Skip | Skip | PASS |
| A Routine planning | verification-runner | Skip | Skip | PASS |
| B Material architecture | architecture-decision-check | Invoke | Invoke | PASS |
| B Material architecture | branch-start-check | Skip | Skip | PASS |
| B Material architecture | code-review-check | Skip | Skip | PASS |
| B Material architecture | commit-readiness | Skip | Skip | PASS |
| B Material architecture | migration-safety-check | Skip | Skip | PASS |
| B Material architecture | project-memory-update | Skip | Skip | PASS |
| B Material architecture | prompt-log-writer | Consider only | Skip | FAIL |
| B Material architecture | push-readiness | Skip | Skip | PASS |
| B Material architecture | secret-scan-check | Skip | Skip | PASS |
| B Material architecture | verification-runner | Skip | Skip | PASS |
| C Approved start | architecture-decision-check | Skip | Skip | PASS |
| C Approved start | branch-start-check | Invoke | Blocked | PASS |
| C Approved start | code-review-check | Skip | Skip | PASS |
| C Approved start | commit-readiness | Skip | Skip | PASS |
| C Approved start | migration-safety-check | Skip | Skip | PASS |
| C Approved start | project-memory-update | Skip | Skip | PASS |
| C Approved start | prompt-log-writer | Invoke | Blocked | PASS |
| C Approved start | push-readiness | Skip | Skip | PASS |
| C Approved start | secret-scan-check | Skip | Skip | PASS |
| C Approved start | verification-runner | Skip | Skip | PASS |
| D Migration proposal | architecture-decision-check | Invoke | Invoke | PASS |
| D Migration proposal | branch-start-check | Skip | Skip | PASS |
| D Migration proposal | code-review-check | Skip | Skip | PASS |
| D Migration proposal | commit-readiness | Skip | Skip | PASS |
| D Migration proposal | migration-safety-check | Invoke | Blocked | PASS |
| D Migration proposal | project-memory-update | Skip | Skip | PASS |
| D Migration proposal | prompt-log-writer | Consider only | Skip | FAIL |
| D Migration proposal | push-readiness | Skip | Skip | PASS |
| D Migration proposal | secret-scan-check | Skip | Skip | PASS |
| D Migration proposal | verification-runner | Skip | Skip | PASS |
| E Code review | architecture-decision-check | Consider only | Invoke | PASS |
| E Code review | branch-start-check | Skip | Skip | PASS |
| E Code review | code-review-check | Invoke | Blocked | PASS |
| E Code review | commit-readiness | Skip | Skip | PASS |
| E Code review | migration-safety-check | Skip | Consider only | FAIL |
| E Code review | project-memory-update | Skip | Skip | PASS |
| E Code review | prompt-log-writer | Skip | Skip | PASS |
| E Code review | push-readiness | Skip | Skip | PASS |
| E Code review | secret-scan-check | Consider only | Invoke | PASS |
| E Code review | verification-runner | Consider only | Consider only | PASS |
| F Project memory | architecture-decision-check | Skip | Skip | PASS |
| F Project memory | branch-start-check | Skip | Skip | PASS |
| F Project memory | code-review-check | Skip | Skip | PASS |
| F Project memory | commit-readiness | Skip | Skip | PASS |
| F Project memory | migration-safety-check | Skip | Skip | PASS |
| F Project memory | project-memory-update | Invoke | Invoke | PASS |
| F Project memory | prompt-log-writer | Skip | Skip | PASS |
| F Project memory | push-readiness | Skip | Skip | PASS |
| F Project memory | secret-scan-check | Skip | Skip | PASS |
| F Project memory | verification-runner | Skip | Skip | PASS |
| G Verification | architecture-decision-check | Skip | Skip | PASS |
| G Verification | branch-start-check | Skip | Skip | PASS |
| G Verification | code-review-check | Skip | Skip | PASS |
| G Verification | commit-readiness | Skip | Skip | PASS |
| G Verification | migration-safety-check | Skip | Skip | PASS |
| G Verification | project-memory-update | Skip | Skip | PASS |
| G Verification | prompt-log-writer | Skip | Skip | PASS |
| G Verification | push-readiness | Skip | Skip | PASS |
| G Verification | secret-scan-check | Skip | Skip | PASS |
| G Verification | verification-runner | Invoke | Blocked | PASS |
| H Commit readiness | architecture-decision-check | Skip | Skip | PASS |
| H Commit readiness | branch-start-check | Consider only | Consider only | PASS |
| H Commit readiness | code-review-check | Consider only | Invoke | PASS |
| H Commit readiness | commit-readiness | Invoke | Blocked | PASS |
| H Commit readiness | migration-safety-check | Skip | Skip | PASS |
| H Commit readiness | project-memory-update | Skip | Skip | PASS |
| H Commit readiness | prompt-log-writer | Skip | Skip | PASS |
| H Commit readiness | push-readiness | Skip | Skip | PASS |
| H Commit readiness | secret-scan-check | Consider only | Invoke | PASS |
| H Commit readiness | verification-runner | Consider only | Invoke | PASS |
| I Push readiness | architecture-decision-check | Skip | Skip | PASS |
| I Push readiness | branch-start-check | Consider only | Consider only | PASS |
| I Push readiness | code-review-check | Skip | Skip | PASS |
| I Push readiness | commit-readiness | Consider only | Skip | PASS |
| I Push readiness | migration-safety-check | Skip | Skip | PASS |
| I Push readiness | project-memory-update | Skip | Skip | PASS |
| I Push readiness | prompt-log-writer | Skip | Skip | PASS |
| I Push readiness | push-readiness | Invoke | Blocked | PASS |
| I Push readiness | secret-scan-check | Invoke | Blocked | PASS |
| I Push readiness | verification-runner | Consider only | Skip | FAIL |
| J Secret handling | architecture-decision-check | Consider only | Blocked | PASS |
| J Secret handling | branch-start-check | Skip | Skip | PASS |
| J Secret handling | code-review-check | Invoke | Blocked | PASS |
| J Secret handling | commit-readiness | Consider only | Skip | PASS |
| J Secret handling | migration-safety-check | Consider only | Invoke | PASS |
| J Secret handling | project-memory-update | Skip | Skip | PASS |
| J Secret handling | prompt-log-writer | Skip | Skip | PASS |
| J Secret handling | push-readiness | Skip | Skip | PASS |
| J Secret handling | secret-scan-check | Blocked | Blocked | PASS |
| J Secret handling | verification-runner | Skip | Skip | PASS |
| K Grammar correction | architecture-decision-check | Skip | Skip | PASS |
| K Grammar correction | branch-start-check | Skip | Skip | PASS |
| K Grammar correction | code-review-check | Skip | Skip | PASS |
| K Grammar correction | commit-readiness | Skip | Skip | PASS |
| K Grammar correction | migration-safety-check | Skip | Skip | PASS |
| K Grammar correction | project-memory-update | Skip | Skip | PASS |
| K Grammar correction | prompt-log-writer | Consider only | Skip | FAIL |
| K Grammar correction | push-readiness | Skip | Skip | PASS |
| K Grammar correction | secret-scan-check | Skip | Skip | PASS |
| K Grammar correction | verification-runner | Skip | Skip | PASS |
| L Ambiguous architecture | architecture-decision-check | Blocked | Blocked | PASS |
| L Ambiguous architecture | branch-start-check | Skip | Skip | PASS |
| L Ambiguous architecture | code-review-check | Skip | Skip | PASS |
| L Ambiguous architecture | commit-readiness | Skip | Skip | PASS |
| L Ambiguous architecture | migration-safety-check | Skip | Skip | PASS |
| L Ambiguous architecture | project-memory-update | Skip | Skip | PASS |
| L Ambiguous architecture | prompt-log-writer | Skip | Skip | PASS |
| L Ambiguous architecture | push-readiness | Skip | Skip | PASS |
| L Ambiguous architecture | secret-scan-check | Skip | Skip | PASS |
| L Ambiguous architecture | verification-runner | Skip | Skip | PASS |

## Instruction-Loading Matrix

Each row records a scenario/file pair. Load means the file was semantically inspected; Skip means it was deliberately left unloaded. Mechanically scanning filenames or metadata is not counted as semantic loading.

| Scenario | File | Expected loading | Actual loading | Reason | Result |
|---|---|---|---|---|---|
| A Routine planning | AGENT.md | Load | Load | router | PASS |
| A Routine planning | instructions/00-role-and-stack.md | Skip | Skip | scenario did not require stack/module context | PASS |
| A Routine planning | instructions/01-execution-and-planning.md | Load | Load | planning, approval, or Git gates | PASS |
| A Routine planning | instructions/02-architecture-and-modules.md | Load | Load | architecture/module boundaries | PASS |
| A Routine planning | instructions/03-cqrs-database-testing-security.md | Skip | Skip | scenario did not require database, verification, or security rules | PASS |
| A Routine planning | instructions/04-documentation-and-memory.md | Load | Load | prompt, memory, or ADR rules | PASS |
| A Routine planning | instructions/05-completion.md | Skip | Skip | scenario did not require review/readiness/verification gates | PASS |
| A Routine planning | instructions/06-loading-index.md | Load | Load | conditional routing index | PASS |
| A Routine planning | docs/project/PROMPT_TEMPLATE.md | Load | Load | plan, execution, or verification contract | PASS |
| A Routine planning | docs/project/PROJECT_STATUS.md | Load | Load | current state evidence | PASS |
| A Routine planning | docs/project/AI_HANDOFF.md | Load | Load | current constraints/security/verification evidence | PASS |
| A Routine planning | docs/project/ROADMAP.md | Load | Load | memory or architecture direction | PASS |
| A Routine planning | docs/project/NEXT_SESSION.md | Load | Load | memory/current-state warnings | PASS |
| A Routine planning | docs/decisions/ADR-*.md | Load | Load | ADR ownership search | PASS |
| A Routine planning | .agents/skills/architecture-decision-check/references/adr-review-checklist.md | Load | Load | materiality/ownership checklist | PASS |
| A Routine planning | docs/project/CODE_REVIEW.md | Skip | Skip | scenario did not require findings-first review criteria | PASS |
| A Routine planning | docs/project/ASSISTANT_TEXT_TO_SQL_READONLY_DB.md | Skip | Skip | scenario did not require Text-to-SQL runtime behavior evidence | PASS |
| A Routine planning | docs/agents/workflow/planning-sub-agent.md | Load | Load | planning responsibility profile | PASS |
| A Routine planning | docs/agents/workflow/execution-sub-agent.md | Skip | Skip | scenario did not require execution-start responsibility profile | PASS |
| A Routine planning | README.md | Skip | Skip | scenario did not require grammar target verification | PASS |
| B Material architecture | AGENT.md | Load | Load | router | PASS |
| B Material architecture | instructions/00-role-and-stack.md | Load | Load | stack/module context | PASS |
| B Material architecture | instructions/01-execution-and-planning.md | Load | Load | planning, approval, or Git gates | PASS |
| B Material architecture | instructions/02-architecture-and-modules.md | Load | Load | architecture/module boundaries | PASS |
| B Material architecture | instructions/03-cqrs-database-testing-security.md | Load | Load | database, verification, or security rules | PASS |
| B Material architecture | instructions/04-documentation-and-memory.md | Load | Load | prompt, memory, or ADR rules | PASS |
| B Material architecture | instructions/05-completion.md | Load | Load | review/readiness/verification gates | PASS |
| B Material architecture | instructions/06-loading-index.md | Load | Load | conditional routing index | PASS |
| B Material architecture | docs/project/PROMPT_TEMPLATE.md | Load | Load | plan, execution, or verification contract | PASS |
| B Material architecture | docs/project/PROJECT_STATUS.md | Load | Load | current state evidence | PASS |
| B Material architecture | docs/project/AI_HANDOFF.md | Load | Load | current constraints/security/verification evidence | PASS |
| B Material architecture | docs/project/ROADMAP.md | Load | Load | memory or architecture direction | PASS |
| B Material architecture | docs/project/NEXT_SESSION.md | Load | Load | memory/current-state warnings | PASS |
| B Material architecture | docs/decisions/ADR-*.md | Load | Load | ADR ownership search | PASS |
| B Material architecture | .agents/skills/architecture-decision-check/references/adr-review-checklist.md | Load | Load | materiality/ownership checklist | PASS |
| B Material architecture | docs/project/CODE_REVIEW.md | Skip | Skip | scenario did not require findings-first review criteria | PASS |
| B Material architecture | docs/project/ASSISTANT_TEXT_TO_SQL_READONLY_DB.md | Skip | Skip | scenario did not require Text-to-SQL runtime behavior evidence | PASS |
| B Material architecture | docs/agents/workflow/planning-sub-agent.md | Load | Load | planning responsibility profile | PASS |
| B Material architecture | docs/agents/workflow/execution-sub-agent.md | Skip | Skip | scenario did not require execution-start responsibility profile | PASS |
| B Material architecture | README.md | Skip | Skip | scenario did not require grammar target verification | PASS |
| C Approved start | AGENT.md | Load | Load | router | PASS |
| C Approved start | instructions/00-role-and-stack.md | Skip | Skip | scenario did not require stack/module context | PASS |
| C Approved start | instructions/01-execution-and-planning.md | Load | Load | planning, approval, or Git gates | PASS |
| C Approved start | instructions/02-architecture-and-modules.md | Skip | Skip | scenario did not require architecture/module boundaries | PASS |
| C Approved start | instructions/03-cqrs-database-testing-security.md | Skip | Skip | scenario did not require database, verification, or security rules | PASS |
| C Approved start | instructions/04-documentation-and-memory.md | Load | Load | prompt, memory, or ADR rules | PASS |
| C Approved start | instructions/05-completion.md | Load | Load | review/readiness/verification gates | PASS |
| C Approved start | instructions/06-loading-index.md | Load | Load | conditional routing index | PASS |
| C Approved start | docs/project/PROMPT_TEMPLATE.md | Load | Load | plan, execution, or verification contract | PASS |
| C Approved start | docs/project/PROJECT_STATUS.md | Skip | Skip | scenario did not require current state evidence | PASS |
| C Approved start | docs/project/AI_HANDOFF.md | Skip | Skip | scenario did not require current constraints/security/verification evidence | PASS |
| C Approved start | docs/project/ROADMAP.md | Skip | Skip | scenario did not require memory or architecture direction | PASS |
| C Approved start | docs/project/NEXT_SESSION.md | Skip | Skip | scenario did not require memory/current-state warnings | PASS |
| C Approved start | docs/decisions/ADR-*.md | Skip | Skip | scenario did not require ADR ownership search | PASS |
| C Approved start | .agents/skills/architecture-decision-check/references/adr-review-checklist.md | Skip | Skip | scenario did not require materiality/ownership checklist | PASS |
| C Approved start | docs/project/CODE_REVIEW.md | Skip | Skip | scenario did not require findings-first review criteria | PASS |
| C Approved start | docs/project/ASSISTANT_TEXT_TO_SQL_READONLY_DB.md | Skip | Skip | scenario did not require Text-to-SQL runtime behavior evidence | PASS |
| C Approved start | docs/agents/workflow/planning-sub-agent.md | Skip | Skip | scenario did not require planning responsibility profile | PASS |
| C Approved start | docs/agents/workflow/execution-sub-agent.md | Load | Load | execution-start responsibility profile | PASS |
| C Approved start | README.md | Skip | Skip | scenario did not require grammar target verification | PASS |
| D Migration proposal | AGENT.md | Load | Load | router | PASS |
| D Migration proposal | instructions/00-role-and-stack.md | Load | Load | stack/module context | PASS |
| D Migration proposal | instructions/01-execution-and-planning.md | Load | Load | planning, approval, or Git gates | PASS |
| D Migration proposal | instructions/02-architecture-and-modules.md | Load | Load | architecture/module boundaries | PASS |
| D Migration proposal | instructions/03-cqrs-database-testing-security.md | Load | Load | database, verification, or security rules | PASS |
| D Migration proposal | instructions/04-documentation-and-memory.md | Load | Load | prompt, memory, or ADR rules | PASS |
| D Migration proposal | instructions/05-completion.md | Load | Load | review/readiness/verification gates | PASS |
| D Migration proposal | instructions/06-loading-index.md | Load | Load | conditional routing index | PASS |
| D Migration proposal | docs/project/PROMPT_TEMPLATE.md | Load | Load | plan, execution, or verification contract | PASS |
| D Migration proposal | docs/project/PROJECT_STATUS.md | Load | Load | current state evidence | PASS |
| D Migration proposal | docs/project/AI_HANDOFF.md | Load | Load | current constraints/security/verification evidence | PASS |
| D Migration proposal | docs/project/ROADMAP.md | Load | Load | memory or architecture direction | PASS |
| D Migration proposal | docs/project/NEXT_SESSION.md | Load | Load | memory/current-state warnings | PASS |
| D Migration proposal | docs/decisions/ADR-*.md | Load | Load | ADR ownership search | PASS |
| D Migration proposal | .agents/skills/architecture-decision-check/references/adr-review-checklist.md | Load | Load | materiality/ownership checklist | PASS |
| D Migration proposal | docs/project/CODE_REVIEW.md | Skip | Skip | scenario did not require findings-first review criteria | PASS |
| D Migration proposal | docs/project/ASSISTANT_TEXT_TO_SQL_READONLY_DB.md | Skip | Skip | scenario did not require Text-to-SQL runtime behavior evidence | PASS |
| D Migration proposal | docs/agents/workflow/planning-sub-agent.md | Load | Load | planning responsibility profile | PASS |
| D Migration proposal | docs/agents/workflow/execution-sub-agent.md | Skip | Skip | scenario did not require execution-start responsibility profile | PASS |
| D Migration proposal | README.md | Skip | Skip | scenario did not require grammar target verification | PASS |
| E Code review | AGENT.md | Load | Load | router | PASS |
| E Code review | instructions/00-role-and-stack.md | Skip | Skip | scenario did not require stack/module context | PASS |
| E Code review | instructions/01-execution-and-planning.md | Load | Load | planning, approval, or Git gates | PASS |
| E Code review | instructions/02-architecture-and-modules.md | Load | Skip | scenario did not require architecture/module boundaries | FAIL |
| E Code review | instructions/03-cqrs-database-testing-security.md | Load | Load | database, verification, or security rules | PASS |
| E Code review | instructions/04-documentation-and-memory.md | Load | Load | prompt, memory, or ADR rules | PASS |
| E Code review | instructions/05-completion.md | Load | Load | review/readiness/verification gates | PASS |
| E Code review | instructions/06-loading-index.md | Load | Load | conditional routing index | PASS |
| E Code review | docs/project/PROMPT_TEMPLATE.md | Skip | Skip | scenario did not require plan, execution, or verification contract | PASS |
| E Code review | docs/project/PROJECT_STATUS.md | Skip | Skip | scenario did not require current state evidence | PASS |
| E Code review | docs/project/AI_HANDOFF.md | Load | Load | current constraints/security/verification evidence | PASS |
| E Code review | docs/project/ROADMAP.md | Skip | Skip | scenario did not require memory or architecture direction | PASS |
| E Code review | docs/project/NEXT_SESSION.md | Skip | Skip | scenario did not require memory/current-state warnings | PASS |
| E Code review | docs/decisions/ADR-*.md | Load | Load | ADR ownership search | PASS |
| E Code review | .agents/skills/architecture-decision-check/references/adr-review-checklist.md | Load | Load | materiality/ownership checklist | PASS |
| E Code review | docs/project/CODE_REVIEW.md | Load | Load | findings-first review criteria | PASS |
| E Code review | docs/project/ASSISTANT_TEXT_TO_SQL_READONLY_DB.md | Skip | Load | Text-to-SQL runtime behavior evidence | FAIL |
| E Code review | docs/agents/workflow/planning-sub-agent.md | Skip | Skip | scenario did not require planning responsibility profile | PASS |
| E Code review | docs/agents/workflow/execution-sub-agent.md | Skip | Skip | scenario did not require execution-start responsibility profile | PASS |
| E Code review | README.md | Skip | Skip | scenario did not require grammar target verification | PASS |
| F Project memory | AGENT.md | Load | Load | router | PASS |
| F Project memory | instructions/00-role-and-stack.md | Skip | Skip | scenario did not require stack/module context | PASS |
| F Project memory | instructions/01-execution-and-planning.md | Skip | Skip | scenario did not require planning, approval, or Git gates | PASS |
| F Project memory | instructions/02-architecture-and-modules.md | Skip | Skip | scenario did not require architecture/module boundaries | PASS |
| F Project memory | instructions/03-cqrs-database-testing-security.md | Skip | Skip | scenario did not require database, verification, or security rules | PASS |
| F Project memory | instructions/04-documentation-and-memory.md | Load | Load | prompt, memory, or ADR rules | PASS |
| F Project memory | instructions/05-completion.md | Skip | Skip | scenario did not require review/readiness/verification gates | PASS |
| F Project memory | instructions/06-loading-index.md | Load | Load | conditional routing index | PASS |
| F Project memory | docs/project/PROMPT_TEMPLATE.md | Skip | Skip | scenario did not require plan, execution, or verification contract | PASS |
| F Project memory | docs/project/PROJECT_STATUS.md | Load | Load | current state evidence | PASS |
| F Project memory | docs/project/AI_HANDOFF.md | Load | Load | current constraints/security/verification evidence | PASS |
| F Project memory | docs/project/ROADMAP.md | Load | Load | memory or architecture direction | PASS |
| F Project memory | docs/project/NEXT_SESSION.md | Load | Load | memory/current-state warnings | PASS |
| F Project memory | docs/decisions/ADR-*.md | Skip | Skip | scenario did not require ADR ownership search | PASS |
| F Project memory | .agents/skills/architecture-decision-check/references/adr-review-checklist.md | Skip | Skip | scenario did not require materiality/ownership checklist | PASS |
| F Project memory | docs/project/CODE_REVIEW.md | Skip | Skip | scenario did not require findings-first review criteria | PASS |
| F Project memory | docs/project/ASSISTANT_TEXT_TO_SQL_READONLY_DB.md | Skip | Skip | scenario did not require Text-to-SQL runtime behavior evidence | PASS |
| F Project memory | docs/agents/workflow/planning-sub-agent.md | Skip | Skip | scenario did not require planning responsibility profile | PASS |
| F Project memory | docs/agents/workflow/execution-sub-agent.md | Skip | Skip | scenario did not require execution-start responsibility profile | PASS |
| F Project memory | README.md | Skip | Skip | scenario did not require grammar target verification | PASS |
| G Verification | AGENT.md | Load | Load | router | PASS |
| G Verification | instructions/00-role-and-stack.md | Skip | Skip | scenario did not require stack/module context | PASS |
| G Verification | instructions/01-execution-and-planning.md | Skip | Skip | scenario did not require planning, approval, or Git gates | PASS |
| G Verification | instructions/02-architecture-and-modules.md | Skip | Skip | scenario did not require architecture/module boundaries | PASS |
| G Verification | instructions/03-cqrs-database-testing-security.md | Load | Load | database, verification, or security rules | PASS |
| G Verification | instructions/04-documentation-and-memory.md | Skip | Skip | scenario did not require prompt, memory, or ADR rules | PASS |
| G Verification | instructions/05-completion.md | Load | Load | review/readiness/verification gates | PASS |
| G Verification | instructions/06-loading-index.md | Load | Load | conditional routing index | PASS |
| G Verification | docs/project/PROMPT_TEMPLATE.md | Load | Load | plan, execution, or verification contract | PASS |
| G Verification | docs/project/PROJECT_STATUS.md | Skip | Skip | scenario did not require current state evidence | PASS |
| G Verification | docs/project/AI_HANDOFF.md | Load | Load | current constraints/security/verification evidence | PASS |
| G Verification | docs/project/ROADMAP.md | Skip | Skip | scenario did not require memory or architecture direction | PASS |
| G Verification | docs/project/NEXT_SESSION.md | Skip | Skip | scenario did not require memory/current-state warnings | PASS |
| G Verification | docs/decisions/ADR-*.md | Skip | Skip | scenario did not require ADR ownership search | PASS |
| G Verification | .agents/skills/architecture-decision-check/references/adr-review-checklist.md | Skip | Skip | scenario did not require materiality/ownership checklist | PASS |
| G Verification | docs/project/CODE_REVIEW.md | Skip | Skip | scenario did not require findings-first review criteria | PASS |
| G Verification | docs/project/ASSISTANT_TEXT_TO_SQL_READONLY_DB.md | Skip | Skip | scenario did not require Text-to-SQL runtime behavior evidence | PASS |
| G Verification | docs/agents/workflow/planning-sub-agent.md | Skip | Skip | scenario did not require planning responsibility profile | PASS |
| G Verification | docs/agents/workflow/execution-sub-agent.md | Skip | Skip | scenario did not require execution-start responsibility profile | PASS |
| G Verification | README.md | Skip | Skip | scenario did not require grammar target verification | PASS |
| H Commit readiness | AGENT.md | Load | Load | router | PASS |
| H Commit readiness | instructions/00-role-and-stack.md | Skip | Skip | scenario did not require stack/module context | PASS |
| H Commit readiness | instructions/01-execution-and-planning.md | Load | Load | planning, approval, or Git gates | PASS |
| H Commit readiness | instructions/02-architecture-and-modules.md | Skip | Skip | scenario did not require architecture/module boundaries | PASS |
| H Commit readiness | instructions/03-cqrs-database-testing-security.md | Load | Load | database, verification, or security rules | PASS |
| H Commit readiness | instructions/04-documentation-and-memory.md | Skip | Skip | scenario did not require prompt, memory, or ADR rules | PASS |
| H Commit readiness | instructions/05-completion.md | Load | Load | review/readiness/verification gates | PASS |
| H Commit readiness | instructions/06-loading-index.md | Load | Load | conditional routing index | PASS |
| H Commit readiness | docs/project/PROMPT_TEMPLATE.md | Load | Load | plan, execution, or verification contract | PASS |
| H Commit readiness | docs/project/PROJECT_STATUS.md | Skip | Skip | scenario did not require current state evidence | PASS |
| H Commit readiness | docs/project/AI_HANDOFF.md | Load | Load | current constraints/security/verification evidence | PASS |
| H Commit readiness | docs/project/ROADMAP.md | Skip | Skip | scenario did not require memory or architecture direction | PASS |
| H Commit readiness | docs/project/NEXT_SESSION.md | Skip | Skip | scenario did not require memory/current-state warnings | PASS |
| H Commit readiness | docs/decisions/ADR-*.md | Skip | Skip | scenario did not require ADR ownership search | PASS |
| H Commit readiness | .agents/skills/architecture-decision-check/references/adr-review-checklist.md | Skip | Skip | scenario did not require materiality/ownership checklist | PASS |
| H Commit readiness | docs/project/CODE_REVIEW.md | Load | Load | findings-first review criteria | PASS |
| H Commit readiness | docs/project/ASSISTANT_TEXT_TO_SQL_READONLY_DB.md | Skip | Skip | scenario did not require Text-to-SQL runtime behavior evidence | PASS |
| H Commit readiness | docs/agents/workflow/planning-sub-agent.md | Skip | Skip | scenario did not require planning responsibility profile | PASS |
| H Commit readiness | docs/agents/workflow/execution-sub-agent.md | Skip | Skip | scenario did not require execution-start responsibility profile | PASS |
| H Commit readiness | README.md | Skip | Skip | scenario did not require grammar target verification | PASS |
| I Push readiness | AGENT.md | Load | Load | router | PASS |
| I Push readiness | instructions/00-role-and-stack.md | Skip | Skip | scenario did not require stack/module context | PASS |
| I Push readiness | instructions/01-execution-and-planning.md | Load | Load | planning, approval, or Git gates | PASS |
| I Push readiness | instructions/02-architecture-and-modules.md | Skip | Skip | scenario did not require architecture/module boundaries | PASS |
| I Push readiness | instructions/03-cqrs-database-testing-security.md | Load | Load | database, verification, or security rules | PASS |
| I Push readiness | instructions/04-documentation-and-memory.md | Skip | Skip | scenario did not require prompt, memory, or ADR rules | PASS |
| I Push readiness | instructions/05-completion.md | Load | Load | review/readiness/verification gates | PASS |
| I Push readiness | instructions/06-loading-index.md | Load | Load | conditional routing index | PASS |
| I Push readiness | docs/project/PROMPT_TEMPLATE.md | Skip | Skip | scenario did not require plan, execution, or verification contract | PASS |
| I Push readiness | docs/project/PROJECT_STATUS.md | Skip | Skip | scenario did not require current state evidence | PASS |
| I Push readiness | docs/project/AI_HANDOFF.md | Load | Load | current constraints/security/verification evidence | PASS |
| I Push readiness | docs/project/ROADMAP.md | Skip | Skip | scenario did not require memory or architecture direction | PASS |
| I Push readiness | docs/project/NEXT_SESSION.md | Skip | Skip | scenario did not require memory/current-state warnings | PASS |
| I Push readiness | docs/decisions/ADR-*.md | Skip | Skip | scenario did not require ADR ownership search | PASS |
| I Push readiness | .agents/skills/architecture-decision-check/references/adr-review-checklist.md | Skip | Skip | scenario did not require materiality/ownership checklist | PASS |
| I Push readiness | docs/project/CODE_REVIEW.md | Skip | Skip | scenario did not require findings-first review criteria | PASS |
| I Push readiness | docs/project/ASSISTANT_TEXT_TO_SQL_READONLY_DB.md | Skip | Skip | scenario did not require Text-to-SQL runtime behavior evidence | PASS |
| I Push readiness | docs/agents/workflow/planning-sub-agent.md | Skip | Skip | scenario did not require planning responsibility profile | PASS |
| I Push readiness | docs/agents/workflow/execution-sub-agent.md | Skip | Skip | scenario did not require execution-start responsibility profile | PASS |
| I Push readiness | README.md | Skip | Skip | scenario did not require grammar target verification | PASS |
| J Secret handling | AGENT.md | Load | Load | router | PASS |
| J Secret handling | instructions/00-role-and-stack.md | Skip | Skip | scenario did not require stack/module context | PASS |
| J Secret handling | instructions/01-execution-and-planning.md | Load | Load | planning, approval, or Git gates | PASS |
| J Secret handling | instructions/02-architecture-and-modules.md | Load | Load | architecture/module boundaries | PASS |
| J Secret handling | instructions/03-cqrs-database-testing-security.md | Load | Load | database, verification, or security rules | PASS |
| J Secret handling | instructions/04-documentation-and-memory.md | Load | Load | prompt, memory, or ADR rules | PASS |
| J Secret handling | instructions/05-completion.md | Load | Load | review/readiness/verification gates | PASS |
| J Secret handling | instructions/06-loading-index.md | Load | Load | conditional routing index | PASS |
| J Secret handling | docs/project/PROMPT_TEMPLATE.md | Skip | Skip | scenario did not require plan, execution, or verification contract | PASS |
| J Secret handling | docs/project/PROJECT_STATUS.md | Load | Load | current state evidence | PASS |
| J Secret handling | docs/project/AI_HANDOFF.md | Load | Load | current constraints/security/verification evidence | PASS |
| J Secret handling | docs/project/ROADMAP.md | Skip | Skip | scenario did not require memory or architecture direction | PASS |
| J Secret handling | docs/project/NEXT_SESSION.md | Skip | Skip | scenario did not require memory/current-state warnings | PASS |
| J Secret handling | docs/decisions/ADR-*.md | Load | Load | ADR ownership search | PASS |
| J Secret handling | .agents/skills/architecture-decision-check/references/adr-review-checklist.md | Load | Load | materiality/ownership checklist | PASS |
| J Secret handling | docs/project/CODE_REVIEW.md | Load | Load | findings-first review criteria | PASS |
| J Secret handling | docs/project/ASSISTANT_TEXT_TO_SQL_READONLY_DB.md | Skip | Skip | scenario did not require Text-to-SQL runtime behavior evidence | PASS |
| J Secret handling | docs/agents/workflow/planning-sub-agent.md | Skip | Skip | scenario did not require planning responsibility profile | PASS |
| J Secret handling | docs/agents/workflow/execution-sub-agent.md | Skip | Skip | scenario did not require execution-start responsibility profile | PASS |
| J Secret handling | README.md | Skip | Skip | scenario did not require grammar target verification | PASS |
| K Grammar correction | AGENT.md | Load | Load | router | PASS |
| K Grammar correction | instructions/00-role-and-stack.md | Skip | Skip | scenario did not require stack/module context | PASS |
| K Grammar correction | instructions/01-execution-and-planning.md | Load | Load | planning, approval, or Git gates | PASS |
| K Grammar correction | instructions/02-architecture-and-modules.md | Skip | Skip | scenario did not require architecture/module boundaries | PASS |
| K Grammar correction | instructions/03-cqrs-database-testing-security.md | Skip | Skip | scenario did not require database, verification, or security rules | PASS |
| K Grammar correction | instructions/04-documentation-and-memory.md | Load | Load | prompt, memory, or ADR rules | PASS |
| K Grammar correction | instructions/05-completion.md | Load | Load | review/readiness/verification gates | PASS |
| K Grammar correction | instructions/06-loading-index.md | Load | Load | conditional routing index | PASS |
| K Grammar correction | docs/project/PROMPT_TEMPLATE.md | Skip | Skip | scenario did not require plan, execution, or verification contract | PASS |
| K Grammar correction | docs/project/PROJECT_STATUS.md | Skip | Skip | scenario did not require current state evidence | PASS |
| K Grammar correction | docs/project/AI_HANDOFF.md | Skip | Skip | scenario did not require current constraints/security/verification evidence | PASS |
| K Grammar correction | docs/project/ROADMAP.md | Skip | Skip | scenario did not require memory or architecture direction | PASS |
| K Grammar correction | docs/project/NEXT_SESSION.md | Skip | Skip | scenario did not require memory/current-state warnings | PASS |
| K Grammar correction | docs/decisions/ADR-*.md | Skip | Skip | scenario did not require ADR ownership search | PASS |
| K Grammar correction | .agents/skills/architecture-decision-check/references/adr-review-checklist.md | Skip | Skip | scenario did not require materiality/ownership checklist | PASS |
| K Grammar correction | docs/project/CODE_REVIEW.md | Skip | Skip | scenario did not require findings-first review criteria | PASS |
| K Grammar correction | docs/project/ASSISTANT_TEXT_TO_SQL_READONLY_DB.md | Skip | Skip | scenario did not require Text-to-SQL runtime behavior evidence | PASS |
| K Grammar correction | docs/agents/workflow/planning-sub-agent.md | Skip | Skip | scenario did not require planning responsibility profile | PASS |
| K Grammar correction | docs/agents/workflow/execution-sub-agent.md | Skip | Skip | scenario did not require execution-start responsibility profile | PASS |
| K Grammar correction | README.md | Load | Load | grammar target verification | PASS |
| L Ambiguous architecture | AGENT.md | Load | Load | router | PASS |
| L Ambiguous architecture | instructions/00-role-and-stack.md | Load | Load | stack/module context | PASS |
| L Ambiguous architecture | instructions/01-execution-and-planning.md | Load | Load | planning, approval, or Git gates | PASS |
| L Ambiguous architecture | instructions/02-architecture-and-modules.md | Load | Load | architecture/module boundaries | PASS |
| L Ambiguous architecture | instructions/03-cqrs-database-testing-security.md | Load | Load | database, verification, or security rules | PASS |
| L Ambiguous architecture | instructions/04-documentation-and-memory.md | Load | Load | prompt, memory, or ADR rules | PASS |
| L Ambiguous architecture | instructions/05-completion.md | Load | Load | review/readiness/verification gates | PASS |
| L Ambiguous architecture | instructions/06-loading-index.md | Load | Load | conditional routing index | PASS |
| L Ambiguous architecture | docs/project/PROMPT_TEMPLATE.md | Skip | Skip | scenario did not require plan, execution, or verification contract | PASS |
| L Ambiguous architecture | docs/project/PROJECT_STATUS.md | Load | Load | current state evidence | PASS |
| L Ambiguous architecture | docs/project/AI_HANDOFF.md | Load | Load | current constraints/security/verification evidence | PASS |
| L Ambiguous architecture | docs/project/ROADMAP.md | Load | Load | memory or architecture direction | PASS |
| L Ambiguous architecture | docs/project/NEXT_SESSION.md | Load | Load | memory/current-state warnings | PASS |
| L Ambiguous architecture | docs/decisions/ADR-*.md | Load | Load | ADR ownership search | PASS |
| L Ambiguous architecture | .agents/skills/architecture-decision-check/references/adr-review-checklist.md | Load | Load | materiality/ownership checklist | PASS |
| L Ambiguous architecture | docs/project/CODE_REVIEW.md | Skip | Skip | scenario did not require findings-first review criteria | PASS |
| L Ambiguous architecture | docs/project/ASSISTANT_TEXT_TO_SQL_READONLY_DB.md | Skip | Skip | scenario did not require Text-to-SQL runtime behavior evidence | PASS |
| L Ambiguous architecture | docs/agents/workflow/planning-sub-agent.md | Skip | Skip | scenario did not require planning responsibility profile | PASS |
| L Ambiguous architecture | docs/agents/workflow/execution-sub-agent.md | Skip | Skip | scenario did not require execution-start responsibility profile | PASS |
| L Ambiguous architecture | README.md | Skip | Skip | scenario did not require grammar target verification | PASS |

## Trigger Quality Review

| Skill | Rating | Finding |
|---|---|---|
| architecture-decision-check | GOOD | States decision purpose, material triggers, exclusions, and blocked behavior |
| branch-start-check | GOOD | Precise approved-execution trigger and explicit non-authorization exclusions |
| code-review-check | GOOD | Clear review trigger and proofreading/commit exclusions |
| commit-readiness | NEEDS_REFINEMENT | “Immediately before an explicitly approved commit” is narrower than explicit readiness-assessment requests |
| migration-safety-check | NEEDS_REFINEMENT | Unconditionally includes connection strings and requires approved scope even during planning |
| project-memory-update | NEEDS_REFINEMENT | Describes mutation near completion but not dry-run determination of required memory updates |
| prompt-log-writer | AMBIGUOUS | Lists planning/testing/review records but says use before approved execution, conflicting with global all-prompts rule |
| push-readiness | NEEDS_REFINEMENT | Does not declare verification, secret-scan, or commit-readiness evidence required by repository gates |
| secret-scan-check | GOOD | Clear credential/config triggers and non-disclosure exclusion |
| verification-runner | NEEDS_REFINEMENT | “Selects and runs” does not clearly cover dry-run verification planning |

Proposed descriptions, not applied:

- commit-readiness: Determines whether current scoped changes have the branch, verification, review, secret-scan, and artifact evidence required for a local commit. Use for explicit commit-readiness assessments and immediately before an approved local commit. Never grants commit permission or performs a commit.
- migration-safety-check: Reviews migrations, schema/raw-SQL/database-user changes, and connection-string changes that alter database scope or privilege. Use during planning or review; distinguish planning safety from separate creation/application approval. Use secret-scan-check alone for credential-only exposure.
- project-memory-update: Determines and, when authorized, applies factual project-memory updates after verified work changes repository state. Use for completion and dry-run memory-impact assessments; never record planned or unverified work as complete.
- prompt-log-writer: Determines and, when authorized, creates or finalizes chronological logs for repository planning, execution, testing, documentation, and review prompts. Use whenever the global prompt-log rule applies, while respecting the execution lock and explicit SKIP PROMPT LOG.
- push-readiness: Assesses a committed branch before a requested push using branch/upstream, clean worktree, outgoing commits, commit-readiness, verification, and secret-scan evidence plus explicit push approval. Never performs a push or creates a PR.
- verification-runner: Selects, reports, and when authorized runs proportionate verification. Use to plan verification after simulated work and to execute checks after implementation; label required, not-required, deferred, and blocked checks without claiming unrun results.

## Skill Overlap Review

| Pair | Shared concern | Primary owner | Dependency/evidence | Duplicate/conflict review |
|---|---|---|---|---|
| branch-start / verification | Safe lifecycle state | branch-start owns pre-implementation state; verification owns result evidence | Verification may record branch context but need not rerun start flow | No harmful duplication |
| branch-start / secret-scan | Pre-execution safety | Separate owners | None normally | No duplication |
| branch-start / commit-readiness | Branch identity and dirty status | branch-start at start; commit-readiness at end | Commit consumes branch provenance/status | Temporal recheck is intentional |
| branch-start / push-readiness | Branch and worktree status | push-readiness at push time | Push may reference start provenance | Temporal recheck is intentional |
| verification / secret-scan | Pre-commit evidence | Separate owners | Commit and push consume both | No duplicate workflow |
| verification / commit-readiness | Verification evidence | verification-runner | commit-readiness must consume a concrete result | Correct composition |
| verification / push-readiness | Verified outgoing state | verification-runner | Push Skill should require verification evidence | Missing dependency in push Skill |
| secret-scan / commit-readiness | Secret-free diff | secret-scan-check | Commit consumes scan result | Correct composition |
| secret-scan / push-readiness | Secret-free outgoing state | secret-scan-check | Push must consume current scan result | Repository rule requires it; push Skill omits it |
| commit-readiness / push-readiness | Scoped committed state | commit owns pre-commit; push owns post-commit remote gate | Push should reference commit-readiness/commits | Boundary is sound but dependency is implicit |

## Context Efficiency Review

| Scenario | Skills invoked | References | Instruction/docs loaded | Unnecessary loads | Correctly skipped | Rating |
|---|---:|---:|---:|---:|---|---|
| A | 1 | 1 | 20 | 1 orphan discovery; broad memory/source read | Execution, DB, review, completion | CONTEXT_LOADING: ACCEPTABLE |
| B | 1 | 1 | 24 | 0 | DB/migration/secret execution | CONTEXT_LOADING: ACCEPTABLE |
| C | 2 | 0 | 7 | 0 | Source, ADR, build, completion | CONTEXT_LOADING: EFFICIENT |
| D | 2 | 1 | 21 | 0 | Text-to-SQL DB, Auth/Orders, commands | CONTEXT_LOADING: ACCEPTABLE |
| E | 3 | 1 | 18 | 1 premature migration body | Full source/tests/migrations | CONTEXT_LOADING: ACCEPTABLE |
| F | 1 | 0 | 7 | 0 | Everything outside declared memory | CONTEXT_LOADING: EFFICIENT |
| G | 1 | 0 | 6 | 0 | Source/config/migrations/ADRs | CONTEXT_LOADING: EFFICIENT |
| H | 4 | 0 | 8 | 0 | Push, migration, memory mutation | CONTEXT_LOADING: EFFICIENT |
| I | 2 | 0 | 6 | Metadata command exposed body fragments | Source/config/full diff/ADRs | CONTEXT_LOADING: INEFFICIENT |
| J | 4 | 1 | 17 | 0, but broad overlap | Actual secrets, source/tests/migrations | CONTEXT_LOADING: ACCEPTABLE |
| K | 0 | 0 | 6 | 0 | All Skill bodies/references and technical docs | CONTEXT_LOADING: EFFICIENT |
| L | 1 | 1 | 19 | 1 non-content inventory pass | Source/deployment state/actions | CONTEXT_LOADING: ACCEPTABLE |

## Defects Found

### Over-triggering

- Scenario E loaded migration-safety-check before confirming the appsettings diff did not contain a connection string or database-scope change.
- Scenario J invoked migration-safety-check solely because “connection string” appears; credential-only review should primarily belong to secret-scan-check unless database scope/privilege changes.
- Architecture routing is intentionally broad for security/persistence proposals but causes four-Skill convergence in Scenario J.

### Under-triggering

- prompt-log-writer did not trigger in planning/documentation Scenarios A, B, D, and K despite instructions/04 declaring that all such prompts are logged.
- push-readiness does not require verification-runner or commit-readiness evidence in its own body.
- Scenario E omitted instructions/02-architecture-and-modules.md despite an architecture-compliance review.

### Incorrect output contracts

- secret-scan-check returns SECRET_SCAN, while Scenario J’s acceptance contract requires SECRET_SCAN_STATUS.
- verification-runner lacks explicit Required, Not required, Deferred, and Blocked categories needed for dry-run reporting.
- migration-safety-check cannot distinguish “planning is safe” from “migration creation/application is not authorized.”

### Broken references

No broken Skill reference links were found.

### Unnecessary or missing loading

- One obsolete empty .agents/skills/workflow/architecture-decision-check directory causes false discovery attempts.
- One isolated evaluator’s metadata command exposed body fragments in Scenario I.
- The loading index uses ambiguous shorthand push-readiness.md and migration-safety-check.md instead of canonical paths.

### Conflicting or stale instructions

- instructions/06-loading-index.md has an unterminated reporting-example code fence.
- instructions/04 requires every planning/documentation/review prompt to be logged, but prompt-log metadata centers on approved execution and the execution lock can prohibit the write.
- push-readiness omits the secret-scan dependency stated in instructions/05.
- PROJECT_STATUS says Catalog has two migrations but lists three; it says Auth has one but lists two.
- Prompt 097 remains APPROVED although its result summary records completed implementation/verification.
- AI_SKILLS_SUBAGENT_ARCHITECTURE retains “introduced later” runtime-sub-agent roadmap wording after Catalog and Orders sub-agents exist.

### Current-diff blocker

The pre-existing unrelated src/Api/Ecommerce.Api/appsettings.json change enables Assistant:TextToSql. It is outside Skill normalization/testing scope and lacks runtime verification in this worktree. This test did not alter it.

## Recommendations

1. Remove the obsolete empty workflow directory in a separately approved cleanup.
2. Close the loading-index code fence and replace shorthand Skill filenames with canonical .agents/skills/.../SKILL.md paths.
3. Align prompt-log-writer metadata with the global logging rule and execution-lock behavior.
4. Add verification, secret-scan, and commit-readiness evidence to push-readiness inputs/workflow/output.
5. Refine migration-safety triggering for credential-only connection-string changes and separate planning safety from mutation approval.
6. Extend verification-runner’s dry-run output categories without claiming commands ran.
7. Align SECRET_SCAN versus SECRET_SCAN_STATUS contract naming.
8. Update stale migration counts, prompt-097 lifecycle status, and runtime-sub-agent roadmap wording.
9. Keep the unrelated Text-to-SQL flag change out of any Skill-test/normalization commit unless separately approved and verified.
10. Add a deployment readiness/approval workflow only if future repeated deployment work proves it necessary; do not create a Skill from this single test.

## Test Status

All ten Skills were discovered and structurally validated. Every Skill received positive and negative routing coverage across twelve scenarios. Safe blocking worked; routing and contract defects are documented as recommendations only.

ECOMMERCE_SKILL_ROUTING_TEST_STATUS: COMPLETE

