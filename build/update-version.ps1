$version = $args[0]
Write-Host "Set version: $version"

# TODO: Replace the path with the path to your vsixmanifest file
$FullPath = Resolve-Path $PSScriptRoot\..\XamlStyler.Package\source.extension.vsixmanifest
Write-Host $FullPath
[xml]$content = Get-Content $FullPath
$content.PackageManifest.Metadata.Identity.Version = $version
$content.Save($FullPath)