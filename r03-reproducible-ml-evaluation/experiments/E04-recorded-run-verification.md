# E04 - Recorded-run verification

## Purpose

Check whether the E01, E02, and E03 JSON records can be regenerated exactly from the
recorded configuration in the pinned local environment.

## Design

The verification script reruns each experiment, loads its checked-in JSON
record, and compares the complete generated object: configuration, environment,
dataset fingerprint, per-seed metrics, and summary.

## First recorded verification

On 2026-09-08, the verification script regenerated E01, E02, and E03 with the
pinned local environment and found an exact object match for all three checked-in
JSON records. This includes the recorded package versions, fingerprints,
per-seed metrics, and summaries.

The verification output is saved in
`results/raw/recorded-run-verification-2026-09-08.json`.

## Status

Implemented and first local verification recorded. An exact match here depends
on using the same dataset generation code and dependency versions; it does not
promise bit-for-bit reproducibility on every future platform or library build.
