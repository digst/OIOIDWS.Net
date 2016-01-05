using System;
using System.Diagnostics;

namespace Digst.OioIdws.Common.Logging
{
    /// <summary>
    ///  Singleton logger that default uses the .Net logging framework or a custom framework if such has been implemented.
    /// </summary>
    public sealed class Logger : ILogger
    {
        private static readonly Lazy<ILogger> LazyLogger =
            new Lazy<ILogger>(() => new Logger());

        public static ILogger Instance => LazyLogger.Value;

        private readonly ILogger _logger;

        private Logger()
        {
            _logger = LoggerFactory.CreateLogger();
        }

        public void WriteCore(TraceEventType eventType, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
        {
            _logger.WriteCore(eventType, eventId, state, exception, formatter);
        }
    }
}