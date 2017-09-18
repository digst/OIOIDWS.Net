#Requires -RunAsAdministrator
$ErrorActionPreference = "Stop"

Push-Location

set-location $PSScriptRoot

. .\functions.ps1

write-host "WARNING: This will rebind the SSL certificate on 0.0.0.0:443 on your machine!"

$certpassword = ConvertTo-SecureString -String "Test1234" -AsPlainText -Force
$certpassword2 = ConvertTo-SecureString -String "test1234" -AsPlainText -Force

write-host "Installing rest AS ssl certificate"
$restAsSslcertificate = Import-PfxCertificate '..\misc\certificates\REST AS SSL.pfx' -Password $certpassword -CertStoreLocation Cert:\LocalMachine\My
$restAsSslcertificate = Import-PfxCertificate '..\misc\certificates\REST AS SSL.pfx' -Password $certpassword -CertStoreLocation Cert:\LocalMachine\TrustedPeople
write-host "Installed rest AS example ssl certificate $($restAsSslcertificate.Thumbprint) in LocalMachine\My and LocalMachine\TrustedPeople. This ensures the certificate is trusted on your machine and browser"

write-host "Installing rest WSP ssl certificate"
$restWspSslcertificate = Import-PfxCertificate '..\misc\certificates\REST WSP SSL.pfx' -Password $certpassword -CertStoreLocation Cert:\LocalMachine\My
$restWspSslcertificate = Import-PfxCertificate '..\misc\certificates\REST WSP SSL.pfx' -Password $certpassword -CertStoreLocation Cert:\LocalMachine\TrustedPeople
write-host "Installed rest WSP example ssl certificate $($restWspSslcertificate.Thumbprint) in LocalMachine\My and LocalMachine\TrustedPeople. This ensures the certificate is trusted on your machine and browser"

write-host "Installing boostrap example ssl certificate"
$bootstrapSslcertificate = Import-PfxCertificate '..\misc\certificates\SP SSL (oiosaml-net.dk).pfx' -Password $certpassword2 -CertStoreLocation Cert:\LocalMachine\My
$bootstrapSslcertificate = Import-PfxCertificate '..\misc\certificates\SP SSL (oiosaml-net.dk).pfx' -Password $certpassword2 -CertStoreLocation Cert:\LocalMachine\TrustedPeople
write-host "Installed boostrap example ssl certificate $($bootstrapSslcertificate.Thumbprint) in LocalMachine\My and LocalMachine\TrustedPeople. This ensures the certificate is trusted on your machine and browser"

write-host "Installing serviceprovider's signing certificate"
$serviceprovidercertificate = Import-PfxCertificate '..\misc\certificates\SP and WSC (Oiosaml-net.dk TEST).pfx' -Password $certpassword2 -CertStoreLocation Cert:\LocalMachine\My
write-host "Installed serviceprovider's signing certificate $($serviceprovidercertificate.Thumbprint) in LocalMachine\My and LocalMachine\TrustedPeople. This ensures the certificate is trusted on your machine and browser"
write-host "This certificate is used by the demo website (service provider) as its signing certificate"

write-host "Installing STS certificate"
$stsCertificate = Import-Certificate '..\misc\certificates\STS (Digitaliseringsstyrelsen - NemLog-in Test).cer' -CertStoreLocation Cert:\LocalMachine\My
write-host "Installed STS certificate $($stsCertificate.Thumbprint) in LocalMachine\My. This ensures the certificate can be reached by the example applications"

write-host "Installing WSP certificate for signature checks - beware: the WSC only requires the public key part to verify signatures from the WSP"
$wspCertificate = Import-PfxCertificate '..\misc\certificates\WSP (wsp.oioidws-net.dk TEST).p12' -Password $certpassword -CertStoreLocation Cert:\LocalMachine\My
write-host "Installed WSP certificate $($wspCertificate.Thumbprint) in LocalMachine\My."

write-host "attempting to delete previous binding on port 20002 if it exists.."
"http delete sslcert ipport=0.0.0.0:20002" | netsh

write-host "Registering boostrap example ssl certificate $($bootstrapSslcertificate.Thumbprint) for SSL bindings for boostrap example site"
"http add sslcert ipport=0.0.0.0:20002 certhash=$($bootstrapSslcertificate.Thumbprint) appid={$([Guid]::NewGuid().ToString().ToUpper())}" | netsh

write-host "attempting to delete previous binding on port 10001 if it exists.."
"http delete sslcert ipport=0.0.0.0:10001" | netsh

write-host "Registering rest AS example ssl certificate $($restAsSslcertificate.Thumbprint) for SSL bindings for boostrap example site"
"http add sslcert ipport=0.0.0.0:10001 certhash=$($restAsSslcertificate.Thumbprint) appid={$([Guid]::NewGuid().ToString().ToUpper())}" | netsh

write-host "attempting to delete previous binding on port 10002 if it exists.."
"http delete sslcert ipport=0.0.0.0:10002" | netsh

write-host "Registering rest AS example ssl certificate $($restWspSslcertificate.Thumbprint) for SSL bindings for boostrap example site"
"http add sslcert ipport=0.0.0.0:10002 certhash=$($restWspSslcertificate.Thumbprint) appid={$([Guid]::NewGuid().ToString().ToUpper())}" | netsh

$username = [System.Security.Principal.WindowsIdentity]::GetCurrent().Name
write-host "Setting private key access for your identity $username on the WSC signing certificate $($serviceprovidercertificate.Thumbprint) in the certificate store"
Set-CertificatePermission $serviceprovidercertificate.Thumbprint $username
write-host "Setting private key access for your identity $username on the WSP signing certificate $($wspCertificate.Thumbprint) in the certificate store"
Set-CertificatePermission $wspCertificate.Thumbprint $username

add-HostEntry "127.0.0.1" "oiosaml-net.dk"
add-HostEntry "127.0.0.1" "digst.oioidws.rest.as"
add-HostEntry "127.0.0.1" "digst.oioidws.rest.wsp"
add-HostEntry "127.0.0.1" "Digst.OioIdws.Wsp"

write-host "Setup completed!"
write-host "You should now open the solution in Visual Studio, build it and run it!"

Pop-Location