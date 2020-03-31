@echo off

set keepassexe=..\..\KeePassPortable-2.35\KeePass.exe
set zipexe="C:\Program Files\7-Zip\7z.exe"
set zipcommand=a

rem END CONFIGURATION

if not exist %keepassexe% (
  echo keepass.exe not found: %keepassexe%
  echo make sure the solution is built first using Visual Studio
  exit /b 1
)

if not exist %zipexe% (
  echo zip program not found: %zipexe%
  exit /b 1
)


rd /s /q .\build

md .\build\GoogleSyncPlugin

copy %keepassexe% .\build\

xcopy /s src\*.* .\build\GoogleSyncPlugin\

md .\build\GoogleSyncPlugin\Libraries
xcopy .\build\GoogleSyncPlugin\bin\Release\*.dll .\build\GoogleSyncPlugin\Libraries\
del /q .\build\GoogleSyncPlugin\Libraries\GoogleSyncPlugin.dll

ren .\build\GoogleSyncPlugin\GoogleSyncPlugin.csproj GoogleSyncPlugin.csproj.old
Powershell -Command "(Get-Content .\build\GoogleSyncPlugin\GoogleSyncPlugin.csproj.old) | ForEach-Object { $_ -replace '<HintPath>packages\\.*?\\([^\\]*?)</HintPath>', '<HintPath>Libraries\$1</HintPath>' } | Set-Content .\build\GoogleSyncPlugin\GoogleSyncPlugin.csproj"
del /q .\build\GoogleSyncPlugin\GoogleSyncPlugin.csproj.old

rd /s /q .\build\GoogleSyncPlugin\.vs
rd /s /q .\build\GoogleSyncPlugin\bin
rd /s /q .\build\GoogleSyncPlugin\obj
rd /s /q .\build\GoogleSyncPlugin\packages
del /q .\build\GoogleSyncPlugin\*.user

.\build\KeePass.exe --plgx-create %~dp0build\GoogleSyncPlugin --plgx-prereq-kp:2.35 --plgx-prereq-net:4.5

del /q .\build\GoogleSyncPlugin.zip
%zipexe% %zipcommand% .\build\GoogleSyncPlugin.zip .\build\GoogleSyncPlugin.plgx doc\*.*