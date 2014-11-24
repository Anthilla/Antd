@echo off

:: Check prerequisites
set _msbuildexe="%ProgramFiles(x86)%\MSBuild\12.0\Bin\MSBuild.exe"
if not exist %_msbuildexe% set _msbuildexe="%ProgramFiles%\MSBuild\12.0\Bin\MSBuild.exe"
if not exist %_msbuildexe% set _msbuildexe="%ProgramFiles(x86)%\MSBuild\14.0\Bin\MSBuild.exe"
if not exist %_msbuildexe% set _msbuildexe="%ProgramFiles%\MSBuild\14.0\Bin\MSBuild.exe"
if not exist %_msbuildexe% echo Error: Could not find MSBuild.exe.  Please see https://github.com/dotnet/corefx/blob/master/docs/Developers.md for build instructions. && goto :eof


if not exist .nuget\NuGet.exe %_msbuildexe% .nuget\NuGet.targets /t:CheckPrerequisites
if not exist .nuget\NuGet.exe echo Error: Failed to get NuGet.exe

%_msbuildexe% Antd.sln /p:Configuration=Release
