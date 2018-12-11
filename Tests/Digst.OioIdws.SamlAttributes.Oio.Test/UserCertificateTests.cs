using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Digst.OioIdws.SecurityTokens.Tokens.ExtendedSaml2SecurityToken;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Digst.OioIdws.SamlAttributes.Oio.Test
{
    [TestClass]
    public class UserCertificateTests
    {
        [TestMethod]
        public void CanDeserializeAndSerializeUserCertificateAttribute()
        {
            var base64Der = @"MIIGLTCCBRWgAwIBAgIEWRuOgTANBgkqhkiG9w0BAQsFADBIMQswCQYDVQQGEwJE
SzESMBAGA1UECgwJVFJVU1QyNDA4MSUwIwYDVQQDDBxUUlVTVDI0MDggU3lzdGVt
dGVzdCBYWElJIENBMB4XDTE4MDIwMjE4MTYxOFoXDTIxMDIwMjE4MTYwMlowezEL
MAkGA1UEBhMCREsxMTAvBgNVBAoMKERpZ2l0YWxpc2VyaW5nc3N0eXJlbHNlbiAv
LyBDVlI6MzQwNTExNzgxOTAVBgNVBAMMDlRob21hcyDDhWxiw6ZrMCAGA1UEBRMZ
Q1ZSOjM0MDUxMTc4LVJJRDo5MDg5NzcwMjCCASIwDQYJKoZIhvcNAQEBBQADggEP
ADCCAQoCggEBAIjojLD+3cNWdYZagCSCNZrtL59NXmLYOp6A3tXiG0Kr4CDjc2K4
H3p94mM+C7+hSq+3bJmJ72T0jcX7k44Yr0tek5JPw4TJWZ3SYA7OYZ/wKkntcgLN
AK+DkSfb7aD96qzzziJ2j1IMEq4KfKb4chhyzaqEh5ZXPFylHOxD+6R4cT1YOJY5
SY7MHtwoi+majdneBOjD954rsDL4buBrdDtKGqw4ACbUDlsvs9mH8a0VT9iVBno2
rDjZt8WXA/YGd45kaFrA7Y7faCkBQCZ+Lwb6WbyfybI0ShndrAZWm/jpOw5vz4a0
X8JfHtfD318dYo8FSyTiGi0b0VR/1nKFvpECAwEAAaOCAuowggLmMA4GA1UdDwEB
/wQEAwID+DCBlwYIKwYBBQUHAQEEgYowgYcwPAYIKwYBBQUHMAGGMGh0dHA6Ly9v
Y3NwLnN5c3RlbXRlc3QyMi50cnVzdDI0MDguY29tL3Jlc3BvbmRlcjBHBggrBgEF
BQcwAoY7aHR0cDovL20uYWlhLnN5c3RlbXRlc3QyMi50cnVzdDI0MDguY29tL3N5
c3RlbXRlc3QyMi1jYS5jZXIwggEgBgNVHSAEggEXMIIBEzCCAQ8GDSsGAQQBgfRR
AgQGAgUwgf0wLwYIKwYBBQUHAgEWI2h0dHA6Ly93d3cudHJ1c3QyNDA4LmNvbS9y
ZXBvc2l0b3J5MIHJBggrBgEFBQcCAjCBvDAMFgVEYW5JRDADAgEBGoGrRGFuSUQg
dGVzdCBjZXJ0aWZpa2F0ZXIgZnJhIGRlbm5lIENBIHVkc3RlZGVzIHVuZGVyIE9J
RCAxLjMuNi4xLjQuMS4zMTMxMy4yLjQuNi4yLjUuIERhbklEIHRlc3QgY2VydGlm
aWNhdGVzIGZyb20gdGhpcyBDQSBhcmUgaXNzdWVkIHVuZGVyIE9JRCAxLjMuNi4x
LjQuMS4zMTMxMy4yLjQuNi4yLjUuMBwGA1UdEQQVMBOBEW5lbWxvZ2luQGRpZ3N0
LmRrMIGsBgNVHR8EgaQwgaEwPaA7oDmGN2h0dHA6Ly9jcmwuc3lzdGVtdGVzdDIy
LnRydXN0MjQwOC5jb20vc3lzdGVtdGVzdDIyMS5jcmwwYKBeoFykWjBYMQswCQYD
VQQGEwJESzESMBAGA1UECgwJVFJVU1QyNDA4MSUwIwYDVQQDDBxUUlVTVDI0MDgg
U3lzdGVtdGVzdCBYWElJIENBMQ4wDAYDVQQDDAVDUkwyNDAfBgNVHSMEGDAWgBSr
qAFEGbCzQ5na+nzM0gAYA+c8vzAdBgNVHQ4EFgQU5H+l2uWffQIiRq9EiU3NanNN
xxgwCQYDVR0TBAIwADANBgkqhkiG9w0BAQsFAAOCAQEAGtiRwf/tMCcGRBXQJ9sW
DSFnpCVPiwe0fjZoSncT5sStRa5l04eBlrQ/VwxyEqNnAbJPgKxTdCe8FK+1bGX6
+qPwJwfEs1EkJo8MTK0WXAP5vO1OYGMPEkdXcXbP6Zn5JsRPeGsldE1B8iRB77uL
yCYvm0y0KDEiq399MtD/92NtqQB57rQR1sob6zYC/d6wl5wNysxVA01uhFI/Ej56
NrU4SzVBzOwdbw1Y3kPu3YXzBZ4pYI88uAxy7HdUJYkwHrbG2KdKU+FvvrQ9D4ww
0T9csPk9smrXsSAMs0SHm0/NcHYOjyIJGRcwUeiltxzLmuN45f/OktjPNdxy7GND
Fw==";
            var mgr = new InMemoryAttributeAdapter();
            mgr.SetAttributeValues(CommonOioAttributes.UserCertificate.Name, null, null, new []{new ComplexSamlAttributeValue(base64Der)}, null);

            // Act
            var cert = mgr.GetValue(CommonOioAttributes.UserCertificate);

            // Assert
            Assert.AreEqual("CN=Thomas Ålbæk + SERIALNUMBER=CVR:34051178-RID:90897702, O=Digitaliseringsstyrelsen // CVR:34051178, C=DK", cert.Subject);

            // Act
            var mgr2 = new InMemoryAttributeAdapter();
            mgr2.SetValue(CommonOioAttributes.UserCertificate, cert);

            // Assert
            var actual = mgr2.GetAttributeValues(CommonOioAttributes.UserCertificate.Name).Single().AttributeValueElement.Value;
            Assert.AreEqual(base64Der.Replace("\r\n",""), actual);

        }
    }
}