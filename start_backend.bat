@echo off
setlocal
cd /d "%~dp0GrowsmartAPI"

REM Check if GrowsmartAPI is already running and kill it to avoid "file in use" errors
taskkill /IM GrowsmartAPI.exe /F 2>nul

echo Starting GrowsmartAPI with watch mode...
echo (It will automatically restart when you change code!)
start "GrowsmartAPI" dotnet watch run --project GrowsmartAPI.csproj --launch-profile http

REM Check if ngrok is already running, if not, start it
tasklist /FI "IMAGENAME eq ngrok.exe" | find /I "ngrok.exe" >nul
if %ERRORLEVEL% NEQ 0 (
    echo Starting ngrok tunnel on port 5050...
    start "ngrok" ngrok http 5050
) else (
    echo ngrok is already running.
)

echo.
echo Backend is starting up. 
echo You don't need to run this again unless you close the terminal windows!
pause
