using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace BracketPairColorizer
{
    public class Telemetry
    {
        private TelemetryClient client;
        public bool Enabled { get; private set; }

        public Telemetry(bool enabled, EnvDTE80.DTE2 dte = null)
        {
            var configuration = TelemetryConfiguration.CreateDefault();

            this.client = new TelemetryClient(configuration);
            this.client.InstrumentationKey = "";

            this.client.Context.User.Id = GetUserId();
            this.client.Context.Session.Id = Guid.NewGuid().ToString();
            this.client.Context.Properties.Add("Host", "VS");
            this.client.Context.Properties.Add("HostVersion", dte.Version);
            this.client.Context.Properties.Add("HostFullVersion", GetFullHostVersion());
            this.client.Context.Component.Version = GetProjectVersion();

            if (enabled && dte != null)
            {
                dte.Events.DTEEvents.OnBeginShutdown += OnBeginShutdown;
            }

            Enabled = enabled;

            WriteEvent("BracketPairColorizer Started!");
        }

        public void WriteEvent(string eventName)
        {
#if !DEBUG
            if (this.client != null && Enabled)
            {
                this.client.TrackEvent(new EventTelemetry(eventName));
            }
#endif
        }

        public void WriteEvent(EventTelemetry evt)
        {
#if !DEBUG
            if (this.client != null && Enabled)
            {
                this.client.TrackEvent(evt);
            }
#endif
        }

        public void WriteException(string msg, Exception ex)
        {
#if !DEBUG
            if (this.client != null && Enabled)
            {
                ExceptionTelemetry telemetry = new ExceptionTelemetry(ex);
                telemetry.Properties.Add("Message", msg);
                this.client.TrackException(telemetry);
            }
#endif
        }

        public void WriteTrace(string message)
        {
#if !DEBUG
            if (this.client != null && Enabled)
            {
                this.client.TrackTrace(message);
            }
#endif
        }

        private void OnBeginShutdown()
        {
            this.client.Flush();
        }

        private static string GetFullHostVersion()
        {
            try
            {
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string devEnvironment = Path.Combine(baseDirectory, "msenv.dll");
                var version = FileVersionInfo.GetVersionInfo(devEnvironment);

                return version.ProductVersion;
            } catch
            {
            }

            return "";
        }

        private static string GetProjectVersion()
        {
            var assembly = typeof(Telemetry).Assembly;
            var fileVersion = assembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute),
                              false).Cast<AssemblyFileVersionAttribute>().First().Version;

            return fileVersion;
        }

        private static string GetUserId()
        {
            var user = Environment.MachineName + "\\" + Environment.UserName;
            var bytes = Encoding.UTF8.GetBytes(user);
            using (var sha = SHA256.Create())
            {
                return Convert.ToBase64String(sha.ComputeHash(bytes));
            }
        }
    }
}
