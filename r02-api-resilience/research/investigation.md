# Investigation

## Working question

What do timeout, bounded retry, and circuit-breaker policies change when a service depends on an API that becomes slow or unavailable?

## Scope

The dependency and gateway both run locally. The dependency exposes repeatable healthy, slow, and unavailable modes. Each experiment will change one gateway policy and record caller outcomes, elapsed time, and dependency request count.

## Claims policy

Local experiments can show the behaviour of this implementation under stated conditions. They will not be used to claim production-scale reliability, vendor availability, or distributed fault tolerance.
