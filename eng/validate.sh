#!/usr/bin/env bash
set -euo pipefail

target="${1:-All}"
if [ "$#" -gt 0 ]; then shift; fi
# Keep child command output visible; Terminal Logger hides Exec messages.
exec dotnet msbuild "$(dirname "${BASH_SOURCE[0]}")/Validate.proj" -nologo -tl:off "-t:$target" "$@"
