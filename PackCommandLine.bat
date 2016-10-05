@Set SOLUTION=XamlStyler.sln

@echo Finding msbuild
for /f "skip=2 tokens=2,*" %%A in ('reg.exe query "HKLM\SOFTWARE\WOW6432Node\Microsoft\MSBuild\ToolsVersions\14.0" /v MSBuildToolsPath') do SET MSBUILDDIR=%%B
@IF NOT EXIST "%MSBUILDDIR%" goto MissingMSBuildToolsPath

@echo Building solution
"%MSBUILDDIR%msbuild.exe" /ToolsVersion:14.0 "%SOLUTION%" /p:configuration=Release /t:Clean,Rebuild /m

@echo Packing nuget package XamlStyler.CommandLine
packages\NuGet.CommandLine.3.4.3\tools\nuget.exe pack XamlStyler.Console.nuspec

@echo Succeed
@goto end
:MissingMSBuildToolsPath
@echo The MSBuild tools path from the registry '%MSBUILDDIR%' does not exist
:end