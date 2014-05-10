using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace ZiZhuJY.Helpers
{
    /// <summary>
    /// Helper to handle write the log text of this program.
    /// </summary>
    public class Log
    {
        private static SourceSwitch sourceSwitch = new SourceSwitch("Test Tracing Switch", "Information");

        /// <summary>
        /// Write the specified message as information level.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments.</param>
        public static void Info(string message, params object[] args)
        {
            Log.WriteLine(TraceEventType.Information, message, args);
        }

        /// <summary>
        /// Write the specified message as error level.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments.</param>
        public static void Error(string message, params object[] args)
        {
            Log.WriteLine(TraceEventType.Error, message, args);
        }

        /// <summary>
        /// Write message followed by a new line to the Trace object with a specified format.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        private static void WriteLine(TraceEventType eventType, string format, params object[] args)
        {
            if (sourceSwitch.ShouldTrace(eventType))
            {
                if (args.Length != 0)
                {
                    format = String.Format(format, args);
                }

                format = DateTime.Now.ToString() + ": " + format;
                Trace.WriteLine(format, eventType.ToString());
            }
        }

        /// <summary>
        /// Starts the trace listners.
        /// </summary>
        public static void StartTraceListners(string logFilePrefix = "RulePerf")
        {
            string logFilePath = "{1}-{0}.log".FormatWith(DateTime.Now.ToString("yyyy-MM-ddThh-mm-ssZ"), logFilePrefix);
            logFilePath = System.IO.Path.Combine(System.Reflection.Assembly.GetExecutingAssembly().Location, logFilePath);
            TextWriterTraceListener textWriterTraceListener = new TextWriterTraceListener(logFilePath, "logListener");
            ConsoleTraceListener consoleTraceListener = new ConsoleTraceListener();

            ////Trace.Listeners.Clear();

            if (!Trace.Listeners.Contains(textWriterTraceListener.GetType(), textWriterTraceListener.Name))
            {
                Trace.Listeners.Add(textWriterTraceListener);
            }
            else
            {
                textWriterTraceListener.Dispose();
            }

            if (!Trace.Listeners.Contains(consoleTraceListener.GetType()))
            {
                Trace.Listeners.Add(consoleTraceListener);
            }
            else
            {
                consoleTraceListener.Dispose();
            }

            Trace.AutoFlush = true;
        }
    }
}
