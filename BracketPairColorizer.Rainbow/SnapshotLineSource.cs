using BracketPairColorizer.Core.Utilities;
using BracketPairColorizer.Languages.Utilities;
using Microsoft.VisualStudio.Text;
using System;

namespace BracketPairColorizer.Rainbow
{
    public class SnapshotLineSource : ITextLinesSource
    {
        private ITextSnapshot snapshot;
        public int Length => snapshot.Length;

        public SnapshotLineSource(ITextSnapshot snapshot)
        {
            this.snapshot = snapshot ?? throw new ArgumentNullException(nameof(snapshot));
        }

        public ITextChars GetLineFromLineNumber(int lineNumber)
        {
            var line = snapshot.GetLineFromLineNumber(lineNumber);

            return new LineCharacters(line, 0);
        }

        public int GetLineNumberFromPosition(int position)
        {
            return snapshot.GetLineNumberFromPosition(position);
        }

        public bool IsSame(ITextLinesSource source)
        {
            return ((SnapshotLineSource)source).snapshot == this.snapshot;
        }
    }
}
