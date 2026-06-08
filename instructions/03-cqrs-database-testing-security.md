# CQRS Database Testing Security

## CQRS RULES

Every write operation must use:

* Command
* CommandHandler
* Validator

Every read operation must use:

* Query
* QueryHandler
* DTO

Do not mix read and write responsibilities.

Controllers must never contain business logic.

Controllers may only:

* Receive requests
* Validate transport-level concerns
* Dispatch commands
* Dispatch queries
* Return responses

---

# DATABASE RULES

Use:

* SQL Server
* EF Core

Rules:

* No raw SQL unless explicitly approved
* No database access from controllers
* No migrations without approval
* No schema changes without approval

All persistence logic belongs in Infrastructure.

---

# TESTING RULES

A feature is not complete unless tests exist.

Testing hierarchy:

1. Unit Tests
2. Architecture Tests
3. Integration Tests (when introduced)

Every completed feature must include appropriate test coverage.

---

# ARCHITECTURE TEST RULES

Architecture tests must protect:

* Dependency direction
* Module isolation
* Forbidden references
* Naming conventions

Architecture tests are mandatory before major feature development.

---

# SECURITY RULES

Never:

* Store plain text passwords
* Expose internal exceptions
* Commit secrets
* Hardcode credentials

Always:

* Validate inputs
* Apply authorization policies
* Use secure defaults
