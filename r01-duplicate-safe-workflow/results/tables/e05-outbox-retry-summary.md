# E05 outbox retry summary

| Field | Value |
| --- | --- |
| Run date | 2026-09-08 |
| Target message | `workflow.created.v1` for `retryable-notification` |
| First dispatch | Controlled failure |
| State after failure | Pending, attempt 1, error recorded |
| Second dispatch | Successful controlled acknowledgement |
| State after retry | Published, attempt 2, error cleared |

Interpretation: the target message persisted through a controlled delivery failure and was eligible for a later retry. This does not establish delivery to an external broker or exactly-once consumption.
