"""Send one logical workflow request concurrently using a shared idempotency key."""

from __future__ import annotations

import argparse
import json
import threading
import uuid
from concurrent.futures import ThreadPoolExecutor, as_completed
from datetime import datetime, timezone
from pathlib import Path
from time import perf_counter
from urllib.error import HTTPError, URLError
from urllib.request import Request, urlopen


def post_workflow(url: str, name: str, key: str, barrier: threading.Barrier, attempt: int) -> dict[str, object]:
    barrier.wait()
    started = perf_counter()
    request = Request(url, data=json.dumps({"name": name}).encode("utf-8"), headers={"Content-Type": "application/json", "Idempotency-Key": key}, method="POST")
    try:
        with urlopen(request, timeout=20) as response:
            return {"attempt": attempt, "status": response.status, "elapsed_ms": round((perf_counter() - started) * 1000, 2), "body": json.loads(response.read().decode("utf-8"))}
    except HTTPError as error:
        return {"attempt": attempt, "status": error.code, "elapsed_ms": round((perf_counter() - started) * 1000, 2), "body": error.read().decode("utf-8")}
    except URLError as error:
        return {"attempt": attempt, "error": str(error.reason), "elapsed_ms": round((perf_counter() - started) * 1000, 2)}


def main() -> None:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--url", default="http://localhost:8080/workflows")
    parser.add_argument("--name", default="concurrent-settlement")
    parser.add_argument("--requests", type=int, default=20)
    parser.add_argument("--key", default=None)
    args = parser.parse_args()
    if args.requests < 2:
        parser.error("--requests must be at least 2")

    key = args.key or f"e04-concurrent-{uuid.uuid4().hex}"
    barrier = threading.Barrier(args.requests)
    started_at = datetime.now(timezone.utc).isoformat()
    with ThreadPoolExecutor(max_workers=args.requests) as executor:
        futures = [executor.submit(post_workflow, args.url, args.name, key, barrier, attempt) for attempt in range(1, args.requests + 1)]
        observations = [future.result() for future in as_completed(futures)]

    observations.sort(key=lambda observation: int(observation["attempt"]))
    workflow_ids = sorted({str(observation["body"]["id"]) for observation in observations if isinstance(observation.get("body"), dict) and "id" in observation["body"]})
    statuses = {str(status): sum(1 for observation in observations if observation.get("status") == status) for status in sorted({observation.get("status") for observation in observations if observation.get("status") is not None})}
    output = {"experiment": "E04-concurrent-idempotency", "started_at_utc": started_at, "request_name": args.name, "idempotency_key": key, "concurrent_requests": args.requests, "status_counts": statuses, "distinct_workflow_ids": workflow_ids, "observations": observations}
    output_path = Path(__file__).resolve().parents[1] / "results" / "raw" / "e04-concurrent-idempotency.json"
    output_path.write_text(json.dumps(output, indent=2) + "\n", encoding="utf-8")
    print(f"Wrote {output_path}")
    print(f"Statuses: {statuses}; distinct workflow IDs: {len(workflow_ids)}")


if __name__ == "__main__":
    main()
