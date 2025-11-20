#!/bin/bash
# SDK AAR Build Tool - Launcher Script
# This script ensures proper setup before running the tool

echo "================================"
echo "SDK AAR Build Tool - TamronTools"
echo "================================"
echo ""

# Check Python3
if ! command -v python3 &> /dev/null; then
    echo "[ERROR] Python 3 is not installed!"
    echo "Please install Python 3.7+ first."
    exit 1
fi

echo "[OK] Python 3 found: $(python3 --version)"

# Check tkinter
echo "[INFO] Checking tkinter..."
if ! python3 -c "import tkinter" 2>/dev/null; then
    echo "[ERROR] tkinter is not installed!"
    echo ""
    echo "Please install tkinter:"
    echo "  Ubuntu/Debian: sudo apt-get install python3-tk"
    echo "  Fedora/RHEL:   sudo dnf install python3-tkinter"
    echo "  macOS:         brew install python-tk"
    exit 1
fi

echo "[OK] tkinter is available"

# Check Java
if ! command -v java &> /dev/null; then
    echo "[WARNING] Java is not found in PATH!"
    echo "You need Java 8+ to build AAR."
    echo "Please install Java JDK and add to PATH."
else
    echo "[OK] Java found: $(java -version 2>&1 | head -n 1)"
fi

echo ""
echo "[INFO] Starting SDK AAR Build Tool..."
echo ""

# Run the tool
cd "$(dirname "$0")"
python3 builder.py

echo ""
echo "[INFO] Tool closed."
