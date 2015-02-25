using System;
using System.Runtime.CompilerServices;
using Digst.OioIdws.Configurations;

namespace Digst.OioIdws.Logging
{
    /// <summary>
    ///  Singleton logger that default uses the .Net logging framework or a custom framework if such has been implemented.
    /// </summary>
    public sealed class Logger : ILogger
    {
        private static readonly Lazy<ILogger> LazyLogger =
            new Lazy<ILogger>(() => new Logger());

        public static ILogger Instance { get { return LazyLogger.Value; } }

        private readonly ILogger _logger;

        private Logger()
        {
            // Retrieve Configuration
            var config =
                (OioIdwsConfiguration)System.Configuration.ConfigurationManager.GetSection("oioIdwsConfiguration");
            if (config != null && !string.IsNullOrEmpty(config.Logger))
            {
                try
                {
                    var t = Type.GetType(config.Logger);
                    if (t != null)
                    {
                        _logger = (ILogger)Activator.CreateInstance(t);
                    }
                    else
                    {
                        throw new Exception(
                            string.Format(
                                "The type {0} is not available for the audit logging. Please check the type name and assembly",
                                config.Logger));
                    }
                }
                catch (Exception e)
                {
                    new TraceLogger().Fatal("Could not instantiate the configured auditLogger. Message: " + e.Message);
                    throw;
                }
            }
            else
            {
                _logger = new TraceLogger();
            }
        }

        public void Trace(string message, [CallerMemberName] string callerMemberName = null, [CallerLineNumber] int callerLineNumber = 0, [CallerFilePath] string callerFilePath = null)
        {
            _logger.Trace(message, callerMemberName, callerLineNumber, callerFilePath);
        }

        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        public void Info(string message)
        {
            _logger.Info(message);
        }

        public void Warning(string message)
        {
            _logger.Warning(message);
        }

        public void Error(string message)
        {
            _logger.Error(message);
        }

        public void Error(string message, Exception exception)
        {
            _logger.Error(message, exception);
        }

        public void Fatal(string message)
        {
            _logger.Fatal(message);
        }

        public void Fatal(string message, Exception exception)
        {
            _logger.Fatal(message, exception);
        }
    }
}