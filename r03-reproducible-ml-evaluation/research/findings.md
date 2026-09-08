# Findings from the first local runs

## What the runs show

| Experiment | Condition | Observation |
| --- | --- | --- |
| E01 | Safe pipeline on five fixed reference-data splits | Mean accuracy was 0.979; the individual splits ranged from 0.958 to 1.000. |
| E02 | Random labels with feature selection before the split | Mean held-out accuracy was 0.840. |
| E02 | The same random labels with selection inside the training pipeline | Mean held-out accuracy was 0.544. |
| E03 | Safe pipeline on 30 fixed reference-data splits | Accuracy ranged from 0.958 to 0.993, with mean 0.978. |
| E04 | Regenerate saved E01-E03 records | All three regenerated objects matched their checked-in JSON records exactly. |

## Reading the results together

The E02 comparison is the clearest result. The labels were generated at random,
so there was no relationship for the classifier to discover. The high score
from the flawed path came from fitting the selector before the test partition
was protected. Putting the same selector inside the pipeline brought the score
back near chance.

E01 and E03 show a different point: even a safe procedure gives a set of
scores, not one permanent number. The best E03 split was 0.015 above the mean
of the 30 fixed splits. Keeping the distribution makes that difference visible
instead of silently reporting the best case.

E04 is about traceability. The saved run objects include the data fingerprint,
configuration, package versions, individual split scores, and summary. In the
pinned local environment, the verification script reproduced all of them
exactly.

## What this does not show

The synthetic leakage experiment is intentionally small and controlled. It does
not measure the size of leakage in a production dataset. The reference dataset
is included to exercise a normal tabular pipeline, not to make a medical claim.
The exact-match check also depends on the current platform and pinned package
versions; it is not a promise of identical floating-point results everywhere.

## Evidence

- [E01 reference pipeline](../experiments/E01-reference-pipeline.md)
- [E02 feature-selection leakage](../experiments/E02-feature-selection-leakage.md)
- [E03 split variation](../experiments/E03-split-variation.md)
- [E04 recorded-run verification](../experiments/E04-recorded-run-verification.md)
- [Raw results](../results/raw/)
