---
name: secret-scan-check
description: Scans changed files and diffs for API keys, tokens, passwords, credentials, private connection strings, bearer values, and unsafe sensitive examples. Use before every commit and before push readiness, and whenever configuration or credential handling changes. Do not expose suspected secret values in the report.
---

# Secret Scan Check

## Required input

Obtain the changed-file list and complete diff, including untracked files intended for the task.

## Workflow

1. Read security rules in `instructions/03-cqrs-database-testing-security.md` and secret-handling warnings in `docs/project/AI_HANDOFF.md`.
2. Inspect configuration, environment examples, logs, documentation, fixtures, scripts, and source changes for secret-like values.
3. Distinguish clear placeholders such as `<API_KEY>` from real-looking credentials. Treat JWTs, provider keys, bearer tokens, passwords, private keys, and credential-bearing connection strings as blockers.
4. Check that `appsettings*.json` contains only safe non-secret defaults.
5. Report file locations and categories without repeating sensitive values.

## Output contract

```text
Files checked:
Findings:
Redaction required:
SECRET_SCAN_STATUS: PASS | BLOCKED
```

## Validation scenarios

- Positive: pre-commit scan of all changed and untracked task files -> `PASS` when clean.
- Negative: unrelated prose question with no repository changes -> do not trigger.
- Valid output: reports categories and locations without echoing a secret.
- Blocked: real or plausible credential, missing diff coverage, or unreadable intended file -> `BLOCKED`.
