@echo off

set KEYTOOL="C:\Program Files\Java\jre1.8.0_162\bin\keytool.exe"

set CERT_CA1="..\..\..\Misc\Certificates\TRUST2048 Systemtest VII Primary CA.cer"
set CERT_CA2="..\..\..\Misc\Certificates\TRUST2048 Systemtest XIX CA.cer"
set CERT_STS="..\..\..\Misc\Certificates\STS (Digitaliseringsstyrelsen - NemLog-in Test).cer"
set CERT_WSP="..\..\..\Misc\Certificates\java\wsp\wsp.oioidws-net.dk TEST (funktionscertifikat).cer"

set CERT_NLI="..\..\..\Misc\Certificates\java\ssl\test-nemlog-in.dk.cer"
set CERT_SSL="..\..\..\Misc\Certificates\java\ssl\digst.oioidws.wsp.cer"

set CERT_WSC="..\..\..\Misc\Certificates\java\wsc\Oiosaml-net.dk TEST (funktionscertifikat).cer"
set CERT_PFX="..\..\..\Misc\Certificates\SP and WSC (Oiosaml-net.dk TEST).pfx"

set KEYSTORE_STS=".\trust-sts.jks"
set KEYSTORE_SSL=".\trust-ssl.jks"
set KEYSTORE_WSC=".\trust-wsc.jks"

set PWD00="Test1234"
set PWD01="test1234"

:: Clean up first
IF EXIST %KEYSTORE_STS% DEL /F %KEYSTORE_STS%
IF EXIST %KEYSTORE_SSL% DEL /F %KEYSTORE_SSL%
IF EXIST %KEYSTORE_WSC% DEL /F %KEYSTORE_WSC%

:: STS Primary CA
%KEYTOOL% -v -import -trustcacerts ^
  -keystore %KEYSTORE_STS% ^
  -keypass %PWD00% -storepass %PWD00% ^
  -noprompt -alias sts_ca1 -file %CERT_CA1%

:: STS Secondary CA
%KEYTOOL% -v -import -trustcacerts ^
  -destkeystore %KEYSTORE_STS% -deststoretype JKS ^
  -keypass %PWD00% -storepass %PWD00% ^
  -noprompt -alias sts_ca2 -file %CERT_CA2%

:: STS
%KEYTOOL% -v -import -trustcacerts ^
  -destkeystore %KEYSTORE_STS% -deststoretype JKS ^
  -keypass %PWD00% -storepass %PWD00% ^
  -noprompt -alias sts -file %CERT_STS%
:: NOTE: Ensure that alias matches the one in "trust.jks"

:: WSP
%KEYTOOL% -v -import -trustcacerts ^
  -destkeystore %KEYSTORE_STS% -deststoretype JKS ^
  -keypass %PWD00% -storepass %PWD00% ^
  -noprompt -alias server -file %CERT_WSP%
:: NOTE: Ensure that alias matches the one in "trust.jks"

:: STS content
%KEYTOOL% -list ^
  -keystore %KEYSTORE_STS% ^
  -keypass %PWD00% -storepass %PWD00%

:: SSL
%KEYTOOL% -v -import -trustcacerts ^
  -keystore %KEYSTORE_SSL% ^
  -keypass %PWD00% -storepass %PWD00% ^
  -noprompt -alias server -file %CERT_SSL%

:: SSL (NemLoging)
%KEYTOOL% -v -import -trustcacerts ^
  -destkeystore %KEYSTORE_SSL% -deststoretype JKS ^
  -keypass %PWD00% -storepass %PWD00% ^
  -noprompt -alias nemlogin -file %CERT_NLI%

:: SSL content
%KEYTOOL% -list ^
  -keystore %KEYSTORE_SSL% ^
  -keypass %PWD00% -storepass %PWD00%

:: WSC (probably not neccesary as private keys contain public)
::KEYTOOL% -v -import -trustcacerts ^
::  -keystore %KEYSTORE_WSC% ^
::  -keypass %PWD00% -storepass %PWD00% ^
::  -noprompt -alias client_pub -file %CERT_WSC%

:: WSC (Private key for signing and encrypting)
%KEYTOOL% -v -importkeystore ^
  -srckeystore %CERT_PFX% -srcstoretype PKCS12 ^
  -srckeypass %PWD01% -srcstorepass %PWD01% ^
  -destkeystore %KEYSTORE_WSC% -deststoretype JKS ^
  -keypass %PWD00% -storepass %PWD00% ^
  -noprompt -alias "oiosaml-net.dk test (funktionscertifikat)" ^
  -destalias "oiosaml-net.dk test (funktionscertifikat)"
:: NOTE: Ensure that alias matches the one in "client.jks"

:: WSC (Ensure same password for key and store)
%KEYTOOL% -v -keypasswd ^
  -new %PWD00% ^
  -keystore  %KEYSTORE_WSC% ^
  -keypass %PWD01% -storepass %PWD00% ^
  -noprompt -alias "oiosaml-net.dk test (funktionscertifikat)"

:: WSC content
%KEYTOOL% -list ^
  -keystore %KEYSTORE_WSC% ^
  -keypass %PWD00% -storepass %PWD00%

pause