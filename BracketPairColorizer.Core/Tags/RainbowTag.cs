using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;

namespace BracketPairColorizer.Core.Tags
{
    public class RainbowTag : IClassificationTag
    {
        public IClassificationType ClassificationType { get; private set; }

        public RainbowTag(IClassificationType classification)
        {
            this.ClassificationType = classification;
        }
    }
}
