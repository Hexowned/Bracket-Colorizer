﻿using BracketPairColorizer.Languages.Utilities;

namespace BracketPairColorizer.Languages.BraceScanners
{
    public class CBraceScanner : IBraceScanner
    {
        private const int stText = 0;
        private const int stString = 1;
        private const int stChar = 2;
        private const int stMultiLineComment = 4;
        private int status = stText;

        public string BraceList => "(){}[]";

        public CBraceScanner()
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
                    case stString:
                        ParseString(tc); break;

                    case stChar:
                        ParseCharLiteral(tc); break;

                    case stMultiLineComment:
                        ParseMultiLineComment(tc); break;

                    default:
                        return ParseText(tc, ref pos);
                }
            }

            return false;
        }

        private bool ParseText(ITextChars tc, ref CharPosition pos)
        {
            while (!tc.AtEnd)
            // multi line comment
            {
                if (tc.Char() == '/' && tc.NChar() == '*')
                {
                    this.status = stMultiLineComment;
                    tc.Skip(2);
                    this.ParseMultiLineComment(tc);
                } else if (tc.Char() == '/' && tc.NChar() == '/')
                {
                    tc.SkipRemainder();
                } else if (tc.Char() == '"')
                {
                    this.status = stString;
                    tc.Next();
                    this.ParseString(tc);
                } else if (IsHexDigit(tc.Char()) && tc.NChar() == '\'')
                // this is a C++ 14 digit separator, such as 1'000'000 or 0xFFFF'0000
                {
                    tc.Skip(2);
                } else if (tc.Char() == '\'')
                {
                    this.status = stString;
                    tc.Next();
                    this.ParseCharLiteral(tc);
                } else if (this.BraceList.IndexOf(tc.Char()) >= 0)
                {
                    pos = new CharPosition(tc.Char(), tc.AbsolutePosition);
                    tc.Next();

                    return true;
                } else { tc.Next(); }
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
                } else tc.Next();
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
                } else { tc.Next(); }
            }
        }

        private static bool IsHexDigit(char c)
        {
            return char.IsDigit(c)
                || (c >= 'a' && c <= 'f')
                || (c >= 'A' && c <= 'F');
        }
    }
}