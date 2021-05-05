using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace DK.Gov.Oio.Idws.IntegrationTests.Rest
{
    public class JavaIntegrationTests : IntegrationTestBase
    {
        private string _verificationString = "JavaIntegrationTests.";
        
        public JavaIntegrationTests() : base(Configuration.BuildJavaWspConfiguration()) { }
        
        [Fact]
        [Trait("Category","JavaREST")]
        public async Task TestSystemUserScenario()
        {
            _verificationString += "TestSystemUserScenario";
            ConfigureSystemUserScenario();
            await TestHttpClient();
        }

        [Fact]
        [Trait("Category","JavaREST")]
        public async Task TestLocalTokenScenario()
        {
            _verificationString += "TestLocalTokenScenario";
            ConfigureLocalTokenScenario();
            await TestHttpClient();
        }
        
        [Fact]
        [Trait("Category","JavaREST")]
        public async Task TestBootstrapScenario()
        {
            _verificationString += "TestBootstrapScenario";
            ConfigureBootstrapScenario();
            
            await TestHttpClient();
        }

        private async Task TestHttpClient()
        {
            var client = await CreateHttpClientWithIssuedToken();
            
            var response = await client.GetAsync($"{Configuration.RestWspConfiguration.Endpoint}?name={_verificationString}");
            var responseString = await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync();
            
            responseString.Should().StartWith($"Hello {_verificationString}");
        }
    }
}