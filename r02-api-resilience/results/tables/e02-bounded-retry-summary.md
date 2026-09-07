# E02 bounded retry summary

| Condition | Result |
| --- | --- |
| Dependency unavailable | Three calls, then `503` after 231 ms |
| Dependency healthy | One call, then `200` |

Interpretation: the bounded retry policy did not improve a sustained outage. It multiplied one caller request into three downstream requests and added wait time. The next experiment will add explicit timeout boundaries rather than assuming a retry is enough.
