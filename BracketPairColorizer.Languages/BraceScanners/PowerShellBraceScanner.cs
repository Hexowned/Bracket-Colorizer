using BracketPairColorizer.Languages.Utilities;

namespace BracketPairColorizer.Languages.BraceScanners
{
    public class PowerShellBraceScanner : IBraceScanner
    {
        private const int stText = 0;
        private const int stString = 1;
        private const int stExpandableString = 2;
        private const int stHereString = 3;
        private const int stHereExpandableString = 4;
        private const int stMultiLineComment = 5;
        private int status = stText;

        public string BraceList => "(){}[]";

        public PowerShellBraceScanner()
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
                    case stExpandableString: ParseExpandableString(tc); break;
                    case stMultiLineComment: ParseMultiLineComment(tc); break;
                    case stHereString: ParseHereString(tc); break;
                    case stHereExpandableString: ParseHereExpandableString(tc); break;
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
                if (tc.Char() == '<' && tc.NChar() == '#')
                {
                    this.status = stMultiLineComment;
                    tc.Skip(2);
                    this.ParseMultiLineComment(tc);
                } else if (tc.Char() == '#')
                {
                    tc.SkipRemainder();
                } else if (tc.Char() == '@' && tc.NChar() == '\'')
                {
                    this.status = stHereString;
                    tc.Skip(2);
                    this.ParseHereString(tc);
                } else if (tc.Char() == '@' && tc.NChar() == '"')
                {
                    this.status = stHereExpandableString;
                    tc.Skip(2);
                    this.ParseHereExpandableString(tc);
                } else if (tc.Char() == '"')
                {
                    this.status = stString;
                    tc.Next();
                    this.ParseExpandableString(tc);
                } else if (tc.Char() == '\'')
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
                if (tc.Char() == '\'')
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

        private void ParseHereString(ITextChars tc)
        {
            while (!tc.AtEnd)
            {
                if (tc.Char() == '\'' && tc.NChar() == '@')
                {
                    tc.Skip(2);
                    break;
                } else
                {
                    tc.Next();
                }
            }

            this.status = stText;
        }

        private void ParseExpandableString(ITextChars tc)
        {
            while (!tc.AtEnd)
            {
                if (tc.Char() == '`')
                {
                    tc.Skip(2);
                } else if (tc.Char() == '"')
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

        private void ParseHereExpandableString(ITextChars tc)
        {
            while (!tc.AtEnd)
            {
                if (tc.Char() == '`')
                {
                    tc.Skip(2);
                } else if (tc.Char() == '"' && tc.NChar() == '@')
                {
                    tc.Skip(2);
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
                if (tc.Char() == '#' && tc.NChar() == '>')
                {
                    tc.Skip(2);
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
