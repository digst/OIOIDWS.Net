using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Digst.OioIdws.Rest.Common;
using Digst.OioIdws.Test.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Digst.OioIdws.Rest.ProviderAuthentication.Tests
{
    [TestClass]
    public class PrincipalBuilderTests
    {
        [TestMethod]
        [TestCategory(Constants.UnitTest)]
        public async Task BuildFromSamlToken()
        {
            var token = new OioIdwsToken
            {
                AuthenticationType = "authtype1",
                NameType = "nametype1",
                RoleType = "roletype1",
                Type = AccessTokenType.Bearer,
                Claims = new[]
                {
                    new OioIdwsClaim
                    {
                        Type = "nametype1",
                        Value = "name1",
                    },
                    new OioIdwsClaim
                    {
                        Type = "roletype1",
                        Value = "role1",
                    },
                    new OioIdwsClaim
                    {
                        Type = "type1",
                        Value = "value1",
                        ValueType = "valutype1",
                        Issuer = "issuer1",
                    },
                    new OioIdwsClaim
                    {
                        Type = "type2",
                        Value = "value2",
                        ValueType = "valutype2",
                        Issuer = "issuer2",
                    }
                },
            };

            var sut = new PrincipalBuilder();
            var principal = await sut.BuildPrincipalAsync(token);
            
            Assert.IsNotNull(principal);
            Assert.IsTrue(principal.Identity.IsAuthenticated);

            var claimsIdentity = (ClaimsIdentity) principal.Identity;

            Assert.AreEqual(token.AuthenticationType, claimsIdentity.AuthenticationType);
            Assert.AreEqual("name1", claimsIdentity.Name);
            Assert.AreEqual(4, claimsIdentity.Claims.Count());
        }
    }
}
