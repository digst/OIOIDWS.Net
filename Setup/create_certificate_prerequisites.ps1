#Requires -RunAsAdministrator
$ErrorActionPreference = "Stop"

Push-Location

set-location $PSScriptRoot

New-SelfSignedCertificate -DnsName "digst.oioidws.soap.wsp" -NotAfter "2030-01-01" -Provider "Microsoft Enhanced Cryptographic Provider v1.0"

write-host "Certificate created!"

Pop-Location