# Verification audit - 2026-09-08

This audit checks the existing project verification paths. It does not rerun
every historical experiment or widen the claims recorded in those projects.

| Project | Command or check | Result |
| --- | --- | --- |
| R01 | `dotnet test tests/Workflow.Api.Tests/Workflow.Api.Tests.csproj` | 5 passed, 0 failed, 0 skipped |
| R02 | Build both APIs, start them locally, then run `python scripts/verify_gateway_behaviour.py` | Passed: baseline `200`; retry made 3 attempts; timeout returned `504`; circuit short-circuited and recovered; limiter returned 2 x `200` and 4 x `429` |
| R03 | `.\.venv\Scripts\python scripts\verify_recorded_runs.py` | E01, E02, and E03 each regenerated as an exact match to their saved JSON record |

The R02 local processes used for the check were stopped after the script
finished. The individual projects retain their own raw experiment records and
their stated limitations.
