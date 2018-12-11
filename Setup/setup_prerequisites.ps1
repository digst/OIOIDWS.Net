#Requires -RunAsAdministrator
$ErrorActionPreference = "Stop"

Push-Location

set-location $PSScriptRoot

. .\functions.ps1

write-host "WARNING: This will rebind the SSL certificate on 0.0.0.0:443 on your machine!"

$certpassword  = ConvertTo-SecureString -String "Test1234" -AsPlainText -Force
$certpassword2 = ConvertTo-SecureString -String "test1234" -AsPlainText -Force

# Add "TRUST2048 Systemtest VII Primary CA" to Trusted ROOT CA
write-host "Installing serviceprovider's root certificate"
$trust2048SystemtestRootCertificate = Import-Certificate '..\misc\certificates\TRUST2048 Systemtest VII Primary CA.cer' -CertStoreLocation Cert:\LocalMachine\Root
write-host "Installed serviceprovider's signing certificate $($trust2048SystemtestRootCertificate.Thumbprint) in LocalMachine\Root. This ensures that the other certificate are trusted on your machine and browser"

# Add "TRUST2048 Systemtest XIX CA.cer" to Intermediate Certification Authorities
write-host "Installing serviceprovider's intermediate certificate"
$trust2048SystemtestIntermediateCertificate19 = Import-Certificate '..\misc\certificates\TRUST2048 Systemtest VII Primary CA.cer' -CertStoreLocation Cert:\LocalMachine\CA
write-host "Installed serviceprovider's signing certificate $($trust2048SystemtestIntermediateCertificate19.Thumbprint) in LocalMachine\CA. This ensures that the other certificate are trusted on your machine and browser"

# Add "TRUST2408 Systemtest XXII CA.cer" to Intermediate Certification Authorities
write-host "Installing serviceprovider's intermediate certificate"
$trust2048SystemtestIntermediateCertificate22 = Import-Certificate '..\misc\certificates\TRUST2048 Systemtest VII Primary CA.cer' -CertStoreLocation Cert:\LocalMachine\CA
write-host "Installed serviceprovider's signing certificate $($trust2048SystemtestIntermediateCertificate22.Thumbprint) in LocalMachine\CA. This ensures that the other certificate are trusted on your machine and browser"


write-host "Installing soap WSP ssl certificate"
$soapWspSslcertificate = Import-PfxCertificate '..\misc\certificates\SOAP WSP SSL (digst.oioidws.wsp).pfx' -Password $certpassword2 -CertStoreLocation Cert:\LocalMachine\My
$soapWspSslcertificate = Import-PfxCertificate '..\misc\certificates\SOAP WSP SSL (digst.oioidws.wsp).pfx' -Password $certpassword2 -CertStoreLocation Cert:\LocalMachine\TrustedPeople
write-host "Installed soap WSP example certificate $($soapWspSslcertificate.Thumbprint) in LocalMachine\My and LocalMachine\TrustedPeople. This ensures the certificate is trusted on your machine and browser"

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
$serviceprovidercertificate = Import-PfxCertificate '..\misc\certificates\SP and WSC (Oiosaml-net.dk TEST).pfx' -Password $certpassword2 -CertStoreLocation Cert:\LocalMachine\TrustedPeople
write-host "Installed serviceprovider's signing certificate $($serviceprovidercertificate.Thumbprint) in LocalMachine\My and LocalMachine\TrustedPeople. This ensures the certificate is trusted on your machine and browser"

write-host "Installing NemLog-in STS certificate"
$stsCertificate = Import-Certificate '..\misc\certificates\STS (Digitaliseringsstyrelsen - NemLog-in Test).cer' -CertStoreLocation Cert:\LocalMachine\My
write-host "Installed STS certificate $($stsCertificate.Thumbprint) in LocalMachine\My. This ensures the certificate can be reached by the example applications"

