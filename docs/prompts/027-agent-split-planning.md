# Prompt 027 - Agent Split Planning

## Date

2026-06-08

## Purpose

Plan splitting AGENT.md into a short router plus dedicated instruction files while preserving the V2 rule set.

## Full Prompt

PLAN MODE

Using current AGENT.md, plan splitting AGENT.md into a small router plus instruction files.

Goal:
Reduce AGENT.md size while preserving all rules exactly.

Also create/update:
docs/prompts/027-agent-split-planning.md

Do not execute.

Requirements:
- AGENT.md becomes the short entry point/router.
- Move detailed rules into instructions/*.md.
- Use stable file/section references, not line numbers.
- Preserve all existing rules.
- If AGENT.md changes, provide full replacement.
- Do not modify source code, tests, migrations, projects, or modules.

Return:
1. Problem
2. Proposed Instruction Structure
3. AGENT.md Router Design
4. Rule Preservation Strategy
5. Files Affected
6. Risks
7. Execution Checklist

PLAN_STATUS: PENDING_APPROVAL

## Status

APPROVED

## Result Summary

Planned splitting AGENT.md into a short router and six instruction files under instructions/, with stable file and section references.
