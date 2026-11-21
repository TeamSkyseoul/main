@echo off
chcp 65001 >nul
setlocal enabledelayedexpansion
set "ROOT_DIR=%~dp0"
cd /d "%ROOT_DIR%"

:MENU
cls
echo ========================================================
echo         SkySeoul Git Multi-Repository Manager
echo ========================================================
echo.
echo  [1] Pull All   : Update all repositories (최신화)
echo  [2] Push All   : Commit and push all repositories (일괄 푸시)
echo  [3] Status All : Check status of all repositories (상태 점검)
echo  [0] Exit       : 종료
echo.
echo ========================================================
set "CHOICE="
set /p "CHOICE=Select menu [0-3]: "

if "%CHOICE%"=="1" (
    call "%ROOT_DIR%git_pull_all.bat"
    goto :MENU
)
if "%CHOICE%"=="2" (
    call "%ROOT_DIR%git_push_all.bat"
    goto :MENU
)
if "%CHOICE%"=="3" (
    cls
    echo ========================================================
    echo         Checking Status of All Repositories
    echo ========================================================
    echo.
    echo [*] Root Repository:
    git -c safe.directory="*" status -s -b
    echo.
    echo [*] Assets Sub-repositories:
    for /d %%D in ("%ROOT_DIR%Assets\*") do (
        if exist "%%D\.git" (
            echo ----------------------------------------------------
            echo Assets\%%~nxD
            pushd "%%D"
            git -c safe.directory="*" status -s -b
            popd
        )
    )
    echo.
    echo ========================================================
    pause
    goto :MENU
)
if "%CHOICE%"=="0" exit /b 0

goto :MENU