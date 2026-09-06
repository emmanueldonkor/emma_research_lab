"""Replay one logical request with a single Idempotency-Key for E02."""

from __future__ import annotations

import argparse
import json
from datetime import datetime, timezone
from pathlib import Path
from urllib.error import HTTPError, URLError
from urllib.request import Request, urlopen


def post_workflow(url: str, name: str, key: str) -> dict[str, object]:
    request = Request(url, data=json.dumps({"name": name}).encode("utf-8"), headers={"Content-Type": "application/json", "Idempotency-Key": key}, method="POST")
    try:
        with urlopen(request, timeout=10) as response:
            return {"status": response.status, "body": json.loads(response.read().decode("utf-8"))}
    except HTTPError as error:
        return {"status": error.code, "body": error.read().decode("utf-8")}
    except URLError as error:
        return {"error": str(error.reason)}


def main() -> None:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--url", default="http://localhost:8080/workflows")
    parser.add_argument("--name", default="monthly-invoice")
    parser.add_argument("--key", default="e02-monthly-invoice-001")
    parser.add_argument("--requests", type=int, default=5)
    args = parser.parse_args()
    if args.requests < 1:
        parser.error("--requests must be at least 1")

    observations = [{"attempt": attempt, "sent_at_utc": datetime.now(timezone.utc).isoformat(), "response": post_workflow(args.url, args.name, args.key)} for attempt in range(1, args.requests + 1)]
    output = {"experiment": "E02-idempotency-key", "request_name": args.name, "idempotency_key": args.key, "requested_attempts": args.requests, "observations": observations}
    output_path = Path(__file__).resolve().parents[1] / "results" / "raw" / "e02-idempotency-key.json"
    output_path.write_text(json.dumps(output, indent=2) + "\n", encoding="utf-8")
    print(f"Wrote {output_path}")


if __name__ == "__main__":
    main()
