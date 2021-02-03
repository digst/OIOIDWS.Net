using System.IdentityModel.Tokens;
using Digst.OioIdws.OioWsTrust;
using System;
using System.IO;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Xml;
using System.Net;

namespace DK.Gov.Oio.Idws.IntegrationTests.TokenAcquisition
{
    public class BootstrapScenario : ITokenAcquisitionScenario
    {
        private readonly BootstrapWscConfiguration _bootstrapWscConfiguration;
        private readonly IStsTokenService _tokenService;

        public BootstrapScenario(BootstrapWscConfiguration bootstrapWscConfiguration, IStsTokenService tokenService)
        {
            _bootstrapWscConfiguration = bootstrapWscConfiguration;
            _tokenService = tokenService;
        }

        public SecurityToken AcquireTokenFromSts()
        {
            var bootstrapToken = AcquireBootstrapToken();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            return _tokenService.GetTokenWithBootstrapToken(bootstrapToken);
        }

        private SecurityToken AcquireBootstrapToken()
        {
            // Retrieve token from WscBootstrapExample using Selenium/Chrome.
            var bootstrapTokenBase64 = GetBase64EncodedBootstrapTokenFromWscBootstrapExample();

            // Decode base64 encoded bootstrap token.
            var rawBootstrapToken = Convert.FromBase64String(bootstrapTokenBase64);

            // Read byte array and deserialize token to SecurityToken.
            using (var memoryStream = new MemoryStream(rawBootstrapToken))
            using (var streamReader = new StreamReader(memoryStream))
            using (var xmlTextReader = new XmlTextReader(streamReader))
            {
                SecurityTokenHandler handler = new Saml2SecurityTokenHandler();
                handler.Configuration = new SecurityTokenHandlerConfiguration();
                return handler.ReadToken(xmlTextReader);
            }
        }

        private string GetBase64EncodedBootstrapTokenFromWscBootstrapExample()
        {
            //Start browser instance (Chrome with extensions enabled)
            //NemID Nøglefilsprogram
            var pathToExtension =
                "C:\\Users\\Developer\\AppData\\Local\\Google\\Chrome\\User Data\\Default\\Extensions\\mbjoejbgakiicfllhcdilppjkmmicnch\\1.41_0";

            var options = new ChromeOptions();
            options.AddArguments("load-extension=" + pathToExtension);

            var driver = new ChromeDriver(options);
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            //Navigate to login page
            driver.Navigate().GoToUrl(_bootstrapWscConfiguration.WscEndpoint);
            driver.FindElement(By.LinkText("Go to My Page.")).Click();

            // Jeg har deaktiveret denne midlertidigt for at få login til at virke lokalt. --thjak

            //Select Idp, using IntTest at the moment...
            // //TODO: I'm guessing this step is not necessary once we only point to DevTest4
            // Thread.Sleep(5000);
            // wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(
            //     By.LinkText("https://saml.test-nemlog-in.dk/"))).Click();
            //
            // //Login - TEMPORARY SOLUTION WHILE WAITING FOR MIT ID LOGIN OPTION
            // wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(
            //     By.CssSelector("#Repeater2_LoginMenuItem_1 > span:nth-child(2)"))).Click();
            // Thread.Sleep(3000);
            // driver.SwitchTo().Frame(0);
            // driver.FindElement(By.Id("ok")).Click();
            //ENTER PASSWORD MANUALLY (unable to access password pop-up)
            Thread.Sleep(20000);

            var bootstrapTokenBase64 = wait.Until(d =>
                d.FindElement(By.XPath("//*[@id='aspnetForm']/div[4]/div[1]/table/tbody/tr[14]/td[2]"))).Text;

            driver.Quit();

            return bootstrapTokenBase64;
        }
    }
}