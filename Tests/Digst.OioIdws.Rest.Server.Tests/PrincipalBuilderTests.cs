using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Digst.OioIdws.Rest.Common;
using Digst.OioIdws.Rest.Server;
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

            var sut = new IdentityBuilder();
            var identity = await sut.BuildIdentityAsync(token);
            
            Assert.IsNotNull(identity);
            Assert.IsTrue(identity.IsAuthenticated);

            Assert.AreEqual("OioIdws", identity.AuthenticationType);
            Assert.IsTrue(identity.HasClaim("nametype1", "name1"));
            Assert.AreEqual(4, identity.Claims.Count());
        }
    }
}
