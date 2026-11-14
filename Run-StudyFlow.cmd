@echo off
cd /d "%~dp0"
echo Starting StudyFlow... The terminal will show the local web address.
dotnet run --project StudyFlow.Web
pause
