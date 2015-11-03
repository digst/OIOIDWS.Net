using System;
using System.Configuration;

namespace Digst.OioIdws.Common.Logging
{
    public class Configuration : ConfigurationSection
    {
        /// <summary>
        /// Specifies a <see cref="Type.AssemblyQualifiedName"/> of a custom implementation of the <see cref="ILogger"/> interface. <see cref="TraceLogger"/> is used if no custom logger has been specified.
        /// </summary>
        [ConfigurationProperty("logger", IsRequired = false)]
        public string Logger
        {
            get { return (string) this["logger"]; }
            set { this["logger"] = value; }
        }
    }
}
