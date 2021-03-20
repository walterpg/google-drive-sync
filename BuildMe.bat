:: 
:: Google Sync Plugin for KeePass Password Safe
:: Copyright © 2012-2016  DesignsInnovate
:: Copyright © 2014-2016  Paul Voegler
::  
:: KPSync for Google Drive
:: Copyright © 2020-2021 Walter Goodwin
:: 
:: This program is free software: you can redistribute it and/or modify
:: it under the terms of the GNU General Public License as published by
:: the Free Software Foundation, either version 3 of the License, or
:: (at your option) any later version.
::
:: This program is distributed in the hope that it will be useful,
:: but WITHOUT ANY WARRANTY; without even the implied warranty of
:: MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
:: GNU General Public License for more details.
:: 
:: You should have received a copy of the GNU General Public License
:: along with this program.  If not, see <http://www.gnu.org/licenses/>.
:: 

@echo off

set archname=KPSyncForDrive
set kp_version_manifest_name=kpsync_final
set versionPrefix=4.0.6
set versionSuffix=beta
set netsdkver=net45

set sevenzip="%ProgramFiles%\7-Zip\7z.exe"
:: set dotnetPgm="%ProgramFiles%\dotnet\dotnet.exe"
set msbuildcmd="%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Community\Common7\Tools\VsMSBuildCmd.bat"
set pandoc="%LOCALAPPDATA%\Pandoc\pandoc.exe"

if not exist %msbuildcmd% goto errmsbuild
if not exist %sevenzip% goto errsevenzip
call %msbuildcmd%

if not exist build mkdir build
del /s /f /q build\*
for /d %%o in (build\*) do rmdir "%%o" /s /q

mkdir build\bin
mkdir build\dist
mkdir build\log

:: TODO modify project(s) to use `dotnet msbuild`.  May require net461
:: runtime support. See https://github.com/dotnet/msbuild/issues/4704
msbuild -t:clean,restore,build -m -p:TargetFramework=%netsdkver% -p:Configuration=Release;Platform="Any CPU" -flp:logfile=build\log\build.log GoogleDriveSync.sln
if %errorlevel% NEQ 0 goto error

xcopy src\bin\Release\%netsdkver%\*.* build\bin /e /h
copy build\bin\*.plgx build\dist\
%sevenzip% a -tzip -r build\dist\%archname%-%versionPrefix%-%versionSuffix%.zip .\build\bin\*.dll .\build\bin\*.pdb .\build\bin\es\*
.\lib\src\GenVerInfo\bin\Release\%netsdkver%\GenVerInfo.exe .\build\bin\%archname%.dll .\%kp_version_manifest_name%.txt

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