write-host "Installing intermediate healthcare-STS certificate"
$stsHealthcareCertificate = Import-Certificate '..\misc\certificates\Test healthcare STS (funktionscertifikat).cer' -CertStoreLocation Cert:\LocalMachine\My
write-host "Installed STS certificate $($stsHealthcareCertificate.Thumbprint) in LocalMachine\My. This ensures the certificate can be reached by the example applications"

write-host "Installing WSP certificate for signature checks - beware: the WSC only requires the public key part to verify signatures from the WSP"
$wspCertificate = Import-PfxCertificate '..\misc\certificates\WSP (wsp.oioidws-net.dk TEST).p12' -Password $certpassword -CertStoreLocation Cert:\LocalMachine\My
$wspCertificate = Import-PfxCertificate '..\misc\certificates\WSP (wsp.oioidws-net.dk TEST).p12' -Password $certpassword -CertStoreLocation Cert:\LocalMachine\TrustedPeople
write-host "Installed WSP example certificate $($wspCertificate.Thumbprint) in LocalMachine\My."

# Java - BEGIN

write-host "Installing Test healthcare STS SSL certificate"
$HealthcareStsSslCertificate = Import-Certificate '..\misc\certificates\Test healthcare STS (SSL).cer' -CertStoreLocation Cert:\LocalMachine\Root
write-host "Installed healthcare STS SSL certificate $($HealthcareStsSslCertificate.Thumbprint) in LocalMachine\Root."


write-host "Installing soap WSP (Java) ssl certificate"
$soapWspJavaSslCertificate = Import-PfxCertificate '..\misc\certificates\java\ssl-keystore.pfx' -Password $certpassword -CertStoreLocation Cert:\LocalMachine\My
$soapWspJavaSslCertificate = Import-PfxCertificate '..\misc\certificates\java\ssl-keystore.pfx' -Password $certpassword -CertStoreLocation Cert:\LocalMachine\TrustedPeople
write-host "Installed soap WSP (Java) ssl certificate $($soapWspJavaSslCertificate.Thumbprint) in LocalMachine\My and LocalMachine\TrustedPeople. This ensures the certificate is trusted on your machine and browser"

write-host "Installing soap WSP (Java) service certificate"
$soapWspJavaSystemCertificate = Import-PfxCertificate '..\misc\certificates\java\service.pfx' -Password $certpassword -CertStoreLocation Cert:\LocalMachine\My
write-host "Installed soap WSP (Java) service certificate $($soapWspJavaSystemCertificate.Thumbprint) in LocalMachine\My. This ensures the certificate is trusted on your machine and browser"

write-host "Installing soap WSC (Java) certificate"
$soapWscJavaCertificate = Import-PfxCertificate '..\misc\certificates\java\client.pfx' -Password $certpassword -CertStoreLocation Cert:\LocalMachine\My
write-host "Installed soap WSC (Java) certificate $($soapWscJavaCertificate.Thumbprint) in LocalMachine\My. This ensures the certificate is trusted on your machine and browser"

# Java - END

write-host "attempting to delete previous binding on port 9090 if it exists.."
"http delete sslcert ipport=0.0.0.0:9090" | netsh

write-host "Registering soap WSP example idp ssl certificate $($soapWspSslcertificate.Thumbprint) for SSL bindings for soap WSP example"
"http add sslcert ipport=0.0.0.0:9090 certhash=$($soapWspSslcertificate.Thumbprint) appid={$([Guid]::NewGuid().ToString().ToUpper())}" | netsh

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


add-HostEntry "127.0.0.1" "oiosaml-net.dk"
add-HostEntry "127.0.0.1" "digst.oioidws.rest.as"
add-HostEntry "127.0.0.1" "digst.oioidws.rest.wsp"
add-HostEntry "127.0.0.1" "digst.oioidws.wsp"
add-HostEntry "127.0.0.1" "sts-idws-xua"
add-HostEntry "127.0.0.1" "idp-idws-xua"

write-host "Setup completed!"
write-host "You should now open the solution in Visual Studio, build it and run it!"

Pop-Location