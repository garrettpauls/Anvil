Param(
    [ValidateSet("Clean", "Compile", "NugetRestore", "Publish", "Release")]
    [string]$Target = "Compile",
    [switch]$Help
)

Import-Module $PSScriptRoot\.tools\psake\psake.psm1

#$psake.log_error = $true
$buildScript = "$PSScriptRoot\.tools\default.ps1"

if($Help)
{
    Invoke-psake $buildScript -docs
}
else
{
    Invoke-psake $buildScript $Target
}
