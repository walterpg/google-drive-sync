@echo off
set version=4.0.0-alpha
set sevenzip="%ProgramFiles(x86)%\7-Zip\7z.exe"
set msbuildcmd="%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Community\Common7\Tools\VsMSBuildCmd.bat"
set pandoc="%LOCALAPPDATA%\Pandoc\pandoc.exe"

if not exist %msbuildcmd% goto errmsbuild
if not exist %sevenzip% goto errsevenzip
call %msbuildcmd%

if not exist build mkdir build
del /s /f /q build\*

if not exist build\bin mkdir build\bin
if not exist build\dist mkdir build\dist
if not exist build\log mkdir build\log
del /s /f /q build\bin\*
del /s /f /q build\dist\*
del /s /f /q build\log\*

nuget.exe restore
msbuild -t:clean,build -p:Configuration=Release;Platform="Any CPU" -flp:logfile=build\log\build.log GoogleDriveSync.sln
if %errorlevel% NEQ 0 goto error

xcopy src\bin\Release\*.* build\bin
copy src\bin\Release\GoogleDriveSync.plgx build\dist\GoogleDriveSync-%version%.plgx
%sevenzip% a -tzip build\dist\GoogleDriveSync-%version%.zip .\build\bin\*.dll .\build\bin\*.pdb
.\lib\src\GenVerInfo\bin\Release\GenVerInfo.exe .\build\bin\GoogleDriveSync.dll .\current_version_manifest.txt

if not exist %pandoc% goto end
%pandoc% -f gfm -t plain --wrap=auto doc\README.md -o build\dist\readme.txt

goto end

:errmsbuild
echo MsBuild command-line setup not found!
goto error

:errsevenzip
echo Zip command-line tool not found!
goto error

:error
echo *************
echo Build Failed.
echo *************
pause

:end
