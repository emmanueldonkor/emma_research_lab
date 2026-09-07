"""Run six concurrent gateway requests and print the observed responses as JSON."""

from __future__ import annotations

import argparse
import json
import threading
import time
import urllib.error
import urllib.request
from concurrent.futures import ThreadPoolExecutor


def request(url: str, start_barrier: threading.Barrier) -> dict[str, object]:
    start_barrier.wait()
    started = time.perf_counter()
    try:
        with urllib.request.urlopen(url) as response:
            body = response.read().decode("utf-8")
            status = response.status
    except urllib.error.HTTPError as error:
        body = error.read().decode("utf-8")
        status = error.code

    return {
        "status": status,
        "elapsed_milliseconds": round((time.perf_counter() - started) * 1000),
        "body": json.loads(body),
    }


def main() -> None:
    parser = argparse.ArgumentParser()
    parser.add_argument("endpoint", choices=("baseline", "limited"))
    parser.add_argument("--callers", type=int, default=6)
    parser.add_argument("--gateway-url", default="http://localhost:5090")
    arguments = parser.parse_args()

    url = f"{arguments.gateway_url}/proxy/{arguments.endpoint}"
    start_barrier = threading.Barrier(arguments.callers)
    with ThreadPoolExecutor(max_workers=arguments.callers) as executor:
        futures = [executor.submit(request, url, start_barrier) for _ in range(arguments.callers)]
        results = [future.result() for future in futures]

    print(json.dumps({"endpoint": arguments.endpoint, "callers": arguments.callers, "results": results}, indent=2))


if __name__ == "__main__":
    main()
