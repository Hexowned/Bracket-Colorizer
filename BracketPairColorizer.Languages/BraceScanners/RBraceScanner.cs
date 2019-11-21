using BracketPairColorizer.Languages.Utilities;

namespace BracketPairColorizer.Languages.BraceScanners
{
    public class RBraceScanner : IBraceScanner
    {
        private const int stText = 0;
        private const int stString = 1;
        private const int stSQString = 2;
        private int status = stText;

        public string BraceList => "(){}[]";

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
                    case stSQString: ParseSQString(tc); break;
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
                } else if (tc.Char() == '"')
                {
                    this.status = stString;
                    tc.Next();
                    this.ParseString(tc);
                } else if (tc.Char() == '\'')
                {
                    this.status = stSQString;
                    tc.Next();
                    this.ParseSQString(tc);
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
            ParseStringInt(tc, '"');
        }

        private void ParseSQString(ITextChars tc)
        {
            ParseStringInt(tc, '\'');
        }

        private void ParseStringInt(ITextChars tc, char quote)
        {
            while (!tc.AtEnd)
            {
                if (tc.Char() == '\\')
                {
                    tc.Skip(2);
                } else if (tc.Char() == quote)
                {
                    tc.Next();
                    this.status = stText;
                    break;
                } else
                {
                    tc.Next();
                }
            }
        }
    }
}
