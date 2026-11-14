@echo off
cd /d "%~dp0"
echo Checking the StudyFlow automated tests...
dotnet run --project StudyFlow.Tests
pause
