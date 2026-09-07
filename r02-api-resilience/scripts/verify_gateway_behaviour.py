"""Check the local R02 gateway contracts against a running dependency and gateway."""

from __future__ import annotations

import concurrent.futures
import json
import threading
import time
import urllib.error
import urllib.request

DEPENDENCY_URL = "http://localhost:5091"
GATEWAY_URL = "http://localhost:5090"


def call(url: str, method: str = "GET", payload: dict[str, object] | None = None) -> tuple[int, dict[str, object]]:
    data = None if payload is None else json.dumps(payload).encode("utf-8")
    request = urllib.request.Request(url, data=data, method=method)
    request.add_header("Content-Type", "application/json")

    try:
        with urllib.request.urlopen(request) as response:
            return response.status, json.loads(response.read().decode("utf-8"))
    except urllib.error.HTTPError as error:
        return error.code, json.loads(error.read().decode("utf-8"))


def configure(mode: str, delay_milliseconds: int = 0) -> dict[str, object]:
    status, body = call(
        f"{DEPENDENCY_URL}/control",
        method="POST",
        payload={"mode": mode, "delayMilliseconds": delay_milliseconds},
    )
    assert status == 200, body
    return body


def expect(status: int, body: dict[str, object], expected_status: int, message: str) -> None:
    assert status == expected_status, f"{message}: expected {expected_status}, got {status}; body={body}"


def main() -> None:
    results: dict[str, object] = {}

    configure("Healthy")
    status, body = call(f"{GATEWAY_URL}/proxy/baseline")
    expect(status, body, 200, "healthy baseline")
    results["baseline"] = status

    configure("Unavailable")
    status, body = call(f"{GATEWAY_URL}/proxy/retry")
    expect(status, body, 503, "retry during sustained outage")
    assert body["attemptCount"] == 3, body
    results["retry_attempts"] = body["attemptCount"]

    configure("Slow", 750)
    status, body = call(f"{GATEWAY_URL}/proxy/timeout")
    expect(status, body, 504, "timeout against slow dependency")
    assert body["attemptCount"] == 1, body
    results["timeout"] = status

    configure("Healthy")
    status, body = call(f"{GATEWAY_URL}/proxy/circuit")
    expect(status, body, 200, "circuit reset call")

    configure("Unavailable")
    for failure_number in range(3):
        status, body = call(f"{GATEWAY_URL}/proxy/circuit")
        expect(status, body, 503, f"circuit failure {failure_number + 1}")
    status, body = call(f"{GATEWAY_URL}/proxy/circuit")
    expect(status, body, 503, "open circuit")
    assert body["shortCircuited"] is True and body["dependencyAttemptCount"] == 0, body
    results["circuit_short_circuited"] = body["shortCircuited"]

    configure("Healthy")
    time.sleep(1.1)
    status, body = call(f"{GATEWAY_URL}/proxy/circuit")
    expect(status, body, 200, "circuit recovery")
    results["circuit_recovery"] = status

    configure("Slow", 500)
    start_barrier = threading.Barrier(6)
    def limited_call() -> int:
        start_barrier.wait()
        return call(f"{GATEWAY_URL}/proxy/limited")[0]

    with concurrent.futures.ThreadPoolExecutor(max_workers=6) as executor:
        futures = [executor.submit(limited_call) for _ in range(6)]
        concurrent_statuses = [future.result() for future in futures]
    assert concurrent_statuses.count(200) == 2, concurrent_statuses
    assert concurrent_statuses.count(429) == 4, concurrent_statuses
    results["limited_statuses"] = sorted(concurrent_statuses)

    print(json.dumps(results, indent=2))


if __name__ == "__main__":
    main()
