@echo off

:: Initialize Visual Studio 2022 Community environment
set "VS_DEV_CMD=C:\Program Files\Microsoft Visual Studio\2022\Community\Common7\Tools\VsDevCmd.bat"

if exist "%VS_DEV_CMD%" (
    echo Initializing Visual Studio 2022 Community environment...
    call "%VS_DEV_CMD%"
) else (
    echo Warning: VsDevCmd.bat not found at "%VS_DEV_CMD%".
    echo Ensure Visual Studio 2022 Community is installed.
)

dotnet build PLog.Win/PLog.Win.fsproj -c Release
pause
