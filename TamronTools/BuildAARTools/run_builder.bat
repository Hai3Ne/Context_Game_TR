@echo off
REM SDK AAR Build Tool - Windows Launcher Script
REM This script ensures proper setup before running the tool

echo ================================
echo SDK AAR Build Tool - TamronTools
echo ================================
echo.

REM Check Python
where python >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Python is not installed or not in PATH!
    echo Please install Python 3.7+ from python.org
    pause
    exit /b 1
)

echo [OK] Python found
python --version

REM Check Java
where java >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo [WARNING] Java is not found in PATH!
    echo You need Java 8+ to build AAR.
    echo Please install Java JDK and add to PATH.
) else (
    echo [OK] Java found
    java -version
)

echo.
echo [INFO] Starting SDK AAR Build Tool...
echo.

REM Run the tool
cd /d "%~dp0"
python builder.py

echo.
echo [INFO] Tool closed.
pause
