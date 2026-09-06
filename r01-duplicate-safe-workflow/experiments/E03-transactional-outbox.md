# E03 — Transactional outbox

## Purpose

Test whether workflow state and a pending integration event are committed or rolled back as one database unit.

## Design

`POST /outbox-workflows` stages a workflow record and a `workflow.created.v1` message in `outbox_messages` inside one PostgreSQL transaction. The message remains pending (`publishedAtUtc = null`) for a future dispatcher; E03 does not claim to deliver it to an external broker.

The `simulateRollback` request option is a controlled test hook: it saves both rows inside the transaction, rolls the transaction back, and returns `503`. It is not a simulation of a process crash.

## Procedure

1. Start PostgreSQL with `docker compose up -d database`.
2. Start the API with `dotnet run --project src/Workflow.Api --urls http://localhost:8080`.
3. Submit `{"name":"contract-renewal"}` to `POST /outbox-workflows`.
4. Confirm a workflow and one pending outbox message exist.
5. Submit `{"name":"rollback-check","simulateRollback":true}`.
6. Confirm neither a workflow nor an outbox message exists for `rollback-check`.

## First recorded run

On 2026-09-06, the committed request produced one workflow and one pending outbox message. The controlled rollback returned `503`; afterward, no workflow or outbox message for `rollback-check` existed.

| Measure | Observed value |
| --- | --- |
| Committed workflow records for `contract-renewal` | 1 |
| Pending outbox messages after commit | 1 |
| Controlled rollback response | `503 Service Unavailable` |
| Pending outbox messages after rollback | 1 (unchanged) |
| `rollback-check` workflow or message persisted | No |

Raw output is recorded in `results/raw/e03-transactional-outbox.json`.

## Status

Implemented and first local transaction-boundary run recorded. A dispatcher, broker delivery, retry policy, and actual process-crash testing remain outside E03.
