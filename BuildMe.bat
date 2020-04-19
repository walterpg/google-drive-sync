@echo off
set version=4.0.0-unstable
set sevenzip="C:\Program Files (x86)\7-Zip\7z.exe"
set msbuildcmd="C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\Tools\VsMSBuildCmd.bat"

if not exist %msbuildcmd% goto errmsbuild
if not exist %sevenzip% goto errsevenzip
call %msbuildcmd%

if not exist build mkdir build
del /s /f /q build\*

if not exist build\bin mkdir build\bin
if not exist build\dist mkdir build\dist

nuget.exe restore src\
msbuild -t:clean,build -p:Configuration=Release;Platform="Any CPU" -flp:logfile=build\build.log src\GoogleDriveSync.sln
if %errorlevel% NEQ 0 goto error

xcopy src\bin\Release\*.* build\bin
copy src\bin\Release\GoogleDriveSync.plgx build\dist\GoogleDriveSync-%version%.plgx
copy doc\readme.txt build\dist
%sevenzip% a -tzip build\dist\GoogleDriveSync-%version%.zip .\build\bin\*.dll .\build\bin\*.pdb

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
