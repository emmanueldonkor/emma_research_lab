# E05 - Concurrent callers and a no-queue limit

## Purpose

Compare a direct gateway path with a gateway path that allows only two
in-flight dependency calls. The dependency will be deliberately slow and six
callers will start at the same time.

## Design

`GET /proxy/limited` uses an in-memory semaphore with two permits and no queue.
Calls that cannot obtain a permit receive `429 Too Many Requests` immediately;
they do not call the dependency. The direct comparison uses
`GET /proxy/baseline`, which has no gateway limit.

## Measurements

- Caller status and elapsed time
- Number of accepted and rejected callers
- Dependency request count
- Difference between direct forwarding and the limited path

## First recorded run

On 2026-09-07, the dependency was configured as `Slow` with a 500 ms delay.
The runner started six callers together for each condition.

Without a limit, all six callers returned `200` and the dependency request
counter increased from zero to six. Their measured end-to-end times were
1090-1095 ms. With the two-permit limit, two callers returned `200` and four
returned `429`; the dependency request counter rose only from six to eight.
The two admitted gateway calls each measured about 503-507 ms at the gateway.
The rejected gateway responses recorded zero dependency attempts.

| Path | `200` callers | `429` callers | Added dependency calls |
| --- | ---: | ---: | ---: |
| Direct baseline | 6 | 0 | 6 |
| Two-permit, no queue | 2 | 4 | 2 |

The raw per-caller results are in `results/raw/e05-concurrency-limit.json`.
The flow is illustrated in `diagrams/e05-concurrency-limit.svg`.

## Notes on timing

The Python runner measures from its own request start to final response, so its
figures include local connection and scheduling overhead. The gateway response
also includes its measured dependency-call duration. The request counts and
the zero-attempt rejected responses are the stronger evidence for the
protection claim in this local experiment.

## Status

Implemented and first local run recorded. This is an in-memory, no-queue
limit for one gateway process. It does not coordinate limits between multiple
instances or decide which callers should be prioritised.
