@echo off
REM Auto Download Java 17 Installer for Windows

echo ========================================
echo   Java 17 Download Helper for Windows
echo ========================================
echo.

echo [INFO] This script will help you download Java 17
echo.

echo [STEP 1] Opening Java 17 download page...
timeout /t 2 >nul
start https://adoptium.net/temurin/releases/?version=17

echo.
echo ========================================
echo   Manual Installation Steps:
echo ========================================
echo.
echo 1. On the webpage that just opened:
echo    - Version: 17 - LTS
echo    - Operating System: Windows
echo    - Architecture: x64
echo    - Package Type: JDK
echo    - Click: .msi installer
echo.
echo 2. Run the downloaded .msi file
echo    - Click Next
echo    - Click Next
echo    - Click Install
echo    - Wait for installation
echo    - Click Finish
echo.
echo 3. Open Android Studio:
echo    - File -^> Project Structure
echo    - SDK Location
echo    - JDK location -^> Browse
echo    - Select: C:\Program Files\Eclipse Adoptium\jdk-17...
echo    - Click Apply -^> OK
echo.
echo 4. Sync and Build:
echo    - File -^> Sync Project with Gradle Files
echo    - Build -^> Rebuild Project
echo.
echo ========================================
echo   Alternative: Auto-detect in Android Studio
echo ========================================
echo.
echo If Java 17 doesn't appear in Android Studio:
echo.
echo 1. File -^> Project Structure -^> SDK Location
echo 2. JDK location dropdown -^> Download JDK...
echo 3. Version: 17
echo 4. Vendor: Eclipse Temurin
echo 5. Click Download
echo 6. Wait for download
echo 7. Click Apply -^> OK
echo.
echo ========================================

pause
