# R02 - API resilience under dependency failure

## Status

E01 through E05 have a first recorded local run.

## Question

What do timeout, bounded retry, and circuit-breaker policies change when a
service depends on an API that becomes slow or unavailable?

## Design

R02 contains two local .NET services:

- `Dependency.Api` simulates a healthy, slow, or unavailable downstream dependency.
- `Gateway.Api` calls that dependency and exposes one endpoint per policy.

The dependency is controlled through a local endpoint rather than a public
service so that each condition can be repeated.

## Experiments and records

1. [E01](experiments/E01-baseline.md) - baseline forwarding under healthy, slow, and unavailable modes.
2. [E02](experiments/E02-bounded-retry.md) - bounded retry and extra downstream attempts.
3. [E03](experiments/E03-timeout-boundary.md) - per-attempt timeout behaviour.
4. [E04](experiments/E04-circuit-breaker.md) - circuit opening and recovery.
5. [E05](experiments/E05-concurrency-limit.md) - concurrent callers and dependency-protection limits.

Read the [first-run findings](research/findings.md) for the comparison across
all five experiments. Raw response records are in `results/raw/`; concise
tables are in `results/tables/`.

## Local setup

Run the dependency in one terminal:

```powershell
dotnet run --project src/Dependency.Api --urls http://localhost:5091
```

Run the gateway in another:

```powershell
dotnet run --project src/Gateway.Api --urls http://localhost:5090
```

The baseline proxy endpoint is `GET http://localhost:5090/proxy/baseline`.

To verify the documented behaviours against running local services:

```powershell
python scripts/verify_gateway_behaviour.py
```

## Project map

- `src/Dependency.Api/` — controlled downstream service and its state
- `src/Gateway.Api/` — policy implementations and gateway endpoints
- `experiments/` — conditions, procedure, and observed outputs for E01–E05
- `results/raw/` — recorded response-level results
- `results/tables/` — short comparisons of the recorded results
- `scripts/` — repeatable pressure and verification runners
- `diagrams/` — concurrency-limit experiment diagram
