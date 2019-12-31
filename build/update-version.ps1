$version = $args[0]
Write-Host "Set version: $version"

# Update VSIX Version
$vsixPath = Resolve-Path $PSScriptRoot\..\XamlStyler.Package\source.extension.vsixmanifest
Write-Host $vsixPath
$content = [xml] (Get-Content $vsixPath)
$content.PackageManifest.Metadata.Identity.Version = $version
$content.Save($vsixPath)

# Update Console Version
$consolePath = Resolve-Path $PSScriptRoot\..\XamlStyler.Console\XamlStyler.Console.csproj
Write-Host $consolePath
$content = [xml] (Get-Content $consolePath)
$content.Project.PropertyGroup.Version = $version
$content.Save($consolePath)

# Update Nuspec Version
$nuspecPath = Resolve-Path $PSScriptRoot\..\build\nuget\XamlStyler.Console.nuspec
Write-Host $nuspecPath
$content = [xml] (Get-Content $nuspecPath)
$content.Package.Metadata.Version = $version
$content.Save($nuspecPath)