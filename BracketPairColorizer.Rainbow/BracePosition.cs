using BracketPairColorizer.Core.Tags;
using BracketPairColorizer.Languages;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;

namespace BracketPairColorizer.Rainbow
{
    public struct BracePosition
    {
        private readonly int depth;
        private readonly CharPosition charPosition;

        public char Brace => this.charPosition.Char;
        public int Depth => this.depth;
        public int Position => this.charPosition.Position;
        public int State => this.charPosition.State;

        public BracePosition(char ch, int pos, int depth)
        {
            this.charPosition = new CharPosition(ch, pos);
            this.depth = depth;
        }

        public BracePosition(CharPosition pos, int depth)
        {
            this.charPosition = pos;
            this.depth = depth;
        }

        public ITagSpan<RainbowTag> ToSpan(ITextSnapshot snapshot, IClassificationType type)
        {
            var span = new SnapshotSpan(snapshot, Position, 1);

            return new TagSpan<RainbowTag>(span, new RainbowTag(type));
        }
    }
}
