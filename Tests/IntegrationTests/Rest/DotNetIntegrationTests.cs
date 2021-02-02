﻿using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace DK.Gov.Oio.Idws.IntegrationTests.Rest
{
    public class DotNetIntegrationTests : IntegrationTestBase
    {
        private string _requestUri = "DotNetIntegrationTests.";
        
        public DotNetIntegrationTests() : base(Configuration.BuildDotNetWspConfiguration()) { }
        
        [Fact]
        public async Task TestSystemUserScenario()
        {
            _requestUri += "TestSystemUserScenario";
            ConfigureSystemUserScenario();
            await TestHttpClient();
        }

        [Fact]
        public async Task TestLocalTokenScenario()
        {
            _requestUri += "TestLocalTokenScenario";
            ConfigureLocalTokenScenario();
            await TestHttpClient();
        }

        private async Task TestHttpClient()
        {
            var client = await CreateHttpClientWithIssuedToken();
            var response = await client.GetAsync(_requestUri);
            var responseString = await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync();
            
            responseString.Should().StartWith(
                $"Requested at {Configuration.RestWspConfiguration.Endpoint}{_requestUri}\nAuthenticationType: OioIdws\r\nClaims:");
        }
    }
}