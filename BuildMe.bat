@echo off

set archname=KeePassSyncForDrive
set kp_version_manifest_name=kpsync_version_4
set version=4.0.2-unstable

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
copy src\bin\Release\%archname%.plgx build\dist\%archname%-%version%.plgx
%sevenzip% a -tzip build\dist\%archname%-%version%.zip .\build\bin\*.dll .\build\bin\*.pdb
.\lib\src\GenVerInfo\bin\Release\GenVerInfo.exe .\build\bin\%archname%.dll .\%kp_version_manifest_name%.txt

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
