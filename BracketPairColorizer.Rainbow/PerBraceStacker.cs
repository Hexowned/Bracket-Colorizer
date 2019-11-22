using BracketPairColorizer.Languages;
using System.Collections.Generic;

namespace BracketPairColorizer.Rainbow
{
    public class PerBraceStacker : IBraceStacker
    {
        private string braceList;
        private Dictionary<char, Stack<BracePosition>> stack;

        public PerBraceStacker(string braceList)
        {
            this.braceList = braceList;
            this.stack = new Dictionary<char, Stack<BracePosition>>();
            for (int i = 0; i < braceList.Length; i += 2)
            {
                var pairs = new Stack<BracePosition>();
                this.stack[braceList[i]] = pairs;
                this.stack[braceList[i + 1]] = pairs;
            }
        }

        public int Count(char brace) => this.stack[brace].Count;

        public BracePosition Pop(char brace) => this.stack[brace].Pop();

        public BracePosition Peek(char brace) => this.stack[brace].Peek();

        public BracePosition Push(CharPosition brace)
        {
            var pairs = this.stack[brace.Char];
            var bp = new BracePosition(brace, pairs.Count);
            pairs.Push(bp);

            return bp;
        }
    }
}
