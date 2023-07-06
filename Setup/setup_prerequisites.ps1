#Requires -RunAsAdministrator
$ErrorActionPreference = "Stop"

Push-Location

set-location $PSScriptRoot

. .\functions.ps1

write-host "WARNING: This will rebind the SSL certificate on 0.0.0.0:443 on your machine!"

$certpassword  = ConvertTo-SecureString -String "Test1234" -AsPlainText -Force
$certpassword2 = ConvertTo-SecureString -String "test1234" -AsPlainText -Force
$certpasswordWsc = ConvertTo-SecureString -String "B3fL5V+GvZSX" -AsPlainText -Force
$certpasswordWsp = ConvertTo-SecureString -String "N6kfQ:BLQly?" -AsPlainText -Force
$certpasswordLocalSts = ConvertTo-SecureString -String "IDjgECGZR9;R" -AsPlainText -Force

# Add "TRUST2048 Systemtest VII Primary CA" to Trusted ROOT CA
write-host "Installing serviceprovider's root certificate"
$trust2048SystemtestRootCertificate = Import-Certificate '..\misc\certificates\TRUST2048 Systemtest VII Primary CA.cer' -CertStoreLocation Cert:\LocalMachine\Root
write-host -ForegroundColor Green "Installed serviceprovider's signing certificate $($trust2048SystemtestRootCertificate.Thumbprint) in LocalMachine\Root. This ensures that the other certificate are trusted on your machine and browser"

# Add "TRUST2048 Systemtest XIX CA.cer" to Intermediate Certification Authorities
write-host "Installing serviceprovider's intermediate certificate"
$trust2048SystemtestIntermediateCertificate19 = Import-Certificate '..\misc\certificates\TRUST2048 Systemtest VII Primary CA.cer' -CertStoreLocation Cert:\LocalMachine\CA
write-host -ForegroundColor Green "Installed serviceprovider's signing certificate $($trust2048SystemtestIntermediateCertificate19.Thumbprint) in LocalMachine\CA. This ensures that the other certificate are trusted on your machine and browser"

# Add "TRUST2408 Systemtest XXII CA.cer" to Intermediate Certification Authorities
write-host "Installing serviceprovider's intermediate certificate"
$trust2048SystemtestIntermediateCertificate22 = Import-Certificate '..\misc\certificates\TRUST2048 Systemtest VII Primary CA.cer' -CertStoreLocation Cert:\LocalMachine\CA
write-host -ForegroundColor Green "Installed serviceprovider's signing certificate $($trust2048SystemtestIntermediateCertificate22.Thumbprint) in LocalMachine\CA. This ensures that the other certificate are trusted on your machine and browser"


write-host "Installing soap WSP ssl certificate"
$soapWspSslcertificate = Import-PfxCertificate '..\misc\certificates\SOAP WSP SSL (digst.oioidws.wsp).pfx' -Password $certpassword2 -CertStoreLocation Cert:\LocalMachine\My
$soapWspSslcertificate = Import-PfxCertificate '..\misc\certificates\SOAP WSP SSL (digst.oioidws.wsp).pfx' -Password $certpassword2 -CertStoreLocation Cert:\LocalMachine\TrustedPeople
write-host -ForegroundColor Green "Installed soap WSP example certificate $($soapWspSslcertificate.Thumbprint) in LocalMachine\My and LocalMachine\TrustedPeople. This ensures the certificate is trusted on your machine and browser"

write-host "Installing rest AS ssl certificate"
$restAsSslcertificate = Import-PfxCertificate '..\misc\certificates\REST AS SSL.pfx' -Password $certpassword -CertStoreLocation Cert:\LocalMachine\My
$restAsSslcertificate = Import-PfxCertificate '..\misc\certificates\REST AS SSL.pfx' -Password $certpassword -CertStoreLocation Cert:\LocalMachine\TrustedPeople
write-host -ForegroundColor Green "Installed rest AS example ssl certificate $($restAsSslcertificate.Thumbprint) in LocalMachine\My and LocalMachine\TrustedPeople. This ensures the certificate is trusted on your machine and browser"

write-host "Installing rest WSP ssl certificate"
$restWspSslcertificate = Import-PfxCertificate '..\misc\certificates\REST WSP SSL.pfx' -Password $certpassword -CertStoreLocation Cert:\LocalMachine\My
$restWspSslcertificate = Import-PfxCertificate '..\misc\certificates\REST WSP SSL.pfx' -Password $certpassword -CertStoreLocation Cert:\LocalMachine\TrustedPeople
write-host -ForegroundColor Green "Installed rest WSP example ssl certificate $($restWspSslcertificate.Thumbprint) in LocalMachine\My and LocalMachine\TrustedPeople. This ensures the certificate is trusted on your machine and browser"

write-host "Installing boostrap example ssl certificate"
$bootstrapSslcertificate = Import-PfxCertificate '..\misc\certificates\SP SSL (oiosaml-net.dk).pfx' -Password $certpassword2 -CertStoreLocation Cert:\LocalMachine\My
$bootstrapSslcertificate = Import-PfxCertificate '..\misc\certificates\SP SSL (oiosaml-net.dk).pfx' -Password $certpassword2 -CertStoreLocation Cert:\LocalMachine\TrustedPeople
write-host -ForegroundColor Green "Installed boostrap example ssl certificate $($bootstrapSslcertificate.Thumbprint) in LocalMachine\My and LocalMachine\TrustedPeople. This ensures the certificate is trusted on your machine and browser"

