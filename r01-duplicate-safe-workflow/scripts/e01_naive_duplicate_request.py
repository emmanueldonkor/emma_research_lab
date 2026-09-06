"""Send the same logical request repeatedly against the E01 baseline."""

from __future__ import annotations

import argparse
import json
from datetime import datetime, timezone
from pathlib import Path
from urllib.error import HTTPError, URLError
from urllib.request import Request, urlopen


def post_workflow(url: str, name: str) -> dict[str, object]:
    request = Request(
        url,
        data=json.dumps({"name": name}).encode("utf-8"),
        headers={"Content-Type": "application/json"},
        method="POST",
    )
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
    parser.add_argument("--name", default="weekly-report")
    parser.add_argument("--requests", type=int, default=5)
    args = parser.parse_args()
    if args.requests < 1:
        parser.error("--requests must be at least 1")

    observations = []
    for attempt in range(1, args.requests + 1):
        observations.append({"attempt": attempt, "sent_at_utc": datetime.now(timezone.utc).isoformat(), "response": post_workflow(args.url, args.name)})

    output = {"experiment": "E01-naive-handler", "request_name": args.name, "requested_attempts": args.requests, "observations": observations}
    output_path = Path(__file__).resolve().parents[1] / "results" / "raw" / "e01-naive-duplicate-request.json"
    output_path.write_text(json.dumps(output, indent=2) + "\n", encoding="utf-8")
    print(f"Wrote {output_path}")


if __name__ == "__main__":
    main()
