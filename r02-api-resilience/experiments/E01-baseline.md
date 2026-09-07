# E01 — Direct dependency call

## Purpose

Establish the behaviour of a gateway that forwards one request to the dependency without retry, timeout policy, or circuit breaker.

## Conditions

- Healthy dependency
- Slow dependency with an explicit delay
- Unavailable dependency returning `503`

## Measurements

- Gateway response status
- Gateway elapsed time
- Dependency request count

## First recorded run

On 2026-09-07, the local dependency was configured through each mode and called once through the baseline gateway.

| Dependency mode | Gateway status | Gateway elapsed time | Dependency calls |
| --- | --- | --- | --- |
| Healthy | `200 OK` | 92 ms | 1 |
| Slow (750 ms delay) | `200 OK` | 755 ms | 1 |
| Unavailable | `503 Service Unavailable` | No timeout or retry applied | 1 |

The raw result is in `results/raw/e01-baseline-modes.json`.

## Status

Implemented and first baseline run recorded. This is the direct-call comparison; it intentionally has no retry, timeout policy, or circuit breaker.
