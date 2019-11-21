using BracketPairColorizer.Languages.Utilities;

namespace BracketPairColorizer.Languages.BraceScanners
{
    public class PythonBraceScanner : IBraceScanner
    {
        private const int stText = 0;
        private const int stString = 1;
        private const int stMultiLineString = 3;
        private int status = stText;
        private char quoteChar;

        public string BraceList => "(){}[]";

        public PythonBraceScanner()
        {
        }

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
                    case stString: ParseString(tc); break;
                    case stMultiLineString: ParseMultiLineString(tc); break;
                    default:
                        return ParseText(tc, ref pos);
                }
            }

            return false;
        }

        private bool ParseText(ITextChars tc, ref CharPosition pos)
        {
            while (!tc.AtEnd)
            {
                if (tc.Char() == '#')
                {
                    tc.SkipRemainder();
                } else if ((tc.Char() == '"' && tc.NChar() == '"' && tc.NNChar() == '"')
                            || (tc.Char() == '\'' && tc.NChar() == '\'' && tc.NNChar() == '\''))
                {
                    this.status = stMultiLineString;
                    this.quoteChar = tc.Char();
                    tc.Skip(3);
                    this.ParseMultiLineString(tc);
                } else if (tc.Char() == '\'' || tc.Char() == '"')
                {
                    this.status = stString;
                    this.quoteChar = tc.Char();
                    tc.Next();
                    this.ParseString(tc);
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

        private void ParseString(ITextChars tc)
        {
            while (!tc.AtEnd)
            {
                if (tc.Char() == '\\')
                {
                    tc.Skip(2);
                } else if (tc.Char() == this.quoteChar)
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

        private void ParseMultiLineString(ITextChars tc)
        {
            while (!tc.AtEnd)
            {
                if (tc.Char() == this.quoteChar && tc.NChar() == this.quoteChar && tc.NNChar() == this.quoteChar)
                {
                    tc.Skip(3);
                    this.status = stText;
                    return;
                } else
                {
                    tc.Next();
                }
            }
        }
    }
}
