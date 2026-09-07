# Reproducibility

## Environment

- .NET SDK 8
- Two local ASP.NET Core services
- Python 3 for future experiment runners

## Baseline check

Start both services, set the dependency to `Healthy` through `POST /control`, and call `GET /proxy/baseline` through the gateway.

## E04 circuit-breaker check

Set the dependency to `Unavailable`, then make four quick calls to
`GET /proxy/circuit`. The first three calls should reach the dependency and
return `503`. The fourth should return `503` with `shortCircuited: true` and
`dependencyAttemptCount: 0`. Set the dependency back to `Healthy`, wait a little
over one second, and call the same endpoint again; it should return `200`.
