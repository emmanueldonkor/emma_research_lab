# R01 — Duplicate-safe workflow service

## Status

Setup.

## Question

How do idempotency keys and a transactional outbox change the correctness and recovery behaviour of a workflow service when requests are retried or processes fail?

## What this project will compare

1. A basic request handler.
2. A handler with idempotency keys.
3. A handler with idempotency keys and a transactional outbox.

## What will be measured

- duplicate workflow records
- lost events
- duplicate event delivery
- request latency
- recovery after an injected failure

No results are recorded yet. The first task is to make the local environment repeatable.

## Local setup

```powershell
docker compose up -d database
dotnet run --project src/Workflow.Api
```

The API health check will be available at `http://localhost:8080/health`.
