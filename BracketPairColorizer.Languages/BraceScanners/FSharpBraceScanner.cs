using BracketPairColorizer.Languages.Utilities;

namespace BracketPairColorizer.Languages.BraceScanners
{
    public class FSharpBraceScanner : IBraceScanner, IResumeControl
    {
        private const int stText = 0;
        private const int stString = 1;
        private const int stChar = 2;
        private const int stVerbatimString = 3;
        private const int stMultiLineComment = 4;
        private const int stTripleQuotedString = 5;
        private int status = stText;

        public string BraceList => "(){}[]";

        public FSharpBraceScanner()
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
                    case stChar: ParseCharLiteral(tc); break;
                    case stMultiLineComment: ParseMultiLineComment(tc); break;
                    case stVerbatimString: ParseVerbatimString(tc); break;
                    case stTripleQuotedString: ParseTripleQuotedString(tc); break;
                    default:
                        return ParseText(tc, ref pos);
                }
            }

            return false;
        }

        public bool CanResume(CharPosition brace)
        {
            return brace.Char != '(';
        }

        public bool ParseText(ITextChars tc, ref CharPosition pos)
        {
            while (!tc.AtEnd)
            {
                if (tc.Char() == '(' && tc.NChar() == '*' && tc.NNChar() != ')')
                {
                    this.status = stMultiLineComment;
                    tc.Skip(2);
                    this.ParseMultiLineComment(tc);
                } else if (tc.Char() == '/' && tc.NChar() == '/')
                {
                    tc.SkipRemainder();
                } else if (tc.Char() == '@' && tc.NChar() == '"')
                {
                    this.status = stVerbatimString;
                    tc.Skip(2);
                    this.ParseVerbatimString(tc);
                } else if (tc.Char() == '"' && tc.NChar() == '"' && tc.NNChar() == '"')
                {
                    this.status = stTripleQuotedString;
                    tc.Skip(3);
                    this.ParseTripleQuotedString(tc);
                } else if (tc.Char() == '"')
                {
                    this.status = stString;
                    tc.Next();
                    this.ParseString(tc);
                } else if (tc.Char() == '<' && tc.NChar() == '\'')
                {
                    tc.Skip(2);
                } else if (char.IsLetterOrDigit(tc.Char()) && tc.NChar() == '\'')
                {
                    tc.Skip(2);
                } else if (tc.Char() == '\'')
                {
                    this.status = stChar;
                    tc.Next();
                    this.ParseCharLiteral(tc);
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

        public void ParseCharLiteral(ITextChars tc)
        {
            if (tc.Char() == '\\')
            {
                tc.Skip(2);
                while (!tc.AtEnd && tc.Char() != '\'')
                {
                    tc.Next();
                }
                tc.Next();
            } else
            {
                tc.Next();
                if (tc.Char() == '\'')
                {
                    tc.Next();
                }
            }

            this.status = stText;
        }

        private void ParseString(ITextChars tc)
        {
            while (!tc.AtEnd)
            {
                if (tc.Char() == '\\')
                {
                    tc.Skip(2);
                } else if (tc.Char() == '"')
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

        private void ParseVerbatimString(ITextChars tc)
        {
            while (!tc.AtEnd)
            {
                if (tc.Char() == '"')
                {
                    tc.Next();
                    this.status = stText;
                    return;
                } else
                {
                    tc.Next();
                }
            }
        }

        private void ParseTripleQuotedString(ITextChars tc)
        {
            while (!tc.AtEnd)
            {
                if (tc.Char() == '"' && tc.NChar() == '"' && tc.NNChar() == '"')
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

        private void ParseMultiLineComment(ITextChars tc)
        {
            while (!tc.AtEnd)
            {
                if (tc.Char() == '*' && tc.NChar() == ')')
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
