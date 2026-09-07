# E04 — Circuit breaker and recovery

## Purpose

Measure whether repeated dependency failures cause the gateway to stop issuing new calls for a short period, and whether it resumes after the dependency recovers.

## Design

The R02 circuit opens after three consecutive `5xx` dependency responses and remains open for one second. While open, the gateway returns `503` without calling the dependency. After the break window, one call is allowed again.

## Measurements

- Number of failures required to open the circuit
- Dependency request count before and during the open window
- Gateway response when short-circuited
- Recovery response after the dependency becomes healthy

## First recorded run

On 2026-09-07, the dependency was set to `Unavailable`. Three consecutive
gateway calls received `503` responses from the dependency. The third failure
opened the circuit. A fourth request made during the one-second break window
was returned immediately by the gateway with `503`; the dependency request
counter remained at three.

The dependency was then returned to `Healthy`. After waiting for the break
window to expire, the next request reached the dependency and returned `200`.

| Step | Gateway result | Dependency calls | Circuit state |
| --- | --- | --- | --- |
| First failure | `503` after 99 ms | 1 | closed, 1 failure |
| Second failure | `503` after 3 ms | 2 | closed, 2 failures |
| Third failure | `503` after 2 ms | 3 | open, 3 failures |
| Request while open | `503` after 0 ms | 3 | open; call skipped |
| Recovery request | `200` after 2 ms | 4 | closed, failures reset |

The raw response trace is in `results/raw/e04-circuit-breaker.json`.

## Status

Implemented and first local run recorded. This is a small in-memory circuit
breaker for a controlled local setup. Its state is intentionally lost when the
gateway restarts, and the result does not establish behaviour across multiple
gateway instances.
