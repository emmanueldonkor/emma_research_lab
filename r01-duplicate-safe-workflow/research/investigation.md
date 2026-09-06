# Investigation

## Working question

How do idempotency keys and a transactional outbox change the duplicate and recovery behaviour of a workflow service under controlled retries and failures?

## Scope

The service accepts workflow requests, persists them in PostgreSQL, and stages a corresponding event in an outbox. The completed experiments cover sequential and concurrent retries, controlled transaction rollback, and controlled dispatch failure/retry.

This scope excludes an external broker, multi-instance deployment, and actual process termination during a commit or dispatch.

## Claims policy

This document distinguishes controlled observation from broader claims. Raw output and run summaries are stored under `results/`; the comparison table is `results/tables/experiment-comparison.md`.
