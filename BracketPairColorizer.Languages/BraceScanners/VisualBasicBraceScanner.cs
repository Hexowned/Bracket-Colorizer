using BracketPairColorizer.Languages.Utilities;

namespace BracketPairColorizer.Languages.BraceScanners
{
    public class VisualBasicBraceScanner : IBraceScanner
    {
        private const int stText = 0;
        private const int stString = 1;
        private int status;

        public string BraceList => "(){}";

        public VisualBasicBraceScanner()
        {
            this.status = stText;
        }

        public void Reset(int state)
        {
            this.status = stText;
        }

        public bool Extract(ITextChars tc, ref CharPosition pos)
        {
            pos = CharPosition.Empty;
            while (!tc.AtEnd)
            {
                switch (this.status)
                {
                    case stString: ParseString(tc); break;
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
                if (tc.Char() == '\'')
                {
                    tc.SkipRemainder();
                } else if (tc.Char() == '"')
                {
                    this.status = stString;
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
                if (tc.Char() == '"' && tc.NChar() == '"')
                {
                    tc.Skip(2);
                } else if (tc.Char() == '"')
                {
                    this.status = stText;
                    tc.Next();
                    break;
                } else
                {
                    tc.Next();
                }
            }
        }
    }
}
