using System;
using System.Diagnostics;
using System.Globalization;

namespace Digst.OioIdws.Common.Logging
{
    public static class LoggerExtensions
    {
        private static readonly Func<object, Exception, string> TheMessage = (message, error) => (string)message;
        private static readonly Func<object, Exception, string> TheMessageAndError = (message, error) => string.Format(CultureInfo.CurrentCulture, "{0}\r\n{1}", message, error);

        public static void Fatal(this ILogger logger, string message)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            logger.WriteCore(TraceEventType.Critical, 0, message, null, TheMessage);
        }

        public static void Trace(this ILogger logger, string message)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            logger.WriteCore(TraceEventType.Verbose, 0, message, null, TheMessage);
        }

        public static void Warning(this ILogger logger, string message)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            logger.WriteCore(TraceEventType.Warning, 0, message, null, TheMessage);
        }

        public static void Error(this ILogger logger, string message)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            logger.WriteCore(TraceEventType.Error, 0, message, null, TheMessage);
        }

        public static void Error(this ILogger logger, string message, Exception error)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            logger.WriteCore(TraceEventType.Error, 0, message, error, TheMessageAndError);
        }
    }
}
