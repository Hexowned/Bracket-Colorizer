using System;

namespace BracketPairColorizer.Core.Contracts
{
    public interface ILogger
    {
        void LogInfo(string format, params object[] args);

        void LogError(string message, Exception ex);
    }
}
