# Research questions

## Q01

When a client retries a request after losing the response, how often does a basic handler create duplicate workflow records?

## Q02

How does an idempotency key change duplicate-record behaviour?

## Q03

What failures remain when the workflow record and its event are written separately?

## Q04

How does a transactional outbox change event-loss and duplicate-delivery behaviour?
