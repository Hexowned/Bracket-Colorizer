using BracketPairColorizer.Languages.Utilities;
using System.Linq;

namespace BracketPairColorizer.Languages.BraceScanners
{
    public class CssBraceScanner : IBraceScanner
    {
        private const int stText = 0;
        private const int stComment = 1;
        private const int stSingleQuotedString = 2;
        private const int stDoubleQuotedString = 3;
        private int state;

        public string BraceList => "(){}[]";

        public CssBraceScanner()
        {
        }

        public void Reset(int state)
        {
            this.state = stText;
        }

        public bool Extract(ITextChars tc, ref CharPosition pos)
        {
            while (!tc.AtEnd)
            {
                switch (this.state)
                {
                    case stComment:
                        ParseComment(tc); break;
                    case stSingleQuotedString:
                        ParseString(tc); break;
                    case stDoubleQuotedString:
                        ParseDString(tc); break;
                    default:
                        return ParseText(tc, ref pos);
                }
            }

            return false;
        }

        private bool ParseText(ITextChars tc, ref CharPosition pos)
        {
            pos = CharPosition.Empty;
            while (!tc.AtEnd)
            {
                if (tc.Char() == '/' && tc.NChar() == '*')
                {
                    this.state = stComment;
                    tc.Skip(2);
                    ParseComment(tc);
                } else if (tc.Char() == '/' && tc.NChar() == '/')
                {
                    tc.SkipRemainder();
                } else if (tc.Char() == '"')
                {
                    this.state = stDoubleQuotedString;
                    tc.Next();
                    ParseDString(tc);
                } else if (tc.Char() == '\'')
                {
                    this.state = stSingleQuotedString;
                    tc.Next();
                    ParseString(tc);
                } else if (this.BraceList.Contains(tc.Char()))
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
            ParseString(tc, '\'');
        }

        private void ParseString(ITextChars tc, char quote)
        {
            while (!tc.AtEnd)
            {
                if (tc.Char() == '\\')
                {
                    tc.Skip(2);
                } else if (tc.Char() == quote)
                {
                    tc.Next();
                    this.state = stText;
                    break;
                }

                tc.Next();
            }
        }

        private void ParseDString(ITextChars tc)
        {
            ParseString(tc, '"');
        }

        private void ParseComment(ITextChars tc)
        {
            while (!tc.AtEnd)
            {
                if (tc.Char() == '*' && tc.NChar() == '/')
                {
                    tc.Skip(2);
                    this.state = stText;
                    return;
                }

                tc.Next();
            }
        }
    }
}
