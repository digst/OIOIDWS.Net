using System;
using Digst.OioIdws.Common.Logging;
using log4net;

namespace Digst.OioIdws.WscExample
{
    public class Log4NetLogger : ILogger
    {
        private static readonly ILog Logger = LogManager.GetLogger("OioIdws");

        static Log4NetLogger()
        {
            // Set Correlation ID if it has not already been set by the using system
            if (System.Diagnostics.Trace.CorrelationManager.ActivityId == Guid.Empty)
            {
                System.Diagnostics.Trace.CorrelationManager.ActivityId = Guid.NewGuid();
            }
        }

        public void Trace(string message, string callerMemberName = null, int callerLineNumber = 0, string callerFilePath = null)
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug(
                    string.Format("CallerMemberName: {0}, CallerLineNumber: {1}, CallerFilePath: {2}, Message: {3}",
                        callerMemberName, callerLineNumber, callerFilePath, message));
            }
        }

        public void Debug(string message)
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug(message);
            }
        }

        public void Info(string message)
        {
            if (Logger.IsInfoEnabled)
            {
                Logger.Info(message);
            }
        }

        public void Warning(string message)
        {
            if (Logger.IsWarnEnabled)
            {
                Logger.Warn(message);
            }
        }

        public void Error(string message)
        {
            if (Logger.IsErrorEnabled)
            {
                Logger.Error(message);
            }
        }

        public void Error(string message, Exception exception)
        {
            if (Logger.IsErrorEnabled)
            {
                Logger.Error(message, exception);
            }
        }

        public void Fatal(string message)
        {
            if (Logger.IsFatalEnabled)
            {
                Logger.Fatal(message);
            }
        }

        public void Fatal(string message, Exception exception)
        {
            if (Logger.IsFatalEnabled)
            {
                Logger.Fatal(message, exception);
            }
        }
    }
}
