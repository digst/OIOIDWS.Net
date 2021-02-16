using System.ServiceModel;
using DK.Gov.Oio.Idws.IntegrationTests.JavaHelloWorldProxy;
using FluentAssertions;
using Xunit;

namespace DK.Gov.Oio.Idws.IntegrationTests.Soap
{
    public class JavaIntegrationTests : IntegrationTestBase
    {
        private readonly ChannelFactory<HelloWorldPortType> _wspChannelFactory;
        private string _channelInput = "JavaIntegrationTests.";
        
        public JavaIntegrationTests() : base(Configuration.BuildJavaWspConfiguration())
        {
            _wspChannelFactory = WspConfigurationFactory.CreateChannelFactory<HelloWorldPortType>(
                Configuration.SoapWspConfiguration);
        }
        
        [Fact]
        public void TestSystemUserScenario()
        {
            _channelInput += "TestSystemUserScenario";
            ConfigureSystemUserScenario();
            
            TestChannel();
        }

        [Fact]
        public void TestLocalTokenScenario()
        {
            _channelInput += "TestLocalTokenScenario";
            ConfigureLocalTokenScenario();
            
            TestChannel();
        }
        
        [Fact(Skip = "Enable test once STS is running in DevTest4")]
        public void TestBootstrapScenario()
        {
            _channelInput += "TestBootstrapScenario";
            ConfigureBootstrapScenario();
            
            TestChannel();
        }

        private void TestChannel()
        {
            var channel = CreateChannelWithIssuedToken(_wspChannelFactory);
            var response = channel.HelloWorld(new HelloWorldRequest(_channelInput));
            response.response.Should().Be($"Hello {_channelInput}");
        }
    }
}