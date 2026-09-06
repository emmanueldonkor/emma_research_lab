# E04 — Concurrent idempotency-key requests

## Purpose

Test whether the E02 idempotency implementation preserves one workflow when many callers submit the same logical request at the same time.

## Design

The runner releases 20 Python worker threads together. Every worker sends the same request name and the same generated `Idempotency-Key` to `POST /workflows`.

The expected safe outcome is one `201 Created`, later `200 OK` responses, and exactly one workflow identifier across all successful responses. The database primary key on the idempotency key resolves the create race; the application reloads the winning record after a uniqueness conflict.

## Procedure

1. Start PostgreSQL with `docker compose up -d database`.
2. Start the API with `dotnet run --project src/Workflow.Api --urls http://localhost:8080`.
3. Run `python scripts/e04_concurrent_idempotency.py --requests 20`.
4. Inspect `results/raw/e04-concurrent-idempotency.json`.

## Measurements

- Response-status count
- Distinct workflow identifiers returned
- Per-request end-to-end elapsed time
- Errors or unexpected response bodies

## First recorded run

On 2026-09-06, 20 worker threads submitted `concurrent-settlement` together with one generated idempotency key.

| Measure | Observed value |
| --- | --- |
| Concurrent submissions | 20 |
| `201 Created` responses | 1 |
| `200 OK` replay responses | 19 |
| Distinct workflow identifiers returned | 1 |
| Persisted workflow records with the submitted name | 1 |
| HTTP or transport errors | 0 |

The raw per-request output, including elapsed times, is in `results/raw/e04-concurrent-idempotency.json`.

## Status

Implemented and first local concurrent run recorded. This is a single-machine, 20-thread run; it is not a distributed load test.
