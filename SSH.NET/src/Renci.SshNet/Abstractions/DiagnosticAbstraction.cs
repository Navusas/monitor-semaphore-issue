using System;
using System.Diagnostics;
#if FEATURE_DIAGNOSTICS_TRACESOURCE
using System.Threading;
#endif // FEATURE_DIAGNOSTICS_TRACESOURCE

namespace Renci.SshNet.Abstractions
{
    /// <summary>
    /// /
    /// </summary>
    public static class DiagnosticAbstraction
    {
#if FEATURE_DIAGNOSTICS_TRACESOURCE

        private static readonly SourceSwitch SourceSwitch = new SourceSwitch("SshNetSwitch");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="traceEventType"></param>
        /// <returns></returns>
        public static bool IsEnabled(TraceEventType traceEventType)
        {
            return SourceSwitch.ShouldTrace(traceEventType);
        }

        private static readonly TraceSource Loggging =
#if DEBUG
            new TraceSource("SshNet.Logging", SourceLevels.All);
#else
            new TraceSource("SshNet.Logging");
#endif // DEBUG
#endif // FEATURE_DIAGNOSTICS_TRACESOURCE

        /// <summary>
        /// /
        /// </summary>
        /// <param name="text"></param>
        public static void Log(string text)
        {
            Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId}: {text}");
        }
    }
}
