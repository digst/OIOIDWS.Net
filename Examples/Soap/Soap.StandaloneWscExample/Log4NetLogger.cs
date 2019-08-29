using System;
using System.Diagnostics;
using System.Threading.Tasks;
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
                Trace.CorrelationManager.ActivityId = Guid.NewGuid();
            }
        }

        public void WriteCore(TraceEventType eventType, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
        {
            switch (eventType)
            {
                case TraceEventType.Critical:
                    Logger.Fatal(state, exception);
                    break;
                case TraceEventType.Error:
                    Logger.Error(state, exception);
                    break;
                case TraceEventType.Warning:
                    Logger.Warn(state, exception);
                    break;
                case TraceEventType.Verbose:
                    Logger.Debug(state, exception);
                    break;
                default:
                    Logger.Info(state, exception);
                    break;
            }
        }
    }
}
