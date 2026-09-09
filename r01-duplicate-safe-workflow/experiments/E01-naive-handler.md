# E01 — Naive request handler

## Purpose

Establish the baseline failure mode before introducing idempotency or transactional outbox logic.

## Design

`POST /workflows` accepts a JSON body with a `name`. The handler creates and persists a fresh workflow record for every accepted HTTP request. It does not inspect an idempotency key, derive a request fingerprint, or enforce a uniqueness rule for a logical request.

This is intentional. The expected outcome is that replaying the same logical request creates multiple workflow records.

## Procedure

1. Start PostgreSQL with `docker compose up -d database`.
2. Start the API with `dotnet run --project src/Workflow.Api --urls http://localhost:8080`.
3. Run `python scripts/e01_naive_duplicate_request.py --requests 5`.
4. Inspect `GET http://localhost:8080/workflows` and the generated JSON under `results/raw/`.

## Measurements

- Requests submitted
- Successful responses
- Distinct persisted workflow identifiers for one logical request
- End-to-end response status per attempt

## First recorded run

On 2026-09-08, the runner submitted `weekly-report` five times sequentially to the local API.

| Measure | Observed value |
| --- | --- |
| Requests submitted | 5 |
| HTTP 201 responses | 5 |
| Distinct workflow identifiers returned | 5 |
| Persisted workflow records with the submitted name | 5 |

The raw responses, including the five generated identifiers and timestamps, are in `results/raw/e01-naive-duplicate-request.json`.

## Status

Implemented and first baseline run recorded. This is a local sequential run, not a concurrency benchmark.
