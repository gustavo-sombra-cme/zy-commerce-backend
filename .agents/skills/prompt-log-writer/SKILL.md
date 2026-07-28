---
name: prompt-log-writer
description: Creates the next chronological repository prompt log for repository planning, approved execution, artifact-producing testing, documentation changes, Skill maintenance, review, or global workflow-policy work. Use before the governed work unless SKIP PROMPT LOG is explicit, then finalize the same new log at completion. Do not log general explanation-only conversations, rewrite historical logs, or reuse a number.
---

# Prompt Log Writer

## Required input

Obtain the prompt text, date, purpose, work category, lifecycle status, and verified result summary when available.

## Workflow

1. Read the authoritative prompt logging rule in `instructions/04-documentation-and-memory.md`.
2. Trigger for repository planning, approved execution, testing that creates or changes a repository artifact, documentation changes, repository Skill maintenance, findings-first repository review, or global workflow-policy work.
3. Do not trigger for general explanation, advice, or read-only questions that create no repository artifact, or when the user explicitly writes `SKIP PROMPT LOG`.
4. Enumerate every filename under `docs/prompts/`; select one greater than the highest numeric prefix, even when earlier numbers are duplicated.
5. Before the governed work begins, create `docs/prompts/NNN-kebab-case-title.md` with Prompt Number, Date, Purpose, Full Prompt, Status, and Result Summary. Creating the log records planning; it does not authorize implementation.
6. Use only `PLANNED`, `APPROVED`, `EXECUTED`, or `FAILED`. Planning starts as `PLANNED`; explicitly approved execution starts as `APPROVED`.
7. At completion, update only the log created for the current task with the final status and factual result. Never restyle or renumber historical logs.

## Output contract

```text
Prompt log:
Number:
Status:
PROMPT_LOG: PASS | BLOCKED
```

## Validation scenarios

- Positive: repository planning or approved execution without `SKIP PROMPT LOG` -> create the next log and return `PASS`.
- Negative: general explanation with no repository artifact, or explicit `SKIP PROMPT LOG` -> do not trigger.
- Valid output: number is strictly above all existing numeric prefixes and content has every required field.
- Blocked: duplicate number, missing full prompt, ambiguous current-task log, or attempted historical rewrite -> `BLOCKED`.
