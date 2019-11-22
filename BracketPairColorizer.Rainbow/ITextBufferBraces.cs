using BracketPairColorizer.Languages;
using BracketPairColorizer.Rainbow.Settings;
using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;

namespace BracketPairColorizer.Rainbow
{
    public interface ITextBufferBraces
    {
        ITextSnapshot Snapshot { get; }
        string BraceCharacters { get; }
        int LastParsedPositon { get; }
        bool Enabled { get; }

        void Invalidate(SnapshotPoint startPoint);

        void UpdateSnapshot(ITextSnapshot snapshot);

        IEnumerable<BracePosition> BracesInSpans(NormalizedSnapshotSpanCollection spans);

        IEnumerable<CharPosition> ErrorBracesInSpans(NormalizedSnapshotSpanCollection spans);

        IEnumerable<BracePosition> BracesFromPosition(int position);

        Tuple<BracePosition, BracePosition> GetBracePair(SnapshotPoint point);

        Tuple<BracePosition, BracePosition> GetBracePairFromPosition(SnapshotPoint point, RainbowHighlightMode mode);
    }
}
