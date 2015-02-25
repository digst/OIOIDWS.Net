using System;
using System.Runtime.CompilerServices;

namespace Digst.OioIdws.Logging
{
    /// <summary>
    /// OIOIDWS uses this interface to do logging. 
    /// Create an implemenation that fits your own logging framework to do logging.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Used for writing statements about methods being entered and exited.
        /// The log can then be used to follow a flow.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callerMemberName"></param>
        /// <param name="callerLineNumber"></param>
        /// <param name="callerFilePath"></param>
        void Trace(string message, [CallerMemberName] string callerMemberName = null, [CallerLineNumber] int callerLineNumber = 0, [CallerFilePath] string callerFilePath = null);
        /// <summary>
        /// Used for writing debug statements. E.g. messages being sent forth and back. 
        /// </summary>
        /// <param name="message"></param>
        void Debug(string message);
        /// <summary>
        /// Used for writing info statements. E.g. current configuration.
        /// </summary>
        /// <param name="message"></param>
        void Info(string message);
        /// <summary>
        /// Used for writing warning statements.
        /// </summary>
        /// <param name="message"></param>
        void Warning(string message);
        /// <summary>
        /// Used for writing error statements. This is used when errors are recoverable.
        /// </summary>
        /// <param name="message"></param>
        void Error(string message);
        /// <summary>
        /// <see cref="Error(string)"/>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        void Error(string message, Exception exception);
        /// <summary>
        /// Used for writing error statements. This is used when errors are not recoverable. E.g. when configuration is wrong or unexpected things happen. Fatal messages are also thrown as exceptions.
        /// </summary>
        /// <param name="message"></param>
        void Fatal(string message);
        /// <summary>
        /// <see cref="Fatal(string)"/>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        void Fatal(string message, Exception exception);
    }
}
