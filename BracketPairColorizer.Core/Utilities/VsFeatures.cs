using BracketPairColorizer.Core.Contracts;
using Microsoft.VisualStudio.Language.Intellisense;
using System;
using System.ComponentModel.Composition;

namespace BracketPairColorizer.Core.Utilities
{
    [Export(typeof(IVsFeatures))]
    public class VsFeatures : IVsFeatures
    {
        public bool IsSupported(string featureName)
        {
            switch (featureName)
            {
                case KnownFeatures.TooltipApi:
                    return IsQuickInfoSourceDeprecated();
            }

            throw new InvalidOperationException("Unknown feature: " + featureName);
        }

        private bool IsQuickInfoSourceDeprecated()
        {
            return typeof(IQuickInfoSource).GetCustomAttributes(typeof(ObsoleteAttribute), false).Length > 0;
        }
    }
}
