@echo Building publishing XamlStyler.Console
dotnet publish ..\..\XamlStyler.Console\XamlStyler.Console.csproj -r win-x64 -c Release /p:PublishSingleFile=true /p:PublishTrimmed=true
dotnet publish ..\..\XamlStyler.Console\XamlStyler.Console.csproj -r win-x86 -c Release /p:PublishSingleFile=true /p:PublishTrimmed=true
dotnet publish ..\..\XamlStyler.Console\XamlStyler.Console.csproj -r linux-x64 -c Release /p:PublishSingleFile=true /p:PublishTrimmed=true
dotnet publish ..\..\XamlStyler.Console\XamlStyler.Console.csproj -r osx-x64 -c Release /p:PublishSingleFile=true /p:PublishTrimmed=true

@echo Packing nuget package XamlStyler.CommandLine
%USERPROFILE%\.nuget\packages\nuget.commandline\5.3.1\tools\NuGet.exe pack XamlStyler.Console.nuspec -OutputDirectory nupkgs

@echo Succeed