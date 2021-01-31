using System;
using System.ServiceModel;
using DK.Gov.Oio.Idws.IntegrationTests.DotNetHelloWorldProxy;
using FluentAssertions;
using Xunit;

namespace DK.Gov.Oio.Idws.IntegrationTests.Soap
{
    public class DotNetIntegrationTests : IntegrationTestBase
    {
        private const string HelloNoneFormat = "Hello None {0}. Your claims are:\n";
        private const string HelloSignedFormat = "Hello Sign {0}. Your claims are:\n";
        private const string HelloEncryptedAndSignedFormat = "Hello EncryptAndSign {0}. Your claims are:\n";
        
        private readonly ChannelFactory<IHelloWorld> _wspChannelFactory;

        private string _channelInput = "DotNetIntegrationTests.";
        private IHelloWorld _wspChannel;
        
        public DotNetIntegrationTests()
        {
            var dotNetWspConfiguration = Configuration.BuildDotNetWspConfiguration();
            ConfigureWscAndSts(dotNetWspConfiguration.StsConfiguration);
            _wspChannelFactory = WspConfigurationFactory.CreateChannelFactory<IHelloWorld>(dotNetWspConfiguration.WspConfiguration);
        }
        
        [Fact]
        public void TestSystemUserScenario()
        {
            _channelInput += "TestSystemUserScenario";
            ConfigureSystemUserScenario();
            
            TestChannel();
        }

        [Fact(Skip = "Not implemented yet")]
        public void TestLocalTokenScenario()
        {
            _channelInput += "TestLocalTokenScenario";
            ConfigureLocalTokenScenario();
            
            TestChannel();
        }
        
        [Fact(Skip = "Not implemented yet")]
        public void TestBootstrapScenario()
        {
            _channelInput += "TestBootstrapScenario";
            ConfigureBootstrapScenario();
            
            TestChannel();
        }

        private void TestChannel()
        {
            _wspChannel = CreateChannelWithIssuedToken(_wspChannelFactory);
            
            // Positive cases
            TestChannelMethod(_wspChannel.HelloNone, HelloNoneFormat);
            TestChannelMethod(_wspChannel.HelloSign, HelloSignedFormat);
            TestChannelMethod(_wspChannel.HelloEncryptAndSign, HelloEncryptedAndSignedFormat);
            
            // Negative cases
            _wspChannel.Invoking(c => c.HelloSignError(_channelInput)).Should().Throw<Exception>();
            _wspChannel.Invoking(c => c.HelloSignErrorNotEncrypted(_channelInput)).Should().Throw<Exception>();
        }

        private void TestChannelMethod(Func<string, string> channelMethod, string formatString)
        {
            var expectedResponse = string.Format(formatString, _channelInput);
            var actualResponse = channelMethod.Invoke(_channelInput);
            actualResponse.Should().StartWith(expectedResponse);
        }
    }
}