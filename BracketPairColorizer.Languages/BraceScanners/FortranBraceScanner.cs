using BracketPairColorizer.Languages.Utilities;

namespace BracketPairColorizer.Languages.BraceScanners
{
    public class FortranBraceScanner : IBraceScanner
    {
        private const int stText = 0;
        private const int stStringSingle = 1;
        private const int stStringDouble = 2;
        private int status = stText;

        public string BraceList => "()";

        public void Reset(int state)
        {
            this.status = stText;
        }

        public bool Extract(ITextChars tc, ref CharPosition pos)
        {
            while (!tc.AtEnd)
            {
                switch (this.status)
                {
                    case stStringSingle: ParseStringSingle(tc); break;
                    case stStringDouble: ParseStringDouble(tc); break;
                    default:
                        return ParseText(tc, ref pos);
                }
            }

            return false;
        }

        public bool ParseText(ITextChars tc, ref CharPosition pos)
        {
            while (!tc.AtEnd)
            {
                if (tc.Char() == '!')
                {
                    tc.SkipRemainder();
                } else if (tc.Char() == '\'')
                {
                    this.status = stStringSingle;
                    tc.Next();
                    ParseStringSingle(tc);
                } else if (tc.Char() == '"')
                {
                    this.status = stStringDouble;
                    tc.Next();
                    ParseStringDouble(tc);
                } else if (this.BraceList.IndexOf(tc.Char()) >= 0)
                {
                    pos = new CharPosition(tc.Char(), tc.AbsolutePosition);
                    tc.Next();

                    return true;
                } else
                {
                    tc.Next();
                }
            }

            return false;
        }

        private void ParseStringSingle(ITextChars tc) => ParseString(tc, '\'');

        private void ParseStringDouble(ITextChars tc) => ParseString(tc, '"');

        private void ParseString(ITextChars tc, char quote)
        {
            while (!tc.AtEnd)
            {
                if (tc.Char() == quote && tc.NChar() == quote)
                {
                    tc.Skip(2);
                } else if (tc.Char() == quote)
                {
                    tc.Next();
                    break;
                } else
                {
                    tc.Next();
                }
            }

            this.status = stText;
        }
    }
}
