using BracketPairColorizer.Core.Compatibility;
using BracketPairColorizer.Core.Contracts;
using System;

namespace BracketPairColorizer.Core.Settings
{
    public static class PkgSource
    {
        private static Lazy<ILogger> logger = new Lazy<ILogger>(GetLogger);

        public static void LogError(string message, Exception ex)
        {
            if (logger.Value != null)
            {
                logger.Value.LogError(message, ex);
            }
        }

        private static ILogger GetLogger()
        {
            var model = new SComponentModel();

            return model.GetService<ILogger>();
        }
    }
}
