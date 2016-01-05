using System;
using System.Diagnostics;

namespace Digst.OioIdws.Common.Logging
{
    /// <summary>
    /// OIOIDWS uses this interface to do logging. 
    /// Create an implemenation that fits your own logging framework to do logging.
    /// </summary>
    public interface ILogger
    {
        void WriteCore(TraceEventType eventType, int eventId, object state, Exception exception, Func<object, Exception, string> formatter);
    }
}
