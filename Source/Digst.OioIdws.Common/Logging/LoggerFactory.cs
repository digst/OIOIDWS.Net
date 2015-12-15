using System;

namespace Digst.OioIdws.Common.Logging
{
    public static class LoggerFactory
    {
        private static ILogger _customLogger;

        internal static ILogger CreateLogger()
        {
            if (_customLogger != null)
            {
                return _customLogger;
            }

            // Retrieve Configuration
            var config = (Configuration)System.Configuration.ConfigurationManager.GetSection("oioIdwsLoggingConfiguration");

            if (!string.IsNullOrEmpty(config?.Logger))
            {
                try
                {
                    var t = Type.GetType(config.Logger);
                    if (t != null)
                    {
                        return (ILogger)Activator.CreateInstance(t);
                    }

                    throw new Exception($"The type {config.Logger} is not available for the logging. Please check the type name and assembly");
                }
                catch (Exception e)
                {
                    new TraceLogger().Fatal("Could not instantiate the configured logger. Message: " + e.Message);
                    throw;
                }
            }

            return new TraceLogger();
        }

        public static void SetLogger(ILogger logger)
        {
            _customLogger = logger;
        }
    }
}
