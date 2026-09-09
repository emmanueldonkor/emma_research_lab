# E04 concurrent idempotency summary

| Field | Value |
| --- | --- |
| Run date | 2026-09-08 |
| Concurrent submissions | 20 |
| Shared idempotency key | `e04-concurrent-d8f0911d0c7d451f8772e20622a7ecbd` |
| `201 Created` responses | 1 |
| `200 OK` responses | 19 |
| Unique workflow IDs returned | 1 |
| Matching persisted workflow records | 1 |
| Errors | 0 |

Interpretation: under this local concurrent replay, the idempotency-key unique constraint and race-reload path preserved one workflow record. This is one trial, not a claim about multi-instance or distributed failure behaviour.
