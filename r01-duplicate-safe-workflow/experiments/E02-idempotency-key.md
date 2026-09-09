# E02 — Idempotency key

## Purpose

Test whether a stable client-supplied idempotency key prevents a repeated logical request from creating additional workflow records.

## Design

When `POST /workflows` includes an `Idempotency-Key` header, the service stores the key, a SHA-256 fingerprint of the request name, and the created workflow identifier in the same database save operation.

- First use of a key: creates a workflow and returns `201 Created`.
- Replay with the same key and same request name: returns the original workflow with `200 OK`.
- Replay with the same key but a different request name: returns `409 Conflict`.
- No key: follows the E01 naive baseline and creates a new workflow per request.

The database primary key on `idempotency_records.Key` is the concurrency guard. If two requests race to create the same key, the losing transaction reloads and returns the winner's workflow.

## Procedure

1. Start PostgreSQL with `docker compose up -d database`.
2. Start the API with `dotnet run --project src/Workflow.Api --urls http://localhost:8080`.
3. Run `python scripts/e02_idempotency_key.py --requests 5`.
4. Inspect `GET http://localhost:8080/workflows` and `results/raw/e02-idempotency-key.json`.

## Measurements

- HTTP status per repeated submission
- Distinct workflow identifiers returned for one key
- New persisted workflow records for one key
- Response on conflicting reuse of the key

## First recorded run

On 2026-09-08, the runner submitted `monthly-invoice` five times sequentially with the same key, `e02-monthly-invoice-001`.

| Measure | Observed value |
| --- | --- |
| First response | `201 Created` |
| Replay responses | 4 × `200 OK` |
| Distinct workflow identifiers returned | 1 |
| New workflow records for the key | 1 |
| Same key with a different request name | `409 Conflict` |

The replay responses are in `results/raw/e02-idempotency-key.json`; the separate conflicting-key check is in `results/raw/e02-key-conflict.json`.

## Status

Implemented and first baseline run recorded. This is a local sequential run; concurrent collision and process-failure scenarios remain for later experiments.
