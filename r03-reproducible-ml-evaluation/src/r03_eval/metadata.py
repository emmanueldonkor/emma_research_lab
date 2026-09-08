from __future__ import annotations

import hashlib
import platform
from importlib.metadata import version

import numpy as np


def fingerprint(features: np.ndarray, target: np.ndarray) -> str:
    digest = hashlib.sha256()
    digest.update(np.ascontiguousarray(features).tobytes())
    digest.update(np.ascontiguousarray(target).tobytes())
    return digest.hexdigest()


def environment() -> dict[str, str]:
    return {
        "python": platform.python_version(),
        "numpy": version("numpy"),
        "scikit-learn": version("scikit-learn"),
    }
