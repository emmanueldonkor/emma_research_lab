# Research questions

## Q01

How does a direct dependency call behave when the downstream service is healthy, slow, or unavailable?

## Q02

How many additional downstream attempts does a bounded retry policy create, and when does that improve the caller outcome?

## Q03

How do per-attempt and total request timeouts bound caller wait time?

## Q04

When does a circuit breaker stop new dependency calls, and how does it recover after the dependency becomes healthy?

## Q05

When several callers arrive while a dependency is slow, how does a no-queue concurrency limit change the number of downstream calls and the outcome for each caller?
