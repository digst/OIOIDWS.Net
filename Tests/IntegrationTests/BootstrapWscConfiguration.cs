using System;

namespace DK.Gov.Oio.Idws.IntegrationTests
{
    public class BootstrapWscConfiguration
    {
        /// <summary>
        /// WSC Endpoint where login will happen and bootstrap token extracted from.
        /// </summary>
        public Uri WscEndpoint { get; set; }
        public string WscUsername { get; set; }
        public string WscPassword { get; set; }
    }
}
