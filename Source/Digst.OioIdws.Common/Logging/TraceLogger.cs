﻿using System;
using System.Diagnostics;

namespace Digst.OioIdws.Common.Logging
{
    class TraceLogger : ILogger
    {
        /// <summary>
        /// The source to use for logging
        /// </summary>
        private readonly static TraceSource Source;

        static TraceLogger()
        {
            Source = new TraceSource("Digst.OioIdws");
        }

        public void Trace(string message, string callerMemberName = null, int callerLineNumber = 0, string callerFilePath = null)
        {
            if (Source.Switch.ShouldTrace(TraceEventType.Verbose))
            {
                Source.TraceEvent(TraceEventType.Verbose, 0, string.Format("CallerMemberName: {0}, CallerLineNumber: {1}, CallerFilePath: {2}, Message: {3}",
                callerMemberName, callerLineNumber, callerFilePath, message));
            }
        }

        public void Debug(string message)
        {
            if (Source.Switch.ShouldTrace(TraceEventType.Verbose))
            {
                Source.TraceEvent(TraceEventType.Verbose, 0, message);
            }
        }

        public void Info(string message)
        {
            if (Source.Switch.ShouldTrace(TraceEventType.Information))
            {
                Source.TraceEvent(TraceEventType.Information, 0, message);
            }
        }

        public void Warning(string message)
        {
            if (Source.Switch.ShouldTrace(TraceEventType.Warning))
            {
                Source.TraceEvent(TraceEventType.Warning, 0, message);
            }
        }

        public void Error(string message)
        {
            if (Source.Switch.ShouldTrace(TraceEventType.Error))
            {
                Source.TraceEvent(TraceEventType.Error, 0, message);
            }
        }

        public void Error(string message, Exception exception)
        {
            if (Source.Switch.ShouldTrace(TraceEventType.Error))
            {
                Source.TraceEvent(TraceEventType.Error, 0, message + " - " + exception.Message);
            }
        }

        public void Fatal(string message)
        {
            if (Source.Switch.ShouldTrace(TraceEventType.Critical))
            {
                Source.TraceEvent(TraceEventType.Critical, 0, message);
            }
        }

        public void Fatal(string message, Exception exception)
        {
            if (Source.Switch.ShouldTrace(TraceEventType.Critical))
            {
                Source.TraceEvent(TraceEventType.Critical, 0, message + " - " + exception.Message);
            }
        }
    }
}
