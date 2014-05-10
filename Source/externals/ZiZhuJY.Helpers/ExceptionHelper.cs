using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZiZhuJY.Helpers
{
    /// <summary>
    /// Helper to handle exceptions.
    /// </summary>
    public class ExceptionHelper
    {
        /// <summary>
        /// Centrals process the exceptions. Log these exceptions' information.
        /// </summary>
        /// <param name="exceptions">The exceptions.</param>
        /// <returns>The exceptions information text.</returns>
        public static string CentralProcess(params Exception[] exceptions)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Exception ex in exceptions)
            {
                sb.Append(CentralProcessSingle(ex));
            }
            return sb.ToString();
        }

        public static string CentralProcess(List<Exception> exceptions)
        {
            if (exceptions == null || exceptions.Count <= 0) return "";

            return CentralProcess(exceptions.ToArray());
        }

        /// <summary>
        /// Centrals process a single exception object.
        /// </summary>
        /// <param name="ex">The exception object.</param>
        /// <returns>The exception information text.</returns>
        public static string CentralProcessSingle(Exception ex)
        {
            string log = ExceptionLog(ex);
            Log.Error(log);
            return log;
        }

        /// <summary>
        /// Centrals process the exceptions. Log these exceptions' information. The difference from the CentralProcess method is the return value.
        /// </summary>
        /// <param name="exceptions">The exceptions.</param>
        /// <returns>The exceptions that this method handled.</returns>
        public static Exception[] CentralProcess2(params Exception[] exceptions)
        {
            foreach (Exception ex in exceptions)
            {
                CentralProcessSingle(ex);
            }
            return exceptions;
        }

        /// <summary>
        /// Centrals process a single exception object. The different from the CentralProcessSingle() method is its return value.
        /// </summary>
        /// <param name="ex">The exception object.</param>
        /// <returns>The exception object that this method handled.</returns>
        public static Exception CentralProcessSingle2(Exception ex)
        {
            CentralProcessSingle(ex);
            return ex;
        }

        /// <summary>
        /// Get the log text of a single exception object
        /// </summary>
        /// <param name="ex">The exception object.</param>
        /// <returns>The log text of the exception object.</returns>
        public static string ExceptionLogSingle(Exception ex)
        {
            if (ex == null) return "";

            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendLine(string.Format("Error occurred: {0}\r\nSource: {1}\r\nType:{2}\r\n{3}\r\nData:{4}\r\nHelpLink:{5}", ex.Message, ex.Source, ex.GetType().ToString(), ex.StackTrace, ex.Data, ex.HelpLink));
                Exception innerException = ex.InnerException;
                while (innerException != null)
                {
                    sb.AppendLine(string.Format("Inner Exception: {0}\r\nSource: {1}\r\nType:{2}\r\n{3}\r\nData:{4}\r\nHelpLink:{5}", innerException.Message, innerException.Source, innerException.GetType().ToString(), innerException.StackTrace, innerException.Data, innerException.HelpLink));
                    innerException = innerException.InnerException;
                }
            }
            finally
            {
            }

            return sb.ToString();
        }

        /// <summary>
        /// Log the exceptions.
        /// </summary>
        /// <param name="exceptions">The exceptions.</param>
        /// <returns>The log text for the exception objects.</returns>
        public static string ExceptionLog(params Exception[] exceptions)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Exception ex in exceptions)
            {
                sb.Append(ExceptionLogSingle(ex));
            }
            return sb.ToString();
        }
    }
}
