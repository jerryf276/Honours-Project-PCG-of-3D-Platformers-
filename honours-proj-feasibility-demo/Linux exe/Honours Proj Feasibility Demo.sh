#!/bin/sh
printf '\033c\033]0;%s\a' Honours Proj Feasibility Demo
base_path="$(dirname "$(realpath "$0")")"
"$base_path/Honours Proj Feasibility Demo.x86_64" "$@"
