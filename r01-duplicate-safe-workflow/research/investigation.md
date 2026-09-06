# Investigation

## Working question

How do idempotency keys and a transactional outbox change the correctness and recovery behaviour of a workflow service when requests are retried or processes fail?

## Scope

The service will accept workflow requests, persist them in PostgreSQL, and eventually publish a corresponding event. Experiments will introduce retries, lost responses, and controlled process failures.

## Claims policy

This document will distinguish planned work, observed output, and interpretation. Results are not added until a repeatable run produces them.
