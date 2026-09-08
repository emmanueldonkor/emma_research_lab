# E02 - Feature-selection leakage

## Purpose

Make the effect of feature-selection leakage visible on data with no real
signal.

## Design

Generate 200 rows with 10,000 random features and random binary labels. In the
flawed path, `SelectKBest(k=25)` is fitted on all rows before the train/test
split. In the safe path, the same selector and classifier live in a pipeline
that is fitted on the training partition only.

## Expected interpretation

The correct path should remain close to chance over several seeds. A higher
score from the flawed path is evidence that test-label information influenced
feature selection, not that the model discovered a predictive relationship.

## First recorded run

On 2026-09-08, the same random-label dataset was evaluated with five fixed
split seeds. The flawed path selected features using all 200 rows before the
split. Its mean accuracy was 0.840. The safe pipeline fitted selection only on
the training partition and had mean accuracy 0.544.

| Path | Mean accuracy | Mean balanced accuracy |
| --- | ---: | ---: |
| Feature selection before the split | 0.840 | 0.841 |
| Feature selection inside the training pipeline | 0.544 | 0.542 |

Because the labels are random, the higher leaked score is not a model finding
signal. It is an optimistic evaluation caused by allowing the feature selector
to use information from the eventual test rows.

The raw record is in `results/raw/e02-feature-selection-leakage-2026-09-08.json`.

## Status

Implemented and first local run recorded. This is a controlled demonstration,
not an estimate of how large leakage effects will be in every dataset.
