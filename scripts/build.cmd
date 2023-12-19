@echo off

set version=%1
set currentdir=%~dp0
set root=%currentdir%\..
set toolsdir=%root%\tools
set nuget=%toolsdir%\NuGet\NuGet.exe
set swindlerdir=%root%\Swindler
set releasedir=%swindlerdir%\bin\Release
set deploydir=%root%\deploy

if "%version%"=="" (
	echo Please specify which version to build as a parameter.
	echo.
	goto exit
)

echo This will build, tag, and release version %version% of Swindler.
echo.
echo Please make sure that all changes have been properly committed!
pause


if exist "%deploydir%" (
	echo Cleaning up old deploy dir %deploydir%
	rd %deploydir% /s/q
)

echo Building version %version%

dotnet build %swindlerdir%\Swindler -c Release


echo Packing...

echo Creating deploy dir %deploydir%
mkdir %deploydir%

dotnet pack %swindlerdir%\Swindler -c Release -o %deploydir% -p:PackageVersion %version%

echo Tagging...

git tag %version%

echo Pushing to NuGet.org...

%nuget% push %deploydir%\*.nupkg -Source https://nuget.org

:exit