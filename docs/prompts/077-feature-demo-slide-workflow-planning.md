# Prompt 077: Feature Demo Slide Workflow Planning

Date: 2026-06-22

Purpose: Plan a documentation workflow rule requiring feature-focused demo slide source files for future main feature implementations.

Full Prompt:

```text
PLAN MODE: Add automatic feature demo slide deliverables to the project workflow.

Goal:
For every main feature we implement, the agent must also create a presentation-ready slide source file focused on that feature, so I can demo it to the team without writing slides manually.

Requirements:
1. Do not modify code in this planning task.
2. Review the existing agent workflow:
   - AGENT.md
   - instructions/*
   - docs/project/PROMPT_TEMPLATE.md
   - docs/project/PROJECT_STATUS.md
   - docs/product/DEMO_SCRIPT.md
3. Decide the best location for this rule:
   - Prefer docs/project/PROMPT_TEMPLATE.md and/or instructions/04-documentation-and-memory.md.
   - Avoid expanding AGENT.md unless only a short router reference is needed.
4. Define when the slide file is required:
   - Required for every main feature, major platform capability, API module, integration, or demo-worthy backend behavior.
   - Not required for tiny fixes, typo fixes, internal refactors, or test-only cleanup unless explicitly requested.
5. Define the output location and naming convention.
   Recommended:
   - docs/demo/features/{feature-slug}-demo-slides.md
6. Define the required slide sections:
   - Feature title
   - Business purpose
   - Problem solved
   - Architecture overview
   - Implementation files
   - API/contracts involved
   - Database impact
   - Security/authorization behavior
   - Main sequence diagram
   - Demo script
   - Test evidence
   - Risks/tradeoffs
   - Q&A talking points
7. Require Mermaid diagrams where useful:
   - Architecture diagram
   - Sequence diagram
   - Data flow diagram when applicable
8. Require speaker cues for each slide.
9. Require the execution summary to mention the generated slide file.
10. Require project memory to reference the demo slide file when the feature changes project state.

Return a complete plan using the repository's mandatory Plan Output Contract.

PLAN_STATUS must be PENDING_APPROVAL.
```

Status: APPROVED

Result Summary:

The planning response recommended adding the rule to `docs/project/PROMPT_TEMPLATE.md` and `instructions/04-documentation-and-memory.md`, keeping `AGENT.md` unchanged unless a short router reference becomes necessary. The plan defined trigger rules, naming convention, required slide sections, Mermaid expectations, speaker cues, execution summary requirements, and project memory updates.
