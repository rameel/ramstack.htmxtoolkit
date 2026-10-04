@echo off
rem Keep child command output visible; Terminal Logger hides Exec messages.
if "%~1"=="" (
  dotnet msbuild "%~dp0Validate.proj" -nologo -tl:off -t:All
) else (
  dotnet msbuild "%~dp0Validate.proj" -nologo -tl:off -t:%*
)
exit /b %ERRORLEVEL%
