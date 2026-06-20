# Architecture And Modules

## CLEAN ARCHITECTURE RULES

Dependency direction:

API
-> Application
-> Domain

Infrastructure
-> Application
-> Domain

Domain must remain independent.

Forbidden:

Domain -> Application
Domain -> Infrastructure
Domain -> API

Application -> API

Infrastructure -> API

---

# MODULE RULES

Each module owns:

* Domain
* Application
* Infrastructure
* Contracts

A module owns its business logic.

A module owns its persistence model.

Modules must not access each other's internal implementation.

Cross-module communication must happen through:

* Contracts
* Public abstractions
* Explicit integration mechanisms

Never through internal project references.
