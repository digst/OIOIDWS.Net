#Requires -RunAsAdministrator
$ErrorActionPreference = "Stop"

Push-Location

set-location $PSScriptRoot

. .\functions.ps1

write-host "WARNING: This will rebind the SSL certificate on 0.0.0.0:443 on your machine!"

$certpassword = ConvertTo-SecureString -String "Test1234" -AsPlainText -Force
$certpassword2 = ConvertTo-SecureString -String "test1234" -AsPlainText -Force

write-host "Installing boostrap example ssl certificate"
$bootstrapSslcertificate = Import-PfxCertificate 'misc\certificates\serviceprovider ssl.pfx' -Password $certpassword2 -CertStoreLocation Cert:\LocalMachine\My
$bootstrapSslcertificate = Import-PfxCertificate 'misc\certificates\serviceprovider ssl.pfx' -Password $certpassword2 -CertStoreLocation Cert:\LocalMachine\TrustedPeople
write-host "Installed boostrap example ssl certificate $($bootstrapSslcertificate.Thumbprint) in LocalMachine\My and LocalMachine\TrustedPeople. This ensures the certificate is trusted on your machine and browser"

write-host "Installing serviceprovider's signing certificate"
$serviceprovidercertificate = Import-PfxCertificate 'misc\certificates\SP and WSC (Oiosaml-net.dk TEST).pfx' -Password $certpassword2 -CertStoreLocation Cert:\LocalMachine\My
$serviceprovidercertificate = Import-PfxCertificate 'misc\certificates\SP and WSC (Oiosaml-net.dk TEST).pfx' -Password $certpassword2 -CertStoreLocation Cert:\LocalMachine\TrustedPeople
write-host "Installed serviceprovider's signing certificate $($serviceprovidercertificate.Thumbprint) in LocalMachine\My and LocalMachine\TrustedPeople. This ensures the certificate is trusted on your machine and browser"
write-host "This certificate is used by the demo website (service provider) as its signing certificate"

write-host "Installing STS certificate"
$stsCertificate = Import-Certificate 'misc\certificates\STS (Digitaliseringsstyrelsen - NemLog-in Test).cer' -CertStoreLocation Cert:\LocalMachine\My
write-host "Installed STS certificate $($stsCertificate.Thumbprint) in LocalMachine\My. This ensures the certificate can be reached by the example applications"

write-host "Installing WSP certificate for signature checks - beware: the WSC only requires the public key part to verify signatures from the WSP"
$wspCertificate = Import-PfxCertificate 'misc\certificates\WSP (wsp.oioidws-net.dk TEST).p12' -Password $certpassword -CertStoreLocation Cert:\LocalMachine\My
write-host "Installed WSP certificate $($wspCertificate.Thumbprint) in LocalMachine\My."

write-host "attempting to delete previous binding if it exists.."
"http delete sslcert ipport=0.0.0.0:20002" | netsh

write-host "Registering boostrap example ssl certificate $($bootstrapSslcertificate.Thumbprint) for SSL bindings for boostrap example site"
"http add sslcert ipport=0.0.0.0:20002 certhash=$($bootstrapSslcertificate.Thumbprint) appid={$([Guid]::NewGuid().ToString().ToUpper())}" | netsh

$username = [System.Security.Principal.WindowsIdentity]::GetCurrent().Name
write-host "Setting private key access for your identity $username on the WSC signing certificate $($serviceprovidercertificate.Thumbprint) in the certificate store"
Set-CertificatePermission $serviceprovidercertificate.Thumbprint $username
write-host "Setting private key access for your identity $username on the WSP signing certificate $($wspCertificate.Thumbprint) in the certificate store"
Set-CertificatePermission $wspCertificate.Thumbprint $username

add-HostEntry "127.0.0.1" "oiosaml-net.dk"

write-host "Setup completed!"
write-host "You should now open the solution in Visual Studio, build it and run it!"

Pop-Location