#!/bin/sh
ps x -o pcpu | tail -n+2| awk '{s+=$1} END {print s}'
