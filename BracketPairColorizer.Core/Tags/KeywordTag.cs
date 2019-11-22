using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;

namespace BracketPairColorizer.Core.Tags
{
    public class KeywordTag : IClassificationTag
    {
        public IClassificationType ClassificationType { get; private set; }

        public KeywordTag(IClassificationType classification)
        {
            this.ClassificationType = classification;
        }
    }
}
