@echo off
pushd

:: CD to script's directory
cd /D %~dp0

if '%1'=='Release' ( 
	Set Mode=BuildRelease
) else (
	Set Mode=BuildDebug
)
set doPause=1
if not "%2" == "" set doPause=0
%systemroot%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe build.xml /t:%Mode% /p:Revision=1

@if ERRORLEVEL 1 goto fail

:fail
if "%doPause%" == "1" pause
popd
exit /b 1

:end
popd
if "%doPause%" == "1" pause
exit /b 0