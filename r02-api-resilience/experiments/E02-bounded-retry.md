# E02 — Bounded retry for a safe read

## Purpose

Measure how a small, fixed retry policy changes the result and downstream request count when a dependency is unavailable.

## Design

`GET /proxy/retry` attempts the dependency request at most three times, waiting 50 ms between failed `5xx` responses. The policy applies only to this safe `GET` endpoint; it is not used for writes.

## Measurements

- Gateway response status
- Number of gateway attempts
- Dependency request count
- End-to-end gateway elapsed time

## Expected trade-off

A retry may help when a failure is brief, but it also turns one caller request into multiple downstream requests when the dependency remains unavailable.

## First recorded run

On 2026-09-07, the unavailable dependency was called through the retry endpoint. The gateway made all three allowed attempts, then returned `503` after 231 ms. The dependency request counter increased by three.

After switching the dependency back to healthy, the same retry endpoint succeeded in one attempt.

| Dependency state | Gateway status | Gateway attempts | Dependency calls | Gateway elapsed time |
| --- | --- | --- | --- | --- |
| Unavailable | `503` | 3 | 3 | 231 ms |
| Healthy | `200` | 1 | 1 | Not separately recorded |

The raw result is in `results/raw/e02-bounded-retry.json`.

## Status

Implemented and first local run recorded. The result shows retry amplification under a sustained failure; it does not test a brief failure that recovers between attempts.
