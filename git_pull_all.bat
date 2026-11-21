@echo off
chcp 65001 >nul
setlocal enabledelayedexpansion

:: Set project root directory based on batch file location
set "ROOT_DIR=%~dp0"
cd /d "%ROOT_DIR%"

echo ========================================================
echo       [Git Pull All] Updating All Repositories
echo ========================================================
echo.

:: 1. Root Repository Pull
call :PROCESS_PULL "%ROOT_DIR%" "Root Repository"

:: 2. Assets Sub-repositories Pull
echo.
echo [2/2] Scanning sub-repositories in Assets...
if not exist "%ROOT_DIR%Assets" (
    echo [WARNING] Assets folder not found: "%ROOT_DIR%Assets"
    goto :END
)

for /d %%D in ("%ROOT_DIR%Assets\*") do (
    if exist "%%D\.git" (
        echo ----------------------------------------------------
        call :PROCESS_PULL "%%D" "Assets\%%~nxD"
    )
)

:END
echo.
echo ========================================================
echo       All repositories have been processed.
echo ========================================================
echo.
pause
exit /b 0

:: Function to process pull with conflict/dirty options
:PROCESS_PULL
set "TARGET_DIR=%~1"
set "TARGET_NAME=%~2"
pushd "%TARGET_DIR%"

set "HAS_DIRTY="
git -c safe.directory="*" diff-index --quiet HEAD -- 2>nul
if errorlevel 1 set "HAS_DIRTY=1"
if not defined HAS_DIRTY (
    for /f "tokens=*" %%U in ('git -c safe.directory="*" status --porcelain 2^>nul') do set "HAS_DIRTY=1"
)

if defined HAS_DIRTY (
    call :DIRTY_MENU "%TARGET_NAME%"
    popd
    exit /b 0
)

:: Attempt normal pull when clean
echo [*] %TARGET_NAME%: Clean state. Pulling...
git -c safe.directory="*" fetch --all --prune 2>nul
git -c safe.directory="*" pull
if errorlevel 1 (
    echo.
    echo [*] [%TARGET_NAME%] Pull failed or conflict detected.
    call :DIRTY_MENU "%TARGET_NAME%"
)

popd
exit /b 0

:DIRTY_MENU
set "MENU_LABEL=%~1"
echo.
echo ========================================================
echo [NOTICE] Local Changes or Conflict Detected: %MENU_LABEL%
echo --------------------------------------------------------
git -c safe.directory="*" status -s
echo --------------------------------------------------------
echo  [1] Stash   : Stash changes, Pull, then Stash pop [Recommended]
echo  [2] Discard : Discard local changes and overwrite [reset --hard]
echo  [3] Commit  : Commit local changes first, then Pull
echo  [4] Skip    : Skip this repository without pulling
echo ========================================================
set "CHOICE="
set /p "CHOICE=Select an option [1-4] (Default: 1): "
if "!CHOICE!"=="" set "CHOICE=1"

if "!CHOICE!"=="1" goto :OPT_STASH
if "!CHOICE!"=="2" goto :OPT_DISCARD
if "!CHOICE!"=="3" goto :OPT_COMMIT
goto :OPT_SKIP

:OPT_STASH
echo [*] Stashing local changes...
git -c safe.directory="*" stash
git -c safe.directory="*" fetch --all --prune 2>nul
git -c safe.directory="*" pull
echo [*] Restoring stashed changes...
git -c safe.directory="*" stash pop
goto :MENU_DONE

:OPT_DISCARD
echo [*] Discarding local changes...
git -c safe.directory="*" reset --hard
git -c safe.directory="*" clean -fd
git -c safe.directory="*" fetch --all --prune 2>nul
git -c safe.directory="*" pull
goto :MENU_DONE

:OPT_COMMIT
set "AUTO_MSG="
set /p "AUTO_MSG=Enter commit message (Default: WIP): "
if "!AUTO_MSG!"=="" set "AUTO_MSG=WIP: work in progress before pull"
git -c safe.directory="*" add -A
git -c safe.directory="*" commit -m "!AUTO_MSG!"
git -c safe.directory="*" fetch --all --prune 2>nul
git -c safe.directory="*" pull
goto :MENU_DONE

:OPT_SKIP
echo [*] Skipping %MENU_LABEL%
goto :MENU_DONE

:MENU_DONE
exit /b 0