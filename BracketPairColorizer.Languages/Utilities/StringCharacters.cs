using System;

namespace BracketPairColorizer.Languages.Utilities
{
    public class StringCharacters : ITextChars
    {
        private string text;
        private int position;
        private readonly int length;
        private int mark;
        private const int NO_MARK = -1;
        private const char EOT = '\0';

        public int Position => position;
        public virtual int AbsolutePosition => position;
        public bool AtEnd => position >= length;
        public virtual int End => Position + length;

        public StringCharacters(string text, int start = 0, int len = -1)
        {
            this.text = text;
            this.length = len < 0 ? text.Length : Math.Min(text.Length, len);
            this.position = start;
            this.mark = NO_MARK;
        }

        public char Char()
        {
            return Available(1) ? text[position] : EOT;
        }

        public char NChar()
        {
            return Available(2) ? text[position + 1] : EOT;
        }

        public char NNChar()
        {
            return Available(3) ? text[position + 2] : EOT;
        }

        public void Next()
        {
            Skip(1);
        }

        public void Skip(int count)
        {
            this.position += count;
        }

        public void SkipRemainder()
        {
            this.position = this.length;
        }

        public void Mark()
        {
            this.mark = Position;
        }

        public void ClearMark()
        {
            this.mark = NO_MARK;
        }

        public void BackToMark()
        {
            if (this.mark != NO_MARK)
            {
                this.position = this.mark;
                ClearMark();
            }
        }

        public string PreviousToken()
        {
            int startPos = this.Position - 1;
            // skip any whitespace
            for (; startPos > 0; startPos--)
            {
                if (!char.IsWhiteSpace(text[startPos]))
                    break;
            }
            int end = startPos;
            for (; startPos > 0; startPos--)
            {
                if (char.IsWhiteSpace(text[startPos]))
                    break;
            }
            if (startPos < 0) return "";
            return text.Substring(startPos, end - startPos + 1);
        }

        public string GetRemainder()
        {
            int remainder = this.length - this.position;
            if (remainder > 0)
            {
                return this.text.Substring(this.position, remainder);
            }
            return "";
        }

        private bool Available(int count)
        {
            return this.position + count - 1 < length;
        }
    }
}
