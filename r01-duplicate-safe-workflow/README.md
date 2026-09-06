# R01 — Duplicate-safe workflow service

## Status

Core experiments complete; local evidence recorded.

## Question

How do idempotency keys and a transactional outbox change the duplicate and recovery behaviour of a small workflow service under controlled retries and failures?

## What this project contains

| Experiment | Controlled condition | Observed result |
| --- | --- | --- |
| E01 | Five sequential unkeyed replays | Five workflow records created |
| E02 | Five sequential replays with one idempotency key | One workflow record; replays returned the original |
| E03 | Workflow plus outbox event, with controlled rollback | Both records committed together or neither persisted |
| E04 | Twenty simultaneous replays with one key | One workflow record; 1 × `201`, 19 × `200` |
| E05 | Controlled dispatch failure then retry | Message remained pending after failure and published on retry |

The full comparison is in [`results/tables/experiment-comparison.md`](results/tables/experiment-comparison.md). Raw run outputs are versioned under `results/raw/`.

## Key finding from the recorded runs

The naive handler created one record per replay. With an idempotency key, both sequential and 20-way concurrent replays resolved to one persisted workflow record. The outbox experiments support a narrower durability claim: workflow state and a pending event can be committed or rolled back together, and a failed controlled delivery remains retryable.

This project does **not** establish exactly-once processing, external message-broker delivery, distributed multi-instance behavior, or real process-crash recovery.

## Local setup

```powershell
docker compose up -d database
dotnet run --project src/Workflow.Api
dotnet test tests/Workflow.Api.Tests/Workflow.Api.Tests.csproj
```

The API health check will be available at `http://localhost:8080/health`.

## Project map

- `experiments/` — experiment designs and recorded observations
- `results/raw/` — response-level outputs from runs
- `results/tables/` — concise evidence summaries
- `scripts/` — repeatable request runners
- `src/Workflow.Api/` — service implementation
- `tests/Workflow.Api.Tests/` — five automated invariant tests
