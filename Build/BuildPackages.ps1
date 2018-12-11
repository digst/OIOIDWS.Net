param(
    [string] [parameter(Mandatory = $true)] $version, 
    [string] [parameter(Mandatory = $true)] $assemblyVersion, 
    [switch] $pushPackages)

$names = @(
    'Wsc',
    'Wsp',
    'Rest.Client',
    'Rest.Server',
    'SamlAttributes',
    'Healthcare.Wsc',
    'Healthcare.SamlAttributes'
    )

$ErrorActionPreference = "Stop"


function Build-Package($name) 
{
    write-host "Building $name" -ForegroundColor Yellow
    .\nuget.exe pack ..\Source\$name\$name.csproj -build -Version $version -Symbols -Properties Configuration=Release -IncludeReferencedProjects >4
}

function Push-Package($name) 
{
    write-host "pushing package $name" -ForegroundColor Yellow
    .\nuget.exe push $("$name.$version.nupkg") -Source https://www.nuget.org/api/v2/package 
}


if($pushPackages.IsPresent)
{
    $names | %{ Push-Package "Digst.OioIdws.$_" }
}
else
{
    write-host "Generating assembly versioning" -ForegroundColor Yellow

    "using System.Reflection; 

    [assembly: AssemblyVersion(`"$assemblyversion`")]
    [assembly: AssemblyFileVersion(`"$assemblyversion`")]
    [assembly: AssemblyInformationalVersion(`"$assemblyversion`")]" | sc ..\source\CommonAssemblyInfo.cs

    write-host "Restoring nuget packages" -ForegroundColor Yellow
    .\nuget.exe restore ..

    $names | %{ Build-Package "Digst.OioIdws.$_" }

}