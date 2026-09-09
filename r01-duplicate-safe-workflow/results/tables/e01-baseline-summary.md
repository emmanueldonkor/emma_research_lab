# E01 baseline summary

| Field | Value |
| --- | --- |
| Run date | 2026-09-08 |
| Runner | `scripts/e01_naive_duplicate_request.py` |
| Request name | `weekly-report` |
| Sequential submissions | 5 |
| Successful HTTP responses | 5 (`201 Created`) |
| Unique workflow IDs returned | 5 |
| Persisted records for the request name | 5 |

Interpretation: the E01 handler treats each HTTP submission as a new workflow because it has no idempotency mechanism. This result only establishes the basic replay failure mode; it does not measure concurrent contention, latency distributions, or failure recovery.
