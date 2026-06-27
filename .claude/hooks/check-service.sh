#!/usr/bin/env bash
# PostToolUse hook: checks service registration hygiene after Write/Edit to Scripts/Services/

INPUT=$(cat)

# Extract file_path from JSON input
FILE=$(echo "$INPUT" | grep -o '"file_path":"[^"]*"' | head -1 | sed 's/"file_path":"//;s/"//')

# Normalize backslashes to forward slashes
FILE=$(echo "$FILE" | tr '\\' '/')

# Only act on .cs files inside Scripts/Services/
if [[ "$FILE" != *"Scripts/Services/"* ]] || [[ "$FILE" != *.cs ]]; then
    exit 0
fi

BASENAME=$(basename "$FILE" .cs)

# Skip interface files and non-service files
if [[ "$BASENAME" == I* ]]; then
    exit 0
fi

WARNINGS=0

# Check for sibling interface file
DIR=$(dirname "$FILE")
INTERFACE="$DIR/I${BASENAME}.cs"
if [ ! -f "$INTERFACE" ]; then
    echo "⚠  WARNING: Interface not found — expected I${BASENAME}.cs alongside ${BASENAME}.cs"
    WARNINGS=$((WARNINGS + 1))
fi

# Check registration in GameBootstrap
BOOTSTRAP="Assets/_Project/Scripts/Bootstrap/GameBootstrap.cs"
if ! grep -q "new ${BASENAME}()" "$BOOTSTRAP" 2>/dev/null; then
    echo "⚠  WARNING: ${BASENAME} is not registered in GameBootstrap.RegisterServices()"
    WARNINGS=$((WARNINGS + 1))
fi

if [ $WARNINGS -gt 0 ]; then
    echo "   Run /new-service skill for the full checklist."
fi

exit 0
