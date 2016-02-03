using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using Microsoft.Owin.Logging;

namespace Digst.OioIdws.Rest.Examples.ServerCombined
{
    internal class ConsoleLoggerFactory : ILoggerFactory
    {
        class Logger : ILogger
        {
            private readonly string _name;

            public Logger(string name)
            {
                _name = name.Split('.').LastOrDefault();
            }

            public bool WriteCore(TraceEventType eventType, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
            {
                Console.WriteLine($"{_name} (requestid:{CallContext.LogicalGetData("correlationIdentifier")}, source info:{CallContext.LogicalGetData("sourceInfo")}): {eventType} - {formatter(state, exception)}");
                return true;
            }
        }
        public ILogger Create(string name)
        {
            return new Logger(name);
        }
    }
}