using System;
using System.Collections.Generic;
using System.IdentityModel.Claims;
using System.IdentityModel.Protocols.WSTrust;
using System.Linq;
using Digst.OioIdws.Healthcare.Common;
using Digst.OioIdws.Healthcare.SamlAttributes;
using Digst.OioIdws.SamlAttributes.AttributeAdapters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Digst.OioIdws.SamlAttributes.Healthcare.Test
{
    [TestClass]
    public class ClaimsCollectionTests
    {

        public TestContext TestContext { get; set; }

        [TestMethod]
        public void TestMethod1()
        {

            // Example claims collection, initially empty. Outside tests you may not need to create
            // this collection yourself. Rather, you should be able to use the claims collection of
            // a request
            var claims = new RequestClaimCollection();

            // Adapter through which claims can be added strongly typed with correct encoding as
            // per the marshaller. 
            var claimsAdapter = new RequestClaimCollectionAttributeAdapter(claims, true);

            // Example value 
            var subjectRoles = new[]{
                new SubjectRole("7170", "1.2.208.176.1.3", "Autorisationsregister", "Læge"),
                new SubjectRole("5433","1.2.208.176.1.3","Autorisationsregister","Tandlæge"),
            };

            // Set the claim value through the adapter. This will set the claims in the underlying
            // (adapted) claims collection. Note how this is strongly typed, as the type of the
            // 2nd argument is inferred from the type of the first argument (the marshaller).
            // The SubjectRole marshaller specifies that the value must be of
            // IEnumerable<SubjectRole> type, which includes an array of SubjectRole.
            claimsAdapter.SetValue(CommonHealthcareAttributes.SubjectRole, subjectRoles);

            // Assert that the claim has been set correctly in the underlying claims collection.
            // As we set a multi-valued claim, the claims collection should contain two claims
            // with the same type but different values.
            Assert.AreEqual(2, claims.Count);


            var claim0 = claims[0].Value.Trim();
            var claim1 = claims[1].Value.Trim();

            TestContext.WriteLine(claim0);
            TestContext.WriteLine(claim1);

            var expected0 = @"<Role xsi:type=""CE"" code=""7170"" codeSystem=""1.2.208.176.1.3"" codeSystemName=""Autorisationsregister"" displayName=""Læge"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns=""urn:hl7-org:v3"" />";
            var expected1 = @"<Role xsi:type=""CE"" code=""5433"" codeSystem=""1.2.208.176.1.3"" codeSystemName=""Autorisationsregister"" displayName=""Tandlæge"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns=""urn:hl7-org:v3"" />";

            //for (int i = 0; i < expected0.Length; i++)
            //{
            //}

            //for (int i = 0; i < expected1.Length; i++)
            //{
            //    Assert.AreEqual(i.ToString()+expected1[i], i.ToString()+claim1[i]);
            //}

            Assert.AreEqual(expected0, claim0);
            Assert.AreEqual(expected1, claim1);

        }
    }
}

