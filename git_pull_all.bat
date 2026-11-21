@echo off
setlocal enabledelayedexpansion

:: Set project root directory based on batch file location
set "ROOT_DIR=%~dp0"
cd /d "%ROOT_DIR%"

echo ========================================================
echo       [Git Pull All] Updating All Repositories
echo ========================================================
echo.

:: 1. Root Repository Pull
echo [1/2] Root Repository: %ROOT_DIR%
git -c safe.directory="*" fetch --all --prune 2>nul
git -c safe.directory="*" pull
if errorlevel 1 (
    echo [WARNING] Failed to pull root repository.
)
echo.

:: 2. Assets Sub-repositories Pull
echo [2/2] Scanning sub-repositories in Assets...
if not exist "%ROOT_DIR%Assets" (
    echo [ERROR] Assets directory not found at: "%ROOT_DIR%Assets"
    goto :END
)

for /d %%D in ("%ROOT_DIR%Assets\*") do (
    if exist "%%D\.git" (
        echo ----------------------------------------------------
        echo [*] Assets\%%~nxD
        pushd "%%D"
        git -c safe.directory="*" fetch --all --prune 2>nul
        git -c safe.directory="*" pull
        if errorlevel 1 (
            echo [WARNING] Failed to pull Assets\%%~nxD
        )
        popd
    )
)

:END
echo.
echo ========================================================
echo       All repositories have been pulled!
echo ========================================================
echo.
pause