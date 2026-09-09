# E02 idempotency-key summary

| Field | Value |
| --- | --- |
| Run date | 2026-09-08 |
| Runner | `scripts/e02_idempotency_key.py` |
| Request name | `monthly-invoice` |
| Idempotency key | `e02-monthly-invoice-001` |
| Sequential submissions | 5 |
| First response | `201 Created` |
| Replay responses | 4 × `200 OK` |
| Unique workflow IDs returned | 1 |
| New workflow records for the key | 1 |
| Changed body using the same key | `409 Conflict` |

Interpretation: within this local sequential replay, persisting a unique idempotency key causes later matching requests to reuse the original workflow rather than insert duplicates. This does not yet establish behaviour under sustained concurrency or process failure.
