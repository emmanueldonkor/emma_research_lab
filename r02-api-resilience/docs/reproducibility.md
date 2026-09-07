# Reproducibility

## Environment

- .NET SDK 8
- Two local ASP.NET Core services
- Python 3 for future experiment runners

## Baseline check

Start both services, set the dependency to `Healthy` through `POST /control`, and call `GET /proxy/baseline` through the gateway.
