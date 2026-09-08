from __future__ import annotations

import argparse
import json
import sys
from datetime import date
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(ROOT / "src"))

from r03_eval.experiments import feature_selection_leakage_run, reference_pipeline_run, split_variation_run


def main() -> None:
    parser = argparse.ArgumentParser(description="Run an R03 experiment and save its raw result.")
    parser.add_argument("experiment", choices=("e01", "e02", "e03"))
    parser.add_argument("--output", type=Path, help="Path for the JSON result. Defaults to results/raw.")
    arguments = parser.parse_args()

    runners = {
        "e01": ("e01-reference-pipeline", reference_pipeline_run),
        "e02": ("e02-feature-selection-leakage", feature_selection_leakage_run),
        "e03": ("e03-split-variation", split_variation_run),
    }
    default_name, run = runners[arguments.experiment]
    result = run()
    output = arguments.output or ROOT / "results" / "raw" / f"{default_name}-{date.today().isoformat()}.json"
    output.parent.mkdir(parents=True, exist_ok=True)
    output.write_text(json.dumps(result, indent=2) + "\n", encoding="utf-8")
    print(output)
    print(json.dumps(result["summary"], indent=2))


if __name__ == "__main__":
    main()
