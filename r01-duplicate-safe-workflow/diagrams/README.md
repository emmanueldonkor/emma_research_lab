# Diagrams

These diagrams document the implemented R01 design and controlled experiment paths.

- `system-architecture.svg` — application components and persisted state
- `idempotency-replay-flow.svg` — E01/E02/E04 request decisions
- `outbox-retry-state.svg` — E03/E05 transaction and retry states

They describe the local implementation only. No diagram implies an external broker, multi-instance deployment, or exactly-once processing.
