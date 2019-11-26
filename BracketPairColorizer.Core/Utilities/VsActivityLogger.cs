using BracketPairColorizer.Core.Contracts;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Composition;

namespace BracketPairColorizer.Core.Utilities
{
    [Export(typeof(ILogger))]
    public class VsActivityLogger : ILogger
    {
        private readonly IVsActivityLog activityLog;
        private readonly IBpcTelemetry telemetry;

        [ImportingConstructor]
        public VsActivityLogger(SVsServiceProvider serviceProvider, IBpcTelemetry telemetry)
        {
            this.telemetry = telemetry;
            this.activityLog = serviceProvider.GetService(typeof(SVsActivityLog)) as IVsActivityLog;
        }

        public void LogError(string message, Exception ex)
        {
            var log = this.activityLog;
            if (log != null)
            {
                log.LogEntry((uint)__ACTIVITYLOG_ENTRYTYPE.ALE_ERROR, "BracketPairColorizer", string.Format("{0}. Exception: {1}", message, ex));
            }
        }

        public void LogInfo(string format, params object[] args)
        {
            var log = this.activityLog;
            if (log != null)
            {
                log.LogEntry((uint)__ACTIVITYLOG_ENTRYTYPE.ALE_INFORMATION, "BracketPairColorizer", string.Format(format, args));
            }
        }
    }
}
