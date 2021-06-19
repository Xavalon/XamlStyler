[CmdletBinding()]
param (
    [Parameter(Position = 0, Mandatory = $true)]
    [string]
    [ValidatePattern("^\d+\.\d+\.\d+$", ErrorMessage = "`nPlease provide a valid version number in the format <Major>.<Minor>.<Patch>.")]
    $Version
)

Write-Host "Set version: $Version ($Version)"

function Set-XmlElement {
    [CmdletBinding()]
    param (
        [Parameter(Position = 0, Mandatory = $true)]
        [System.Xml.XmlElement]
        $Parent,

        [Parameter(Position = 1, Mandatory = $true)]
        [string]
        $ElementName,

        [Parameter(Position = 2, Mandatory = $true)]
        [string]
        $ElementValue
    )

    if ($null -eq $Parent.$ElementName) {
        $Parent.AppendChild($Parent.OwnerDocument.CreateElement($ElementName)).InnerXml = $ElementValue
    } else {
        $Parent.$ElementName = $ElementValue
    }
}

function Update-VSIXManifest {
    [CmdletBinding()]
    param (
        [Parameter(Position = 0, Mandatory = $true)]
        [string]
        $VSIXManifestPath
    )
    Write-Host "Updating Version in VSIX Manifest '$VSIXManifestPath'"
    $content = [xml] (Get-Content $VSIXManifestPath)
    $content.PackageManifest.Metadata.Identity.Version = $Version
    $content.Save($VSIXManifestPath)
}

function Update-CSProj {
    [CmdletBinding()]
    param (
        [Parameter(Position = 0, Mandatory = $true)]
        [string]        
        $CSProjPath,

        [Parameter(Position = 1, Mandatory = $false)]
        [switch]
        $UpdatePackageVersion = $false
    )
    Write-Host "Updating Version in .csproj '$CSProjPath'"
    $content = [xml] (Get-Content $CSProjPath)
    $PropertyGroup = $content.Project.PropertyGroup[0]

    if ($true -eq $UpdatePackageVersion) {
        Set-XmlElement -Parent $PropertyGroup -ElementName "PackageVersion" -ElementValue $Version        
    }

    Set-XmlElement -Parent $PropertyGroup -ElementName "Version" -ElementValue $Version
    $content.Save($CSProjPath)
}

function Update-AssemblyInfo {
    [CmdletBinding()]
    param (
        [Parameter(Position = 0, Mandatory = $true)]
        [string]
        $AssemblyInfoPath
    )
    Write-Host "Updating Version in AssemblyInfo '$AssemblyInfoPath'"
    $content = Get-Content $AssemblyInfoPath
    $content = $content -creplace '(?<Prefix>\[assembly:\s*Assembly\w*?Version\(")\d+(?:\.\d+)*?(?<Postfix>"\)\])', "`${Prefix}$Version`${Postfix}"
    Set-Content $AssemblyInfoPath $content
}

Update-VSIXManifest -VSIXManifestPath $(Resolve-Path $PSScriptRoot\..\src\XamlStyler.Extension.Windows.VS2019AndEarlier\source.extension.vsixmanifest)
Update-VSIXManifest -VSIXManifestPath $(Resolve-Path $PSScriptRoot\..\src\XamlStyler.Extension.Windows.VS2022\source.extension.vsixmanifest)
Update-CSProj -CSProjPath $(Resolve-Path $PSScriptRoot\..\src\XamlStyler\XamlStyler.csproj)
Update-CSProj -CSProjPath $(Resolve-Path $PSScriptRoot\..\src\XamlStyler.Extension\XamlStyler.Extension.csproj)
Update-CSProj -CSProjPath $(Resolve-Path $PSScriptRoot\..\src\XamlStyler.Console\XamlStyler.Console.csproj) -UpdatePackageVersion
Update-AssemblyInfo -AssemblyInfoPath $(Resolve-Path $PSScriptRoot\..\src\XamlStyler.Extension.Windows.VS2019AndEarlier\Properties\AssemblyInfo.cs)
Update-AssemblyInfo -AssemblyInfoPath $(Resolve-Path $PSScriptRoot\..\src\XamlStyler.Extension.Windows.VS2022\Properties\AssemblyInfo.cs)
Update-AssemblyInfo -AssemblyInfoPath $(Resolve-Path $PSScriptRoot\..\src\XamlStyler.Extension.Mac\Properties\AssemblyInfo.cs)