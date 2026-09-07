# E04 circuit breaker summary

| Condition | Gateway result | Downstream effect |
| --- | --- | --- |
| First two unavailable responses | `503` passed through | One dependency call per request |
| Third unavailable response | `503`; circuit opens | Third dependency call is made |
| Next request during break window | `503` immediately | No dependency call |
| Healthy dependency after break window | `200` | One new dependency call; failure count resets |

Interpretation: in this implementation, the circuit breaker does not change
the first three failed calls. It prevents additional calls only after the
failure threshold is reached and only for its configured one-second window.
The recovery request shows that the gateway can resume when a later probe
succeeds.