write-host "Installing serviceprovider's signing certificate"
$serviceprovidercertificate = Import-PfxCertificate '..\misc\certificates\OIOIDWS.NET_WSC_-_Test_3DES.p12' -Password $certpasswordWsc -CertStoreLocation Cert:\LocalMachine\My
$serviceprovidercertificate = Import-PfxCertificate '..\misc\certificates\OIOIDWS.NET_WSC_-_Test_3DES.p12' -Password $certpasswordWsc -CertStoreLocation Cert:\LocalMachine\TrustedPeople
write-host -ForegroundColor Green "Installed serviceprovider's signing certificate $($serviceprovidercertificate.Thumbprint) in LocalMachine\My and LocalMachine\TrustedPeople. This ensures the certificate is trusted on your machine and browser"

write-host "Installing STS certificate"
$stsCertificate = Import-Certificate '..\misc\certificates\NemLog-in IdP - Test.cer' -CertStoreLocation Cert:\LocalMachine\My
write-host -ForegroundColor Green "Installed STS certificate $($stsCertificate.Thumbprint) in LocalMachine\My. This ensures the certificate can be reached by the example applications"

write-host "Installing WSP certificate for signature checks - beware: the WSC only requires the public key part to verify signatures from the WSP"
$wspCertificate = Import-PfxCertificate '..\misc\certificates\OIOIDWS.NET_WSP_-_Test_3DES.p12' -Password $certpasswordWsp -CertStoreLocation Cert:\LocalMachine\My
$wspCertificate = Import-PfxCertificate '..\misc\certificates\OIOIDWS.NET_WSP_-_Test_3DES.p12' -Password $certpasswordWsp -CertStoreLocation Cert:\LocalMachine\TrustedPeople
write-host -ForegroundColor Green "Installed WSP example certificate $($wspCertificate.Thumbprint) in LocalMachine\My."

write-host "Installing Local Token certificate for a local token issuer"
$localTokenCertificate = Import-PfxCertificate '..\misc\certificates\OIOIDWS.NET_LocalSTS_-_Test_3DES.p12' -Password $certpasswordLocalSts -CertStoreLocation Cert:\LocalMachine\My
write-host -ForegroundColor Green "Installed Local Token certificate $($localTokenCertificate.Thumbprint) in LocalMachine\My."

# Java - BEGIN

write-host "Installing soap WSP (Java) ssl certificate"
$soapWspJavaSslCertificate = Import-PfxCertificate '..\misc\certificates\java\ssl-keystore.pfx' -Password $certpassword -CertStoreLocation Cert:\LocalMachine\My
$soapWspJavaSslCertificate = Import-PfxCertificate '..\misc\certificates\java\ssl-keystore.pfx' -Password $certpassword -CertStoreLocation Cert:\LocalMachine\TrustedPeople
write-host -ForegroundColor Green "Installed soap WSP (Java) ssl certificate $($soapWspJavaSslCertificate.Thumbprint) in LocalMachine\My and LocalMachine\TrustedPeople. This ensures the certificate is trusted on your machine and browser"

write-host "Installing soap WSP (Java) service certificate"
$soapWspJavaSystemCertificate = Import-PfxCertificate '..\misc\certificates\java\service.pfx' -Password $certpassword -CertStoreLocation Cert:\LocalMachine\My
write-host -ForegroundColor Green "Installed soap WSP (Java) service certificate $($soapWspJavaSystemCertificate.Thumbprint) in LocalMachine\My. This ensures the certificate is trusted on your machine and browser"

write-host "Installing soap WSC (Java) certificate"
$soapWscJavaCertificate = Import-PfxCertificate '..\misc\certificates\java\client.pfx' -Password $certpassword -CertStoreLocation Cert:\LocalMachine\My
write-host -ForegroundColor Green "Installed soap WSC (Java) certificate $($soapWscJavaCertificate.Thumbprint) in LocalMachine\My. This ensures the certificate is trusted on your machine and browser"

# Java - END

write-host "attempting to delete previous binding on port 9090 if it exists.."
"http delete sslcert ipport=0.0.0.0:9090" | netsh

write-host "attempting to delete previous binding on port 9091 if it exists.."
"http delete sslcert ipport=0.0.0.0:9091" | netsh

write-host "Registering soap WSP example idp ssl certificate $($soapWspSslcertificate.Thumbprint) for SSL bindings for soap WSP example"
"http add sslcert ipport=0.0.0.0:9090 certhash=$($soapWspSslcertificate.Thumbprint) appid={$([Guid]::NewGuid().ToString().ToUpper())}" | netsh
"http add sslcert ipport=0.0.0.0:9091 certhash=$($soapWspSslcertificate.Thumbprint) appid={$([Guid]::NewGuid().ToString().ToUpper())}" | netsh

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
write-host "Setting private key access for your identity $username on the SOAP WSP signing certificate $($soapWspSslcertificate.Thumbprint) in the certificate store"
Set-CertificatePermission $soapWspSslcertificate.Thumbprint $username
write-host "Setting private key access for your identity $username on the Local Token $($localTokenCertificate.Thumbprint) in the certificate store"
Set-CertificatePermission $localTokenCertificate.Thumbprint $username


add-HostEntry "127.0.0.1" "oiosaml-net.dk"
add-HostEntry "127.0.0.1" "digst.oioidws.rest.as"
add-HostEntry "127.0.0.1" "digst.oioidws.rest.wsp"
add-HostEntry "127.0.0.1" "digst.oioidws.wsp"

write-host -ForegroundColor Green "Setup completed!"
write-host -ForegroundColor Yellow "You should now open the solution in Visual Studio, build it and run it!"

Pop-Location