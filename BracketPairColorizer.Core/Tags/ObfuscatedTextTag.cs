using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;

namespace BracketPairColorizer.Core.Tags
{
    public class ObfuscatedTextTag : IClassificationTag
    {
        public IClassificationType ClassificationType { get; private set; }

        public ObfuscatedTextTag(IClassificationType classification)
        {
            this.ClassificationType = classification;
        }
    }
}
