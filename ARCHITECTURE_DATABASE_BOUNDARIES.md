# Database Boundaries for Production

## Purpose

This document defines how `dados-tabulares` should coexist with other microservices when the platform moves toward a shared production environment.

It is intended as a baseline for architecture discussions, production setup, and developer guidance.

## Core Decision

Local development may use a dedicated PostgreSQL database per project. Production may use a single PostgreSQL server, and optionally a single shared database, as long as each microservice owns its own schemas and access boundaries.

For `dados-tabulares`, the service owns these schemas:

- `ibge`
- `tse`
- `pnad`

These schemas form one bounded context and must be treated as internal to this microservice, even if they live in a database shared with other services.

## Recommended Production Model

- One PostgreSQL server can host multiple microservices.
- Each microservice should own one or more schemas.
- Each microservice should connect with its own database role.
- Migrations for one service must only create or alter objects in schemas owned by that service.
- Cross-service access must happen through APIs, not direct table reads.

Example shared production database layout:

- `dados-tabulares` -> schemas `ibge`, `tse`, `pnad`
- `admin` -> schema `admin`
- `rag` -> schema `rag`
- `redes-sociais` -> schema `redes_sociais`

## Rules for Developers

- Do not write migrations that touch schemas owned by another microservice.
- Do not make one service depend on direct SQL reads from another service's schema.
- Keep credentials separated by service.
- Treat schema ownership as part of the service contract.
- If a service needs data from another service, expose or consume an API instead of joining across schemas.

## Local vs Production

Local development can stay simpler:

- each project may use its own local database or container;
- ports, credentials, and lifecycle can differ by project;
- local isolation is preferred over trying to mirror production perfectly.

Production can consolidate infrastructure:

- one PostgreSQL server;
- one shared database or multiple databases;
- strict schema ownership per microservice.

## Future Migration Path

If a microservice later needs stronger isolation, scale, or operational independence, it can move from a shared production database to its own dedicated database with minimal impact, provided the API boundary has been respected from the start.
