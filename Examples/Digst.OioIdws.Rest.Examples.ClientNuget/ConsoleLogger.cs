using System;
using System.Diagnostics;
using Digst.OioIdws.Common.Logging;

namespace Digst.OioIdws.Rest.Examples.ClientNuget
{
    internal class ConsoleLogger : ILogger
    {
        public void WriteCore(TraceEventType eventType, int eventId, object state, Exception exception,
            Func<object, Exception, string> formatter)
        {
            Console.WriteLine($"{eventType}: {formatter(state, exception)}");
        }
    }
}