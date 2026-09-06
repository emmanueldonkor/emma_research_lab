# Reproducibility

## Environment

- .NET SDK 8
- PostgreSQL 17 in Docker
- Python 3.13 for experiment scripts

## Current verification

The initial verification is that the database container becomes healthy and the API returns a successful response from `/health`.

The database is exposed locally on port `5433` to avoid conflicting with an existing local PostgreSQL service.
