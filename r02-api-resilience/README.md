# R02 — API resilience under dependency failure

## Status

E01 through E04 have a first recorded local run. E05 remains planned.

## Question

What do timeout, bounded retry, and circuit-breaker policies change when a service depends on an API that becomes slow or unavailable?

## Design

R02 contains two local .NET services:

- `Dependency.Api` simulates a healthy, slow, or unavailable downstream dependency.
- `Gateway.Api` calls that dependency. Later experiments will add resilience policies one at a time.

The dependency is controlled through a local endpoint rather than a public service so that each condition is repeatable.

## Planned experiments

1. E01 — baseline request forwarding under healthy, slow, and unavailable dependency modes.
2. E02 — bounded retry and the extra downstream attempts it creates.
3. E03 — per-attempt and total timeout behaviour.
4. E04 — circuit breaker opening and recovery.
5. E05 — concurrent callers and dependency-protection limits.

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
