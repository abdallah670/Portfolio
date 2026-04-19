@echo off
REM Portfolio API - Environment Variables Setup Script (CMD version)
REM Run this script to set all required environment variables for local development

echo Setting Portfolio API Environment Variables...
echo WARNING: These are development values. Do NOT use in production!
echo.

REM JWT Configuration
setx Jwt__Secret "menomo-portfolio-api-strong-secret-key-32"
echo [OK] Jwt__Secret set

REM Admin User Configuration
setx Admin__Username "Menomo"
echo [OK] Admin__Username set

setx Admin__Password "Menomo@1234"
echo [OK] Admin__Password set

REM Email Settings
setx EmailSettings__SmtpUser "hnbg14006@gmail.com"
echo [OK] EmailSettings__SmtpUser set

setx EmailSettings__SmtpPass "vlkk jmat ouli zbgs"
echo [OK] EmailSettings__SmtpPass set

echo.
echo All environment variables have been set for the CURRENT USER!
echo You may need to restart your terminal/IDE for changes to take effect.
echo.
echo To verify, run: echo %Jwt__Secret%
pause
