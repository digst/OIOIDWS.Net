using System;
using System.Security.Cryptography.X509Certificates;
using Digst.OioIdws.Common.Utils;
using Digst.OioIdws.Healthcare.Sts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Digst.OioIdws.SecurityTokenService.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void CanIssueServiceToken()
        {
            X509Certificate2 tokenSigningCertificate = new LocalMachineCertificateStoreFactory().GetCertificateStore().GetByThumbprint("aaa");

            var factory = new HealthcareSecurityTokenFactory(tokenSigningCertificate, "teststs.tempuri.org", null);

        }
    }
}
