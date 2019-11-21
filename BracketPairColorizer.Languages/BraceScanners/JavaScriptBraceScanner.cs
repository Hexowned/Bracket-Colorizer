using BracketPairColorizer.Languages.Utilities;
using System.Linq;

namespace BracketPairColorizer.Languages.BraceScanners
{
    public class JavaScriptBraceScanner : IBraceScanner, IResumeControl
    {
        private const int stText = 0;
        private const int stString = 1;
        private const int stChar = 2;
        private const int stRegex = 4;
        private const int stMultiLineComment = 4;
        private const int stIString = 5;
        private int status = stText;
        private int nestingLevel = 0;
        private bool parsingExpression = false;

        public string BraceList => "(){}[]";

        public JavaScriptBraceScanner()
        {
        }

        public void Reset(int state)
        {
            this.status = state & 0xFF;
            this.parsingExpression = (state & 0x08000000) != 0;
            this.nestingLevel = (state & 0xFF0000) >> 24;
        }

        public bool CanResume(CharPosition brace)
        {
            return brace.State == stText;
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
                    case stIString:
                        if (ParseInterpolatedString(tc, ref pos)) { return true; }
                        break;

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
                    this.status = stMultiLineComment;
                    tc.Skip(2);
                    this.ParseMultiLineComment(tc);
                } else if (tc.Char() == '/' && tc.NChar() == '/')
                {
                    tc.SkipRemainder();
                } else if (tc.Char() == '/' && CheckPrevious(tc.PreviousToken()))
                {
                    tc.Next();
                    this.status = stRegex;
                    this.ParseRegex(tc);
                } else if (tc.Char() == '"')
                {
                    this.status = stString;
                    tc.Next();
                    this.ParseString(tc);
                } else if (tc.Char() == '\'')
                {
                    this.status = stString;
                    tc.Next();
                    this.ParseCharLiteral(tc);
                } else if (tc.Char() == '`')
                {
                    this.status = stIString;
                    tc.Next();
                    return this.ParseInterpolatedString(tc, ref pos);
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

        private bool CheckPrevious(string previous)
        {
            if (string.IsNullOrEmpty(previous)) { return true; }
            char last = previous[previous.Length - 1];

            return "(,=:[!&|?{};".Contains(last);
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

        private void ParseRegex(ITextChars tc)
        {
            while (!tc.AtEnd)
            {
                if (tc.Char() == '\\')
                {
                    tc.Skip(2);
                } else if (tc.Char() == '/')
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
                if (tc.Char() == '*' && tc.NChar() == '/')
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

        private bool ParseInterpolatedString(ITextChars tc, ref CharPosition pos)
        {
            while (!tc.AtEnd)
            {
                if (this.parsingExpression)
                {
                    if (ParseTemplateExpressionChar(tc, ref pos))
                        return true;
                } else
                {
                    if (tc.Char() == '\\')
                    {
                        tc.Skip(2);
                    } else if (tc.Char() == '$' && tc.NChar() == '{')
                    {
                        this.parsingExpression = true;
                        this.nestingLevel++;
                        tc.Next();
                        pos = new CharPosition(tc.Char(), tc.AbsolutePosition, EncodedState());
                        tc.Next();
                        return true;
                    } else if (tc.Char() == '`')
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

            return false;
        }

        private bool ParseTemplateExpressionChar(ITextChars tc, ref CharPosition pos)
        {
            if (tc.Char() == '"')
            {
                tc.Next();
                this.ParseString(tc);
                this.status = stIString;
            } else if (tc.Char() == '\'')
            {
                tc.Next();
                ParseCharLiteral(tc);
                this.status = stIString;
            } else if (tc.Char() == '}')
            {
                this.nestingLevel--;
                if (this.nestingLevel == 0)
                {
                    this.parsingExpression = false;
                }

                pos = new CharPosition(tc.Char(), tc.AbsolutePosition, EncodedState());
                tc.Next();
                return true;
            } else if (BraceList.Contains(tc.Char()))
            {
                pos = new CharPosition(tc.Char(), tc.AbsolutePosition, EncodedState());
                if (tc.Char() == '{')
                    this.nestingLevel++;
                tc.Next();
                return true;
            } else
            {
                tc.Next();
            }

            return false;
        }

        private int EncodedState()
        {
            int encoded = this.status;
            if (this.parsingExpression)
                encoded |= 0x08000000;
            encoded |= (this.nestingLevel & 0xFF) << 24;
            return encoded;
        }
    }
}
