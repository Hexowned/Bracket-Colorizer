using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using System.Collections.Generic;

namespace BracketPairColorizer.Core.Outlining
{
    public interface IUserOutlining
    {
        IEnumerable<SnapshotSpan> GetTags(NormalizedSnapshotSpanCollection spans);

        void Add(SnapshotSpan span);

        void RemoveAt(SnapshotPoint point);

        void RemoveAll(ITextSnapshot snapshot);

        bool IsInOutliningRegion(SnapshotPoint point);

        bool HasUserOutlines();
    }

    public interface IOutliningManager
    {
        ITagger<IOutliningRegionTag> GetOutliningTagger();

        ITagger<IGlyphTag> GetGlyphTagger();
    }
}
