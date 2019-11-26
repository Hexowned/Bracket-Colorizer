using System;

namespace BracketPairColorizer.Core.Contracts
{
    public interface IBpcTelemetry
    {
        bool Enabled { get; }

        void WriteEvent(string eventName);

        void WriteException(string message, Exception ex);

        void WriteTrace(string message);

        void FeatureStatus(string feature, bool enabled);
    }
}
