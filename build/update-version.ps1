$version = $args[0]
Write-Host "Set version: $version"

# Update VSIX Version
$vsixPath = Resolve-Path $PSScriptRoot\..\XamlStyler.Extension.Windows\source.extension.vsixmanifest
Write-Host $vsixPath
$content = [xml] (Get-Content $vsixPath)
$content.PackageManifest.Metadata.Identity.Version = $version
$content.Save($vsixPath)

# Update Console Version
$consolePath = Resolve-Path $PSScriptRoot\..\XamlStyler.Console\XamlStyler.Console.csproj
Write-Host $consolePath
$content = [xml] (Get-Content $consolePath)
$content.Project.PropertyGroup.PackageVersion = $version
$content.Save($consolePath)