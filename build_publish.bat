@echo off
setlocal EnableExtensions
cd /d "%~dp0"

where dotnet >nul 2>nul
if errorlevel 1 (
  echo ERREUR: .NET SDK 8 introuvable.
  echo Installe Visual Studio 2022 avec "Desktop development with .NET" ou le SDK .NET 8.
  pause
  exit /b 1
)

set "TEMP_PUBLISH=%~dp0.__single_exe_publish"
set "DIST=%~dp0dist"
set "EXE_NAME=BO3 Sound Suite.exe"

if exist "%TEMP_PUBLISH%" rmdir /s /q "%TEMP_PUBLISH%"
if exist "%DIST%" rmdir /s /q "%DIST%"
mkdir "%TEMP_PUBLISH%" >nul
mkdir "%DIST%" >nul

echo [1/3] Restauration...
dotnet restore BO3SoundSuite.csproj -r win-x64
if errorlevel 1 goto :fail

echo [2/3] Compilation et publication en UN SEUL EXE...
dotnet publish BO3SoundSuite.csproj ^
  -c Release ^
  -r win-x64 ^
  --self-contained true ^
  --no-restore ^
  -o "%TEMP_PUBLISH%" ^
  -p:PublishSingleFile=true ^
  -p:IncludeNativeLibrariesForSelfExtract=true ^
  -p:EnableCompressionInSingleFile=true ^
  -p:PublishTrimmed=false ^
  -p:PublishReadyToRun=false ^
  -p:DebugType=None ^
  -p:DebugSymbols=false ^
  -p:CopyOutputSymbolsToPublishDirectory=false ^
  -p:SkipSingleExeAfterBuild=true
if errorlevel 1 goto :fail

if not exist "%TEMP_PUBLISH%\%EXE_NAME%" (
  echo ERREUR: l'EXE final n'a pas ete genere.
  goto :fail
)

copy /y "%TEMP_PUBLISH%\%EXE_NAME%" "%DIST%\%EXE_NAME%" >nul
if errorlevel 1 goto :fail
rmdir /s /q "%TEMP_PUBLISH%"

echo [3/3] Verification stricte...
for /f %%A in ('dir /b /a-d "%DIST%" ^| find /c /v ""') do set "COUNT=%%A"
if not "%COUNT%"=="1" goto :bad_dist
if exist "%DIST%\*.dll" goto :bad_dist
if exist "%DIST%\*.json" goto :bad_dist
if exist "%DIST%\*.pdb" goto :bad_dist
if exist "%DIST%\*.deps.json" goto :bad_dist
if exist "%DIST%\*.runtimeconfig.json" goto :bad_dist

for /d %%D in ("%DIST%\*") do goto :bad_dist

echo.
echo OK - DISTRIBUTION SINGLE-FILE TERMINEE.
echo.
echo IMPORTANT:
echo Le dossier bin contient les fichiers techniques de compilation et peut contenir des DLL.
echo NE DISTRIBUE PAS le dossier bin.
echo Le fichier a distribuer est uniquement:
echo "%DIST%\%EXE_NAME%"
echo.
start "" explorer.exe "%DIST%"
pause
exit /b 0

:bad_dist
echo ERREUR: dist ne contient pas uniquement l'EXE.
dir /b "%DIST%"
goto :fail

:fail
if exist "%TEMP_PUBLISH%" rmdir /s /q "%TEMP_PUBLISH%"
echo.
echo ECHEC DE LA COMPILATION SINGLE-FILE.
pause
exit /b 1
