# E05 concurrency-limit summary

| Condition | Caller outcomes | Added downstream calls | Observation |
| --- | --- | ---: | --- |
| Six direct calls to a 500 ms dependency | 6 x `200` | 6 | Every caller added work to the dependency |
| Six calls through a two-permit, no-queue gateway | 2 x `200`, 4 x `429` | 2 | Four calls were refused before a dependency request |

Interpretation: the limit trades availability for a bound on local downstream
work. It protects the dependency from four additional calls in this run, but
it does not make the four rejected callers succeed. Choosing a different
limit, queue, or caller-priority rule would produce a different trade-off.
