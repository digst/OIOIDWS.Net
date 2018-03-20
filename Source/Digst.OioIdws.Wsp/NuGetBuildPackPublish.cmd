@echo off

set NUGET=..\..\Build\nuget.exe
set BUILD="C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\bin\MSBuild.exe"

set OUTPUT=.\nuget\

set PROJ=Digst.OioIdws.Wsp
set CSPROJ=.\%PROJ%.csproj

:: In order for this script to work, you must first set you API Key from a cmd like this:
:: nuget SetApiKey [KEY-GOES-HERE]

:: Clean
IF EXIST %OUTPUT% DEL /S /Q %OUTPUT%

%BUILD% %CSPROJ% /t:clean

:: Build
%BUILD% %CSPROJ% /p:Configuration=Release

:: Pack the Digst.OioIdws.Wsp project
%NUGET% pack %CSPROJ% ^
  -Symbols -Properties Configuration=Release -IncludeReferencedProjects ^
  -OutputDirectory %OUTPUT%

:: Push the package(s)
for %%f in (.\nuget\*.nupkg) do (
   (echo "%%f" | FIND /I ".symbols." 1>NUL) || (
     %NUGET% push %%f -Source https://www.nuget.org/api/v2/package
   )
)

pause