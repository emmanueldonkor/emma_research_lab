# E01 baseline summary

| Condition | Result |
| --- | --- |
| Healthy dependency | One successful direct call in 92 ms |
| Slow dependency | One successful direct call in 755 ms after a configured 750 ms delay |
| Unavailable dependency | One direct call returned `503` |

Interpretation: without a resilience policy, the gateway exposes downstream delay and status directly to its caller. Later experiments will test whether added policies improve the caller outcome without creating unnecessary downstream load.
