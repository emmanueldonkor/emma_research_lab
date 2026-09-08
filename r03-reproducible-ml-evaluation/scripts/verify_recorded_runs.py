from __future__ import annotations

import json
import sys
from pathlib import Path
from typing import Callable

ROOT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(ROOT / "src"))

from r03_eval.experiments import feature_selection_leakage_run, reference_pipeline_run, split_variation_run


def verify(filename: str, run: Callable[[], dict[str, object]]) -> dict[str, str]:
    recorded_path = ROOT / "results" / "raw" / filename
    recorded = json.loads(recorded_path.read_text(encoding="utf-8"))
    regenerated = run()
    assert regenerated == recorded, f"Regenerated result differs from {filename}."
    return {"record": filename, "result": "exact match"}


def main() -> None:
    verification = [
        verify("e01-reference-pipeline-2026-09-08.json", reference_pipeline_run),
        verify("e02-feature-selection-leakage-2026-09-08.json", feature_selection_leakage_run),
        verify("e03-split-variation-2026-09-08.json", split_variation_run),
    ]
    print(json.dumps({"verification": verification}, indent=2))


if __name__ == "__main__":
    main()
