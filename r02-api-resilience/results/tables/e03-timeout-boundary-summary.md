# E03 timeout boundary summary

| Condition | Result |
| --- | --- |
| Dependency delay | 750 ms |
| Gateway attempt timeout | 250 ms |
| Caller outcome | `504` after 269 ms |
| Downstream requests | 1 |

Interpretation: the explicit timeout prevented the caller from waiting for the full configured downstream delay. It did not make the dependency healthy; it only bounded the gateway's wait.
