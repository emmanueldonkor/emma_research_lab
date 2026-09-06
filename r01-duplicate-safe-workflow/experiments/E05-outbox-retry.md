# E05 — Outbox retry after controlled delivery failure

## Purpose

Test whether a pending outbox message remains durable after a delivery failure and can be published by a later retry.

## Design

`POST /outbox-dispatch` reads pending messages. With `simulateFailure: true`, it increments `deliveryAttempts`, records a failure reason, and leaves each message unpublished. A later normal dispatch marks the message published and clears the error.

The delivery acknowledgement is in-process and controlled. This experiment tests persistence and retry state, not an external message broker's delivery guarantee.

## First recorded run

On 2026-09-06, `retryable-notification` created one outbox message. The first dispatch used the controlled failure mode; the target message remained pending with `deliveryAttempts = 1` and an error. The next dispatch marked it published with `deliveryAttempts = 2` and no remaining error.

An earlier E03 message was also pending, so each dispatch processed two messages. The measurements below are for the E05 target message.

| Measure | Observed value |
| --- | --- |
| Target message created | 1 |
| After simulated failure | unpublished, attempt 1, error recorded |
| After retry | published, attempt 2, error cleared |
| Target message lost | No |

Raw output is recorded in `results/raw/e05-outbox-retry.json`.

## Status

Implemented and first local retry run recorded. Broker delivery, retry scheduling/backoff, and duplicate delivery to a consumer remain outside this experiment.
