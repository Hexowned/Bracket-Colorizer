using BracketPairColorizer.Languages.Utilities;

namespace BracketPairColorizer.Languages.BraceScanners
{
    public class CSharpBraceScanner : IBraceScanner, IResumeControl
    {
        private const int stText = 0;
        private const int stString = 1;
        private const int stChar = 2;
        private const int stMultiLineComment = 4;
        private const int stIString = 5;

        private int status = stText;
        private int nestingLevel = 0;
        private int iStringNestLevel = 0;
        private bool parsingExpression = false;
        private bool multiLine = false;

        public string BraceList => "(){}[]";

        public CSharpBraceScanner()
        {
        }

        public void Reset(int state)
        {
            this.status = state & 0xFF;
            this.parsingExpression = (state & 0x08000000) != 0;
            this.nestingLevel = (state & 0xFF0000) >> 24;
            this.multiLine = (state & 0x04000000) != 0;
            this.iStringNestLevel = (state & 0xFF00) >> 16;
        }

        public bool CanResume(CharPosition brace)
        {
            return brace.State == stText;
        }

        public bool Extract(ITextChars tc, ref CharPosition pos)
        {
            // TODO:
        }

        private bool ParseText(ITextChars tc, ref CharPosition pos)
        {
            // TODO:
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

        private void ParseMultiLineString(ITextChars tc)
        {
            while (!tc.AtEnd)
            {
                if (tc.Char() == '"' && tc.NChar() == '"')
                {
                    tc.Skip(2);
                } else if (tc.Char() == '"')
                {
                    tc.Next();
                    this.status = stText;
                    this.multiLine = false;
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

        // TODO: This unfortunately doesnt handle all possible expressions...
        // handles the basic stuff
        private bool ParseInterpolatedString(ITextChars tc, ref CharPosition pos)
        {
            // TODO:
        }

        private int EncodedState()
        {
            int encoded = this.status;
            if (this.parsingExpression)
                encoded |= 0x08000000;
            if (this.multiLine)
                encoded |= 0x04000000;
            encoded |= (this.nestingLevel & 0xFF) << 24;
            encoded |= (this.iStringNestLevel & 0xFF) << 16;

            return encoded;
        }
    }
}
