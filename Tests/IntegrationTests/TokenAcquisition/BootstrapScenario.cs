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
            var options = new ChromeOptions();
            //options.AddArguments("headless");

            var driver = new ChromeDriver(options);

            var bootstrapTokenBase64 = "";

            try
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                
                //Navigate to login page
                driver.Navigate().GoToUrl(_bootstrapWscConfiguration.WscEndpoint);
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(
                    By.CssSelector("#Repeater2_LoginMenuItem_2 > span:nth-child(2)"))).Click();
                
                //Log in
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(
                    By.Id("ContentPlaceHolder_MitIdSimulatorControl_txtUsername")))
                    .SendKeys(_bootstrapWscConfiguration.WscUsername);
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(
                        By.Id("ContentPlaceHolder_MitIdSimulatorControl_txtPassword")))
                    .SendKeys(_bootstrapWscConfiguration.WscPassword);
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(
                    By.Id("ContentPlaceHolder_MitIdSimulatorControl_btnSubmit"))).Click();
                
                //Get bootstrap token
                bootstrapTokenBase64 = wait.Until(d =>
                    d.FindElement(By.XPath("//*[@id='aspnetForm']/div[4]/div[1]/table/tbody/tr[14]/td[2]"))).Text;
            }
            finally
            {
                driver.Quit();
            }

            return bootstrapTokenBase64;
        }
    }
}