from __future__ import annotations

from statistics import mean

import numpy as np
from sklearn.datasets import load_breast_cancer
from sklearn.feature_selection import SelectKBest, f_classif
from sklearn.linear_model import LogisticRegression
from sklearn.metrics import accuracy_score, balanced_accuracy_score, f1_score
from sklearn.model_selection import train_test_split
from sklearn.pipeline import make_pipeline
from sklearn.preprocessing import StandardScaler

from r03_eval.metadata import environment, fingerprint

SPLIT_SEEDS = (11, 23, 37, 41, 59)
TEST_SIZE = 0.25


def reference_pipeline_run() -> dict[str, object]:
    dataset = load_breast_cancer()
    features = np.asarray(dataset.data)
    target = np.asarray(dataset.target)
    per_seed = []

    for split_seed in SPLIT_SEEDS:
        train_features, test_features, train_target, test_target = train_test_split(
            features, target, test_size=TEST_SIZE, stratify=target, random_state=split_seed
        )
        model = make_pipeline(
            StandardScaler(),
            LogisticRegression(max_iter=2_000, random_state=0),
        )
        model.fit(train_features, train_target)
        prediction = model.predict(test_features)
        per_seed.append({
            "split_seed": split_seed,
            "accuracy": accuracy_score(test_target, prediction),
            "balanced_accuracy": balanced_accuracy_score(test_target, prediction),
            "f1": f1_score(test_target, prediction),
        })

    return {
        "experiment": "E01-reference-pipeline",
        "configuration": {
            "split_seeds": list(SPLIT_SEEDS),
            "test_size": TEST_SIZE,
            "model": "StandardScaler + LogisticRegression(max_iter=2000, random_state=0)",
        },
        "dataset": {
            "source": "sklearn.datasets.load_breast_cancer",
            "shape": list(features.shape),
            "target_names": list(dataset.target_names),
            "fingerprint_sha256": fingerprint(features, target),
        },
        "environment": environment(),
        "per_seed": per_seed,
        "summary": _metric_summary(per_seed),
    }


def feature_selection_leakage_run() -> dict[str, object]:
    generator = np.random.default_rng(20_260_908)
    features = generator.standard_normal((200, 10_000))
    target = generator.integers(0, 2, size=200)
    per_seed = []

    for split_seed in SPLIT_SEEDS:
        flawed_features = SelectKBest(f_classif, k=25).fit_transform(features, target)
        flawed_train, flawed_test, train_target, test_target = train_test_split(
            flawed_features, target, test_size=TEST_SIZE, stratify=target, random_state=split_seed
        )
        flawed_model = LogisticRegression(max_iter=2_000, random_state=0)
        flawed_model.fit(flawed_train, train_target)
        flawed_prediction = flawed_model.predict(flawed_test)

        safe_train, safe_test, safe_train_target, safe_test_target = train_test_split(
            features, target, test_size=TEST_SIZE, stratify=target, random_state=split_seed
        )
        safe_model = make_pipeline(
            SelectKBest(f_classif, k=25),
            LogisticRegression(max_iter=2_000, random_state=0),
        )
        safe_model.fit(safe_train, safe_train_target)
        safe_prediction = safe_model.predict(safe_test)

        per_seed.append({
            "split_seed": split_seed,
            "flawed_accuracy": accuracy_score(test_target, flawed_prediction),
            "safe_accuracy": accuracy_score(safe_test_target, safe_prediction),
            "flawed_balanced_accuracy": balanced_accuracy_score(test_target, flawed_prediction),
            "safe_balanced_accuracy": balanced_accuracy_score(safe_test_target, safe_prediction),
        })

    return {
        "experiment": "E02-feature-selection-leakage",
        "configuration": {
            "dataset_seed": 20_260_908,
            "samples": 200,
            "features": 10_000,
            "selected_features": 25,
            "split_seeds": list(SPLIT_SEEDS),
            "test_size": TEST_SIZE,
            "model": "LogisticRegression(max_iter=2000, random_state=0)",
        },
        "dataset": {
            "source": "locally generated random features and random binary labels",
            "shape": list(features.shape),
            "fingerprint_sha256": fingerprint(features, target),
        },
        "environment": environment(),
        "per_seed": per_seed,
        "summary": {
            "flawed_accuracy_mean": mean(row["flawed_accuracy"] for row in per_seed),
            "safe_accuracy_mean": mean(row["safe_accuracy"] for row in per_seed),
            "flawed_balanced_accuracy_mean": mean(row["flawed_balanced_accuracy"] for row in per_seed),
            "safe_balanced_accuracy_mean": mean(row["safe_balanced_accuracy"] for row in per_seed),
        },
    }


def _metric_summary(per_seed: list[dict[str, object]]) -> dict[str, float]:
    return {
        f"{metric}_mean": mean(float(row[metric]) for row in per_seed)
        for metric in ("accuracy", "balanced_accuracy", "f1")
    }
