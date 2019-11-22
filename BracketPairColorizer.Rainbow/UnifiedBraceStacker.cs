using BracketPairColorizer.Languages;
using System.Collections.Generic;

namespace BracketPairColorizer.Rainbow
{
    public class UnifiedBraceStacker : IBraceStacker
    {
        private Stack<BracePosition> pairs = new Stack<BracePosition>();

        public int Count(char brace) => this.pairs.Count;

        public BracePosition Pop(char brace) => this.pairs.Pop();

        public BracePosition Peek(char brace) => this.pairs.Peek();

        public BracePosition Push(CharPosition brace)
        {
            var bp = new BracePosition(brace, this.pairs.Count);
            this.pairs.Push(bp);

            return bp;
        }
    }
}
