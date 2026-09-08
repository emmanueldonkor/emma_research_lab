# Emma Research Lab

Independent software-systems and evaluation studies by Emmanuel Donkor. Each
top-level folder is self-contained: its own code, experiment notes, raw
records, diagrams, and reproducibility instructions.

## Projects

| Project | Question | Evidence to start with |
| --- | --- | --- |
| [R01 - Duplicate-safe workflow service](r01-duplicate-safe-workflow/) | What changes when a small workflow service adds idempotency keys and a transactional outbox? | [Experiment comparison](r01-duplicate-safe-workflow/results/tables/experiment-comparison.md), [clean-database run](r01-duplicate-safe-workflow/results/raw/clean-database-verification.json) |
| [R02 - API resilience under dependency failure](r02-api-resilience/) | What do retry, timeout, circuit-breaker, and concurrency-limit policies change when a dependency is slow or unavailable? | [Findings](r02-api-resilience/research/findings.md), [behaviour check](r02-api-resilience/results/raw/gateway-behaviour-verification-2026-09-07.json) |
| [R03 - Reproducible evaluation for tabular classification](r03-reproducible-ml-evaluation/) | How can feature-selection leakage inflate a result, and what is needed to reproduce it? | [Findings](r03-reproducible-ml-evaluation/research/findings.md), [leakage run](r03-reproducible-ml-evaluation/results/raw/e02-feature-selection-leakage-2026-09-08.json) |

## How to read a project

Start with its README, then open `research/findings.md` or the experiment
notes. Source code, raw run output, diagrams, and verification commands live
in the same project folder. The studies do not share a runtime or a common
library; keeping them separate makes their assumptions and dependencies easier
to inspect.

## Verification entry points

| Project | Check |
| --- | --- |
| R01 | `dotnet test tests/Workflow.Api.Tests/Workflow.Api.Tests.csproj` |
| R02 | Start both local APIs, then run `python scripts/verify_gateway_behaviour.py` |
| R03 | Create the pinned virtual environment, then run `.\.venv\Scripts\python scripts\verify_recorded_runs.py` |

The companion research-site source is at [emma-research-lab-site](https://github.com/emmanueldonkor/emma-research-lab-site).
