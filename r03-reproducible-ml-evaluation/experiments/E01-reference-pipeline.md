# E01 - Repeated reference pipeline

## Purpose

Establish a small, correctly evaluated tabular-classification reference run
before introducing the leakage comparison.

## Design

Load the scikit-learn Wisconsin breast-cancer dataset. For each fixed split
seed, stratify a 75/25 train/test split and fit a `Pipeline` containing a
`StandardScaler` and logistic regression. The scaler is fitted only on the
training partition through the pipeline.

## Measurements

- Accuracy, balanced accuracy, and F1 for each split seed
- Mean and range across the configured seeds
- Dataset fingerprint and dependency versions

## First recorded run

On 2026-09-08, five fixed stratified split seeds were run against the local
reference dataset. Mean accuracy was 0.979, mean balanced accuracy was 0.975,
and mean F1 was 0.984. The individual accuracy values ranged from 0.958 to
1.000 across the five splits.

| Metric | Mean across five seeds |
| --- | ---: |
| Accuracy | 0.979 |
| Balanced accuracy | 0.975 |
| F1 | 0.984 |

The raw record includes the dataset fingerprint, package versions, per-seed
scores, and configuration: `results/raw/e01-reference-pipeline-2026-09-08.json`.

## Status

Implemented and first local run recorded. This is a reference evaluation on a
small bundled dataset; it is not a clinical or deployment result.
