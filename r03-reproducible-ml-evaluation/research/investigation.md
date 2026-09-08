# Investigation

## Working question

How much can feature-selection leakage inflate a tabular-classification
evaluation, and what needs to be recorded for the result to be reproduced?

## Method

The project will keep an intentionally flawed feature-selection path next to a
safe `Pipeline` path. Both will use the same generated data, train/test split
seeds, model family, and metrics. The difference is when feature selection is
fitted.

Each run record will include the experiment name, configuration, fixed seeds,
dataset fingerprint, Python and package versions, per-seed metrics, and a
summary. A separate script will re-run a saved configuration and compare the
result with the recorded output.

## Claims policy

The synthetic experiment can demonstrate leakage in this implementation under
the stated settings. The breast-cancer run is a reproducible reference
exercise, not a clinical model or a general claim about real-world ML quality.
