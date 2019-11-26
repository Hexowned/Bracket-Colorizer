using BracketPairColorizer.Core.Contracts;
using BracketPairColorizer.Core.Settings;
using BracketPairColorizer.Settings.Settings;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Composition;

namespace BracketPairColorizer
{
    [Export(typeof(IBpcTelemetry))]
    public class TelemetryService : IBpcTelemetry
    {
        private readonly Telemetry telemetry;
        public bool Enabled => this.telemetry.Enabled;

        [ImportingConstructor]
        public TelemetryService(SVsServiceProvider serviceProvider, ITypedSettingsStore settings)
        {
            bool telemetryEnabled = settings.GetBoolean(nameof(IBpcSettings.TelemetryEnabled), true);
            var dte = (EnvDTE80.DTE2)serviceProvider.GetService(typeof(SDTE));
            this.telemetry = new Telemetry(telemetryEnabled, dte);
        }

        public void WriteEvent(string eventName)
        {
            this.telemetry.WriteEvent(eventName);
        }

        public void WriteException(string msg, Exception ex)
        {
            this.telemetry.WriteException(msg, ex);
        }

        public void WriteTrace(string message)
        {
            this.telemetry.WriteTrace(message);
        }

        public void FeatureStatus(string feature, bool enabled)
        {
            var evt = new EventTelemetry("Feature-" + feature);
            evt.Properties["enabled"] = enabled.ToString();
            this.telemetry.WriteEvent(evt);
        }
    }
}
