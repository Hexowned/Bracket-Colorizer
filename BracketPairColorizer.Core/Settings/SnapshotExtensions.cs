using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

namespace BracketPairColorizer.Core.Settings
{
    public static class SnapshotExtensions
    {
        public static SnapshotSpan GetSpan<T>(this IMappingTagSpan<T> tagSpan, ITextSnapshot snapshot) where T : ITag
        {
            var mappedSpans = tagSpan.Span.GetSpans(snapshot);

            return mappedSpans.Count > 0 ? mappedSpans[0] : new SnapshotSpan(snapshot, 0, snapshot.Length);
        }

        public static bool IsValid(this SnapshotPoint point)
        {
            return point.Position >= 0 && point.Position < point.Snapshot.Length;
        }

        public static SnapshotSpan Complete(this NormalizedSnapshotSpanCollection spans)
        {
            if (spans.Count == 0)
                return new SnapshotSpan();

            return new SnapshotSpan(spans[0].Start, spans[spans.Count - 1].End);
        }

        public static SnapshotSpan SpanUntil(this SnapshotPoint end)
        {
            return new SnapshotSpan(end.Snapshot, 0, end.Position);
        }

        public static ITagSpan<IClassificationTag> ToTagSpan(this IMappingTagSpan<IClassificationTag> tagSpan, ITextSnapshot snapshot)
        {
            var span = tagSpan.GetSpan(snapshot);

            return new TagSpan<IClassificationTag>(span, tagSpan.Tag);
        }
    }
}
