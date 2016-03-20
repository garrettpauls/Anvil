Framework 4.6.1

properties {
    $nuget   = Join-Path $psake.build_script_dir 'nuget.exe'
    $sln     = Resolve-Path (Join-Path $psake.build_script_dir '../Anvil.sln')
    $pkgDir  = Resolve-Path (Join-Path $psake.build_script_dir '../packages')
    $msbuild = 'C:\Program Files (x86)\MSBuild\14.0\Bin\msbuild.exe'
}

task default `
     -depends Compile

task Publish `
     -depends NugetRestore, Clean, Compile `
{
    $squirrelDir = Join-Path $pkgDir (Get-ChildItem $pkgDir -Filter squirrel.windows.* -Name | Sort-Object -Descending | Select-Object -First 1)
    $squirrel    = Join-Path $squirrelDir 'tools/Squirrel.exe'

    Write-Output "TODO: build squirrel files with $squirrel"
}

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
