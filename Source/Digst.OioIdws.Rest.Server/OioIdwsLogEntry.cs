using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Digst.OioIdws.Rest.Server
{
    public class OioIdwsLogEntry
    {
        public IDictionary<string, object> Variables { get; }
        public Exception Exception { get; private set; }
        public TraceEventType Level { get; private set; }
        public int EventId { get; set; }

        public OioIdwsLogEntry(TraceEventType level, int eventId, string messageTemplate)
        {
            Variables = new Dictionary<string, object>
            {
                {"messageTemplate", messageTemplate}
            };
            Level = level;
            EventId = eventId;
        }

        public OioIdwsLogEntry Property(string key, object value)
        {
            Variables[key] = value;
            return this;
        }

        public string Message()
        {
            var template = (string) Variables["messageTemplate"];
            return Variables.Aggregate(template, (current, pair) => current.Replace("{" + pair.Key + "}", pair.Value.ToString()));
        }

        public override string ToString()
        {
            return Message();
        }

        public OioIdwsLogEntry ExceptionOccured(Exception exception)
        {
            Exception = exception;
            return this;
        }
    }
}
