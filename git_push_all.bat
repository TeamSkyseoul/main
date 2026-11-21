@echo off
chcp 65001 >nul
setlocal enabledelayedexpansion

:: Set project root directory based on batch file location
set "ROOT_DIR=%~dp0"
cd /d "%ROOT_DIR%"

echo ========================================================
echo       [Git Push All] Commit and Push All Repositories
echo ========================================================
echo.

:: Get Commit Message from argument or user input
set "COMMIT_MSG=%~1"
if "%COMMIT_MSG%"=="" (
    set /p "COMMIT_MSG=Enter commit message (Press Enter for default): "
)
for /f "tokens=* delims= " %%A in ("!COMMIT_MSG!") do set "COMMIT_MSG=%%A"
if "!COMMIT_MSG!"=="" (
    set "COMMIT_MSG=chore: update project and assets"
)

echo.
echo --------------------------------------------------------
echo Commit message: "!COMMIT_MSG!"
echo --------------------------------------------------------
echo.

:: 1. Assets Sub-repositories Push
echo [1/2] Checking sub-repositories in Assets...
if not exist "%ROOT_DIR%Assets" (
    echo [ERROR] Assets directory not found at: "%ROOT_DIR%Assets"
    goto :ROOT_PUSH
)

for /d %%D in ("%ROOT_DIR%Assets\*") do (
    if exist "%%D\.git" (
        pushd "%%D"
        set "HAS_CHANGES="
        set "IS_AHEAD="

        for /f "tokens=*" %%S in ('git -c safe.directory="*" status -s 2^>nul') do set "HAS_CHANGES=1"
        for /f "tokens=*" %%A in ('git -c safe.directory="*" status -b --porcelain 2^>nul ^| findstr "ahead"') do set "IS_AHEAD=1"

        if defined HAS_CHANGES (
            echo.
            echo [*] [Changes detected] Committing and pushing Assets\%%~nxD...
            git -c safe.directory="*" add -A
            git -c safe.directory="*" commit -m "!COMMIT_MSG!"
            call :SMART_PUSH "Assets\%%~nxD"
        ) else if defined IS_AHEAD (
            echo.
            echo [*] [Unpushed commits detected] Pushing Assets\%%~nxD...
            call :SMART_PUSH "Assets\%%~nxD"
        )
        popd
    )
)

:ROOT_PUSH
echo.
echo [2/2] Checking root repository...
set "ROOT_CHANGES="
set "ROOT_AHEAD="

for /f "tokens=*" %%S in ('git -c safe.directory="*" status -s 2^>nul') do set "ROOT_CHANGES=1"
for /f "tokens=*" %%A in ('git -c safe.directory="*" status -b --porcelain 2^>nul ^| findstr "ahead"') do set "ROOT_AHEAD=1"

if defined ROOT_CHANGES (
    echo.
    echo [*] [Changes detected] Committing and pushing root repository...
    git -c safe.directory="*" add -A
    git -c safe.directory="*" commit -m "!COMMIT_MSG!"
    call :SMART_PUSH "Root"
) else if defined ROOT_AHEAD (
    echo.
    echo [*] [Unpushed commits detected] Pushing root repository...
    call :SMART_PUSH "Root"
) else (
    echo.
    echo [*] Root repository is clean and up to date.
)

echo.
echo ========================================================
echo       All repositories push completed!
echo ========================================================
echo.
pause
exit /b 0

:: Smart Push Function (Handles regular push, upstream for PR branches, and pull on reject)
:SMART_PUSH
set "REPO_LABEL=%~1"
git -c safe.directory="*" push 2>nul
if errorlevel 1 (
    set "HAS_UPSTREAM="
    for /f "tokens=*" %%U in ('git -c safe.directory="*" rev-parse --abbrev-ref @{u} 2^>nul') do set "HAS_UPSTREAM=1"

    if not defined HAS_UPSTREAM (
        set "CUR_BRANCH="
        set "TARGET_REMOTE="
        for /f "tokens=*" %%B in ('git -c safe.directory="*" branch --show-current 2^>nul') do set "CUR_BRANCH=%%B"
        for /f "tokens=*" %%R in ('git -c safe.directory="*" remote 2^>nul') do (
            if not defined TARGET_REMOTE set "TARGET_REMOTE=%%R"
        )
        if defined CUR_BRANCH (
            echo [*] Setting upstream and pushing branch [!CUR_BRANCH!] to [!TARGET_REMOTE!]...
            git -c safe.directory="*" push -u !TARGET_REMOTE! !CUR_BRANCH!
        )
        exit /b 0
    )

    echo.
    echo ----------------------------------------------------
    echo [NOTICE] Push Rejected: !REPO_LABEL! - Remote has newer commits.
    echo  [1] Pull and Push : Merge remote changes then retry push [Recommended]
    echo  [2] Skip          : Skip pushing this repository
    echo ----------------------------------------------------
    set "PUSH_CHOICE="
    set /p "PUSH_CHOICE=Select an option [1-2] (Default: 1): "
    if "!PUSH_CHOICE!"=="" set "PUSH_CHOICE=1"

    if "!PUSH_CHOICE!"=="1" (
        echo [*] Pulling remote changes...
        git -c safe.directory="*" pull --no-edit
        echo [*] Retrying push...
        git -c safe.directory="*" push
    ) else (
        echo [*] Skipping push for !REPO_LABEL!
    )
)
exit /b 0