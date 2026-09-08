# E02 feature-selection leakage summary

| Split seed | Leaked accuracy | Safe-pipeline accuracy |
| ---: | ---: | ---: |
| 11 | 0.860 | 0.540 |
| 23 | 0.900 | 0.500 |
| 37 | 0.800 | 0.500 |
| 41 | 0.820 | 0.600 |
| 59 | 0.820 | 0.580 |
| Mean | 0.840 | 0.544 |

The dataset has random labels. The difference is therefore evidence of the
evaluation procedure seeing test information, not a predictive relationship.
