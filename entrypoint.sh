#!/bin/bash

set -e
run_cmd="dotnet run --no-build --urls http://0.0.0.0:5000 -v d"

export PATH="$PATH:/root/.dotnet/tools"

exec $run_cmd