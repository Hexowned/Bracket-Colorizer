using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace BracketPairColorizer.Core.Outlining
{
    [Export(typeof(ITaggerProvider))]
    [ContentType(ContentTypes.Any)]
    [ContentType(ContentTypes.Projection)]
    [TagType(typeof(IOutliningRegionTag))]
    [TextViewRole(PredefinedTextViewRoles.Structured)]
    public class SelectionOutliningTaggerProvider : ITaggerProvider
    {
        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            var manager = SelectionOutliningManager.GetManager(buffer);
            if (typeof(T) == typeof(IOutliningRegionTag))
            {
                return manager.GetOutliningTagger() as ITagger<T>;
            }

            return null;
        }
    }
}
