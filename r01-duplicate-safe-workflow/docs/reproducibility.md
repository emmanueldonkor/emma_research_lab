# Reproducibility

## Environment

- .NET SDK 8
- PostgreSQL 17 in Docker
- Python 3.13 for experiment scripts

## Current verification

The initial verification is that the database container becomes healthy and the API returns a successful response from `/health`.

The database is exposed locally on port `5433` to avoid conflicting with an existing local PostgreSQL service.

## Automated checks

Run the invariant test suite from the R01 directory:

```powershell
dotnet test tests/Workflow.Api.Tests/Workflow.Api.Tests.csproj
```

The suite uses an isolated in-memory SQLite database and checks the E01 duplicate baseline, E02 idempotency reuse and conflict handling, E03 rollback boundary, and E05 retry state transition. PostgreSQL-backed experiment scripts remain the evidence for the recorded local runs.
