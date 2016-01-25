using System;
using System.Diagnostics;
using System.Globalization;
using Microsoft.Owin.Logging;

namespace Digst.OioIdws.Rest.Server
{
    public static class LoggerExtensions
    {
        private static readonly Func<object, Exception, string> TheMessage = (message, error) => (string)message;
        private static readonly Func<object, Exception, string> TheMessageAndError = (message, error) => string.Format(CultureInfo.CurrentCulture, "{0}\r\n{1}", message, error);

        public static void WriteEntry(this ILogger logger, OioIdwsLogEntry entry)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
            if (entry == null)
            {
                throw new ArgumentNullException(nameof(entry));
            }

            Func<object, Exception, string> formatter;

            if (entry.Exception != null)
            {
                formatter = (state, exc) => TheMessageAndError(entry.Message(), exc);
            }
            else
            {
                formatter = (state, exc) => TheMessage(entry.Message(), null);
            }

            logger.WriteCore(entry.Level, entry.EventId, entry.Variables, entry.Exception, formatter);
        }
    }
}
