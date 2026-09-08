# E03 - One split versus repeated splits

## Purpose

Show how much context a single held-out score leaves out, even when the model,
dataset, and evaluation procedure are otherwise fixed.

## Design

Run the safe E01 pipeline across 30 predetermined stratified train/test split
seeds. Record every score, then compare the distribution with the highest
single accuracy observed in that same set of splits. The highest value is kept
as an illustration of selection bias; it is not treated as the project result.

## Measurements

- Accuracy, balanced accuracy, and F1 for every split
- Mean, standard deviation, minimum, and maximum
- Difference between the highest individual accuracy and the mean

## First recorded run

On 2026-09-08, the safe reference pipeline was evaluated across 30 fixed
stratified split seeds. Mean accuracy was 0.978, with a population standard
deviation of 0.011. Individual scores ranged from 0.958 to 0.993.

| Measure | Accuracy |
| --- | ---: |
| Mean across 30 splits | 0.978 |
| Population standard deviation | 0.011 |
| Lowest split | 0.958 |
| Highest split | 0.993 |
| Highest split minus mean | 0.015 |

The highest split came from seed 6. It is retained in the raw record to show
the selection effect, not to represent the overall estimate.

The raw record is `results/raw/e03-split-variation-2026-09-08.json`.

## Status

Implemented and first local run recorded. Thirty predetermined splits are
still a small sample of possible evaluations, so the range is descriptive of
this design rather than a general uncertainty interval.
