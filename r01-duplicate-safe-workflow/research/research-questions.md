# Research questions

## Q01

How does a basic handler behave under repeated identical requests?

E01 observed five records from five sequential unkeyed submissions.

## Q02

How does an idempotency key change duplicate-record behaviour under sequential and concurrent replays?

E02 and E04 observed one persisted record for five sequential and twenty concurrent keyed submissions, respectively.

## Q03

Does a database transaction keep workflow state and its pending outbox event aligned during a controlled rollback?

E03 observed that the rollback persisted neither record.

## Q04

Can a pending outbox event survive a controlled delivery failure and be marked published by a retry?

E05 observed that the event remained pending after failure and was published on retry. This does not measure broker delivery or consumer-side duplicate handling.
