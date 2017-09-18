param(
    [string] [parameter(Mandatory = $true)] $version, 
    [string] [parameter(Mandatory = $true)] $assemblyVersion, 
    [switch] $pushPackages)

$ErrorActionPreference = "Stop"

if($pushPackages.IsPresent)
{
    write-host "pushing package Digst.OioIdws.Wsc" -ForegroundColor Yellow
    .\nuget.exe push $("Digst.OioIdws.Wsc.$version.nupkg") -Source https://www.nuget.org/api/v2/package

    write-host "pushing package Digst.OioIdws.Wsp" -ForegroundColor Yellow
    .\nuget.exe push $("Digst.OioIdws.Wsp.$version.nupkg") -Source https://www.nuget.org/api/v2/package
    
    write-host "pushing package Digst.OioIdws.Rest.Client" -ForegroundColor Yellow
    .\nuget.exe push $("Digst.OioIdws.Rest.Client.$version.nupkg") -Source https://www.nuget.org/api/v2/package

    write-host "pushing package Digst.OioIdws.Rest.Server" -ForegroundColor Yellow
    .\nuget.exe push $("Digst.OioIdws.Rest.Server.$version.nupkg") -Source https://www.nuget.org/api/v2/package
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

    write-host "Building nuget package dk.nita.saml20" -ForegroundColor Yellow
    .\nuget.exe pack ..\Source\Digst.OioIdws.Wsc\Digst.OioIdws.Wsc.csproj -build -Version $version -Symbols -Properties Configuration=Release -IncludeReferencedProjects

    write-host "Building nuget package Digst.OioIdws.Wsp" -ForegroundColor Yellow
    .\nuget.exe pack ..\Source\Digst.OioIdws.Wsp\Digst.OioIdws.Wsp.csproj -build -Version $version -Symbols -Properties Configuration=Release -IncludeReferencedProjects

    write-host "Building nuget package Digst.OioIdws.Rest.Client" -ForegroundColor Yellow
    .\nuget.exe pack ..\Source\Digst.OioIdws.Rest.Client\Digst.OioIdws.Rest.Client.csproj -build -Version $version -Symbols -Properties Configuration=Release -IncludeReferencedProjects

    write-host "Building nuget package Digst.OioIdws.Rest.Server" -ForegroundColor Yellow
    .\nuget.exe pack ..\Source\Digst.OioIdws.Rest.Server\Digst.OioIdws.Rest.Server.csproj -build -Version $version -Symbols -Properties Configuration=Release -IncludeReferencedProjects
}