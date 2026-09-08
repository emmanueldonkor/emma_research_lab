# R03 - Reproducible evaluation for tabular classification

## Status

E01 through E04 have first recorded local runs.

## Question

How much can feature-selection leakage inflate a tabular-classification
evaluation, and what evidence is needed to make a small evaluation run
repeatable and inspectable?

## Scope

R03 uses two local datasets supplied by scikit-learn:

- the Wisconsin breast-cancer dataset as a fixed tabular reference dataset;
- a synthetic dataset with random labels and random features, where a good
  classifier should perform near chance.

The second dataset exists to make leakage visible under controlled conditions.
This project does not make medical or deployment claims from the breast-cancer
dataset.

## Experiments and records

1. E01 - repeat a correctly split and pipelined reference evaluation across
   fixed seeds.
2. E02 - compare feature selection performed before the split with selection
   fitted only on training data.
3. E03 - compare one reported split with variation across several fixed splits.
4. E04 - verify that a recorded run can be regenerated from its configuration,
   environment, and dataset fingerprint.

Read the [first-run findings](research/findings.md) for the comparison across
all four experiments. Raw JSON is in `results/raw/`, summary tables are in
`results/tables/`, and the runnable checks are in `scripts/`.

## Local setup

```powershell
python -m venv .venv
.\.venv\Scripts\python -m pip install -r requirements.txt
.\.venv\Scripts\python scripts\run_experiments.py --help
```

To verify the saved E01-E03 records in the pinned environment:

```powershell
.\.venv\Scripts\python scripts\verify_recorded_runs.py
```

## Project map

- `src/r03_eval/` — experiment definitions and environment metadata
- `scripts/` — raw-result generation and exact-record verification
- `experiments/` — conditions, procedure, and observed outputs for E01–E04
- `results/raw/` — versioned JSON records
- `results/tables/` — short comparisons of the recorded results
- `diagrams/` — leakage and split-variation diagrams
