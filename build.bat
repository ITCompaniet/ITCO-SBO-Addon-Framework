@echo Off
set config=%1
if "%config%" == "" (
   set config=Release
)

set version=
if not "%PackageVersion%" == "" (
   set version=-Version %PackageVersion%
)

REM Package restore
call %NuGet% restore "src\ITCO.SboAddon.Framework\packages.config" -OutputDirectory %cd%\packages -NonInteractive

REM Build
"%programfiles(x86)%\MSBuild\14.0\Bin\MSBuild.exe" "src\ITCO.SboAddon.Framework.sln" /p:Configuration="%config%" /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false
if not "%errorlevel%"=="0" goto failure

REM Package
mkdir Build
call %nuget% pack "src\ITCO.SboAddon.Framework\ITCO.SboAddon.Framework.csproj" -symbols -o Build -p Configuration=%config% %version%
if not "%errorlevel%"=="0" goto failure

:success
exit 0

:failure
exit -1