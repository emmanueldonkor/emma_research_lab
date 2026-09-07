# E03 — Attempt timeout boundary

## Purpose

Measure whether a gateway with an explicit 250 ms dependency-attempt timeout bounds caller wait time when the dependency responds more slowly.

## Design

`GET /proxy/timeout` uses a linked cancellation token with `CancelAfter(250 ms)` for one dependency request. A timeout returns `504 Gateway Timeout`; it does not retry.

## Measurements

- Gateway status
- Gateway elapsed time
- Dependency request count
- Difference between configured downstream delay and caller wait time

## First recorded run

On 2026-09-07, the dependency was configured to delay for 750 ms. The gateway returned `504 Gateway Timeout` after 269 ms, made one attempt, and the dependency request counter increased by one.

| Configured dependency delay | Gateway timeout limit | Gateway status | Gateway elapsed time | Dependency calls |
| --- | --- | --- | --- | --- |
| 750 ms | 250 ms | `504` | 269 ms | 1 |

The raw result is in `results/raw/e03-timeout-boundary.json`.

## Status

Implemented and first local run recorded. The measured time includes local HTTP and cancellation overhead, which explains why it is slightly above the configured 250 ms limit.
