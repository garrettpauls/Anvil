Framework 4.6.1

properties {
    $nuget   = Join-Path $psake.build_script_dir 'nuget.exe'
    $rootDir = Resolve-Path (Join-Path $psake.build_script_dir '..')
    $sln     = Join-Path $rootDir 'Anvil.sln'
    $nuspec  = Join-Path $rootDir 'Source/Anvil/Anvil.nuspec'
    $pkgDir  = Join-Path $rootDir 'packages'
    $msbuild = 'C:\Program Files (x86)\MSBuild\14.0\Bin\msbuild.exe'
    $temp    = $env:temp
    $releaseDir = Join-Path $rootDir 'Releases'
}

function Get-Version {
    $assemblyInfo = Get-Content "$rootDir/Source/Anvil/Properties/AssemblyInfo.cs"
    return ([regex]'AssemblyVersion\("([^"]+)"\)').match($assemblyInfo).groups[1].value
}

task default `
     -depends Compile

task Publish `
     -depends NugetRestore, Clean, Compile, PackageNuspec, SquirrelReleasify

task Compile `
     -alias Build `
     -description 'Compiles the solution' `
     -depends NugetRestore `
{
    Exec { msbuild "$sln" /t:Build /p:Configuration=Release /v:n }
}

task Clean `
     -description 'Removes existing build artifacts' `
{
    Exec { msbuild "$sln" /t:Clean /p:Configuration=Release /v:n }
}

task NugetRestore `
     -description 'Restores nuget packages' `
{
    Exec { &"$nuget" restore "$sln" }
}

task PackageNuspec `
     -depends Compile, UpdateNuspec `
{
    Exec { &"$nuget" pack "$nuspec" -outputDirectory $temp }
}

task UpdateNuspec `
{
    [xml]$xml = Get-Content "$nuspec"

    # Update version
    $version = Get-Version
    $xml.package.metadata.version = $version

    # Update release notes
    $releaseNotes = [System.IO.File]::ReadAllText((Join-Path $rootDir 'release-notes.md'))
    $xml.package.metadata.releaseNotes = $releaseNotes

    $xml.Save($nuspec)
}

task SquirrelReleasify `
     -depends PackageNuspec `
{
    $version = Get-Version
    $target = Join-Path $temp "Anvil.$version.nupkg"
    
    $squirrelDir = Join-Path $pkgDir (Get-ChildItem $pkgDir -Filter squirrel.windows.* -Name | Sort-Object -Descending | Select-Object -First 1)
    $squirrel    = Join-Path $squirrelDir 'tools/Squirrel.com'

    Exec { &"$squirrel" --releasify "$target" --releaseDir "$releaseDir" }
}
