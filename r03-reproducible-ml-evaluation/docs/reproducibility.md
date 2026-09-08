# Reproducibility

## Run contract

Raw result files will include the full configuration, split seeds, data
fingerprint, package versions, per-seed scores, and summary values. A saved
result is only considered reproducible when the same command can regenerate
the same metrics on the pinned dependency versions.

## Dataset handling

No downloaded training data is committed. Both datasets are generated or
loaded locally by scikit-learn. Their shape, feature names where available,
target names where available, and a SHA-256 fingerprint of values and labels
will be recorded with every run.

## First recorded runs

The initial E01 and E02 records use Python 3.13.12, NumPy 2.3.0, and
scikit-learn 1.9.0. Re-run them with:

```powershell
.\.venv\Scripts\python scripts\run_experiments.py e01
.\.venv\Scripts\python scripts\run_experiments.py e02
.\.venv\Scripts\python scripts\run_experiments.py e03
```

The commands write a raw JSON record under `results/raw/`. The current E01 and
E02 records include their dataset fingerprints, so a later verification step
can compare regenerated results against them.

## Verification command

```powershell
.\.venv\Scripts\python scripts\verify_recorded_runs.py
```

The script checks the complete regenerated objects against the saved E01 and
E02 records. It is intentionally strict: a changed dependency version, seed,
dataset value, or metric changes the result instead of being silently accepted.
