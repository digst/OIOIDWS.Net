using System;
using System.Diagnostics;
using Microsoft.Owin.Logging;

namespace Digst.OioIdws.Test.Common
{
    public class OwinConsoleLoggerFactory : ILoggerFactory
    {
        class TestLogger : ILogger
        {
            private readonly string _name;

            public TestLogger(string name)
            {
                _name = name;
            }

            public bool WriteCore(TraceEventType eventType, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
            {
                var t = formatter(state, exception);
                Console.WriteLine($"{_name} - {eventType} - {eventId} - {state} - {exception}");
                return true;
            }
        }

        public ILogger Create(string name)
        {
            return new TestLogger(name);
        }
    }
}
