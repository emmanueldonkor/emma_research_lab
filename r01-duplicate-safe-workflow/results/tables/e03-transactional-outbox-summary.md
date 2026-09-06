# E03 transactional outbox summary

| Field | Value |
| --- | --- |
| Run date | 2026-09-06 |
| Committed request | `contract-renewal` |
| Workflow rows created | 1 |
| Pending outbox rows created | 1 |
| Controlled rollback response | `503 Service Unavailable` |
| Workflow/outbox rows from rollback request | 0 / 0 |

Interpretation: this local run supports the transaction-boundary invariant: the workflow and its pending event were committed together, and both were removed by a deliberate rollback. It is not evidence of external event delivery or crash recovery.
