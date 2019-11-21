using BracketPairColorizer.Languages.Utilities;
using Microsoft.VisualStudio.Text;

namespace BracketPairColorizer.Core.Utilities
{
    public class LineCharacters : StringCharacters
    {
        private ITextSnapshotLine line;
        private const char EOT = '\0';

        public LineCharacters(ITextSnapshotLine line, int start = 0)
            : base(line.GetText(), start)
        {
            this.line = line;
        }

        public override int AbsolutePosition => this.line.Start + this.AbsolutePosition;
        public override int End => this.line.End;
    }
}
