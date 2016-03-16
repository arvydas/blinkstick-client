#!/bin/sh
ps x -o pmem | tail -n+2| awk '{s+=$1} END {print s}'
