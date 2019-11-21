using BracketPairColorizer.Languages.Utilities;

namespace BracketPairColorizer.Languages.BraceScanners
{
    public class USQLBraceScanner : IBraceScanner
    {
        private const int stText = 0;
        private const int stString = 1;
        private const int stMultiLineComment = 4;
        private int status = stText;

        public string BraceList => "(){}[]";

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
                    case stMultiLineComment: ParseMultiLineComment(tc); break;
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
                if (tc.Char() == '/' && tc.NChar() == '*')
                {
                    tc.Skip(2);
                    this.status = stMultiLineComment;
                    ParseMultiLineComment(tc);
                } else if (tc.Char() == '/' && tc.NChar() == '/')
                {
                    tc.SkipRemainder();
                } else if (tc.Char() == '\'')
                {
                    this.status = stString;
                    tc.Next();
                    ParseCharLiteral(tc);
                } else if (tc.Char() == '"')
                {
                    this.status = stString;
                    tc.Next();
                    ParseString(tc);
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

        private void ParseCharLiteral(ITextChars tc)
        {
            while (!tc.AtEnd)
            {
                if (tc.Char() == '\\')
                {
                    tc.Skip(2);
                } else if (tc.Char() == '\'')
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

        private void ParseMultiLineComment(ITextChars tc)
        {
            while (!tc.AtEnd)
            {
                if (tc.Char() == '*' && tc.Char() == '/')
                {
                    tc.Skip(2);
                    this.status = stText;
                    break;
                } else
                {
                    tc.Next();
                }
            }
        }

        private void ParseString(ITextChars tc)
        {
            while (!tc.AtEnd)
            {
                if (tc.Char() == '\"')
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
