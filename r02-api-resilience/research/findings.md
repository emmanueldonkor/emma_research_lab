# Findings from the first local runs

## What changed with each policy

| Policy | Local condition | What the run showed |
| --- | --- | --- |
| Direct forwarding | Dependency delayed or unavailable | The caller waited for the dependency and received its status directly. |
| Bounded retry | Sustained `503` | One caller request became three dependency requests and still ended in `503`. |
| 250 ms timeout | 750 ms dependency delay | The gateway returned `504` after 269 ms instead of waiting for the full delay. |
| Circuit breaker | Three consecutive `503` responses | The next request was skipped for one second; a later healthy request closed the circuit. |
| Two-permit, no-queue limit | Six simultaneous callers, 500 ms delay | Two calls reached the dependency; four received `429` without adding downstream work. |

## Reading the results together

No policy repaired an unavailable dependency. Each policy changed a different
cost of waiting for or repeatedly calling it. Retry increased downstream work
when the outage persisted. The timeout limited a caller's wait. The circuit
breaker stopped further calls only after its threshold was reached. The
concurrency limit protected the dependency by refusing some callers upfront.

That makes the policy choice a trade-off, not a list of features to switch on.
The threshold, timeout, retry count, and limit used here are deliberately small
values for a local experiment; they are not defaults for another system.

## Evidence and boundaries

The detailed run records sit beside each experiment:

- [E01 baseline](../experiments/E01-baseline.md)
- [E02 bounded retry](../experiments/E02-bounded-retry.md)
- [E03 timeout](../experiments/E03-timeout-boundary.md)
- [E04 circuit breaker](../experiments/E04-circuit-breaker.md)
- [E05 concurrency limit](../experiments/E05-concurrency-limit.md)

Both services ran on one machine, and the circuit breaker and concurrency
limit keep state only in the gateway process. These runs do not test network
partitions, multiple gateway instances, persistent state, or production
traffic. The repository records those limits because they matter to the
meaning of the results.

## Contract check

The local behaviour verification script passed on 2026-09-07. Its output is
saved in `results/raw/gateway-behaviour-verification-2026-09-07.json`.
