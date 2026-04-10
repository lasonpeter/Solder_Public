#!/usr/bin/env bash
set -euo pipefail

TAG="${1:-solder-sim:java8-aot}"

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"

echo "Building image: $TAG"
docker build \
  -f "$SCRIPT_DIR/Dockerfile" \
  -t "$TAG" \
  "$REPO_ROOT"

echo "Done: $TAG"
